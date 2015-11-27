namespace CSharpSlam
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    internal class RobotControl : IRobotControl
    {
        public const int MapZoom = 50;
        public const int MapSize = 2000;

        private int _clientId = -1;
        //private int _handleNeo;
        private int _handleLeftMotor, _handleRightMotor;
        private int _handleSick;
        private int _handleRelative;
        private bool _connected;
        private IntPtr _signalValuePtr;
        private int _signalLength;
        private Thread _mapBuilderThread;
        //private Thread _localizationThread;

        private int ind = 0;
        public RobotControl()
        {

        }

        private void CalculatePose(object sender, EventArgs e)
        {
            Localization.CurrentRawDatas = MapBuilder.LaserData;
            Localization.Layers = MapBuilder.Layers;
            Localization.GetPose();
            MapBuilder.Pose = Localization.Pose;
        }

        private MapBuilder MapBuilder { get; set; }

        private Localization Localization { get; set; }

        public int Connect()
        {
            // If not connected - try to connect
            if (!_connected)
            {
                try
                {
                    _clientId = VREPWrapper.simxStart(R.localhost, 19997, true, true, 5000, 5);
                }
                catch (DllNotFoundException)
                {
                    MessageBox.Show(R.ErrorMessage_DLL);
                }

                // Successfully connected to V-REP
                if (_clientId != -1)
                {
                    Debug.WriteLine("Connected to V-REP");
                    _connected = true;

                    InitHandlers();
                    MapBuilder = new MapBuilder();
                    Localization = new Localization
                    {
                        ClientId = _clientId,
                        HandleSick = _handleSick,
                        HandleRelative = _handleRelative
                    };
                    MapBuilder.RequestLaserScannerDataRefresh += RequestLaserScannerDataRefresh;
                    MapBuilder.CalculatePose += CalculatePose;

                    _mapBuilderThread = new Thread(MapBuilder.BuildLayers);
                    //_localizationThread = new Thread(Localization.GetPose);

                    try
                    {
                        //_localizationThread.Start();
                        //Thread.Sleep(1000);
                        _mapBuilderThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    return 0;
                }

                Debug.WriteLine("Error connecting to V-REP");
                MessageBox.Show("Error connecting to V-REP :(");
                _connected = false;

                return -1;
            }

            VREPWrapper.simxFinish(_clientId);
            _connected = false;
            Debug.WriteLine("Disconnected from V-REP");

            return -2;
        }

        public void Disconnect()
        {
            if (!_connected)
            {
                return;
            }

            VREPWrapper.simxFinish(_clientId);
            Localization.PoseChanged -= PoseChanged;
            MapBuilder.RequestLaserScannerDataRefresh -= RequestLaserScannerDataRefresh;
            _mapBuilderThread.Abort();
            //_localizationThread.Abort();
        }

        public void ResetSimulation()
        {
            VREPWrapper.simxStopSimulation(_clientId, simx_opmode.oneshot_wait);
            Thread.Sleep(400);
            VREPWrapper.simxStartSimulation(_clientId, simx_opmode.oneshot_wait);
        }

        public double[] GetWheelSpeed()
        {
            throw new NotImplementedException();
        }

        public void SetWheelSpeed(double r, double l)
        {
            VREPWrapper.simxSetJointTargetVelocity(_clientId, _handleLeftMotor, (float)r, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientId, _handleRightMotor, (float)l, simx_opmode.oneshot_wait);
        }

        public void SetWheelSpeed(double[] linAng)
        {
            throw new NotImplementedException();
        }

        private double[,] GetLaserScannerData()
        {
            double[,] laserScannerData;
            // reading the laser scanner stream 
            simx_error s = VREPWrapper.simxReadStringStream(_clientId, R.measuredData0, ref _signalValuePtr, ref _signalLength, simx_opmode.streaming);
            
            if (s != simx_error.noerror)
                return new double[0, 0];
            //Debug.WriteLine(s);
            //  Debug.WriteLine(String.Format("test: {0:X8} {1:D} {2:X8}", _signalValuePtr, _signalLength, _signalValuePtr+_signalLength));
            float[] f = new float[685 * 3];
            if (_signalLength / sizeof(float) >= f.GetLength(0))
            {
                //we managed to get the laserdatas from Vrep
                laserScannerData = new double[3, f.GetLength(0) / 3];

                // todo read the latest stream (this is not the latest)
                int i;
                unsafe
                {
                    float* pp = (float*)_signalValuePtr.ToPointer();
                    //Debug.WriteLine("pp: " + *pp);
                    for (i = 0; i < f.GetLength(0); i++)
                    {
                        f[i] = *pp++; // pointer to float array 
                    }
                }
                // reshaping the 1D [3*685] data to 2D [3, 685] > x, y, z coordinates
                for (i = 0; i < f.GetLength(0); i++)
                {
                    if (!(Math.Abs(f[i]) < 0.000001))
                    {
                        laserScannerData[i % 3, i / 3] = f[i];
                    }
                }

                return laserScannerData;
            }
            // we couldnt get the laserdata, so we return an empty array
            laserScannerData = new double[0, 0];

            return laserScannerData;
        }

        /// <summary>
        /// TODO: test function, should be removed later
        /// </summary>
        public Layers GetLayers()
        {
            return MapBuilder.Layers;
        }

        private void PoseChanged(object sender, EventArgs e)
        {
            MapBuilder.Pose = Localization.Pose;
        }

        private void RequestLaserScannerDataRefresh(object sender, EventArgs e)
        {
            MapBuilder.LaserData = GetLaserScannerData();
        }

        private void InitHandlers()
        {
            VREPWrapper.simxGetObjectHandle(_clientId, R.wheelLeft0, out _handleLeftMotor, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle left motor: " + _handleLeftMotor);
            VREPWrapper.simxGetObjectHandle(_clientId, R.wheelRight0, out _handleRightMotor, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle right motor: " + _handleRightMotor);
            VREPWrapper.simxGetObjectHandle(_clientId, R.SICKS300, out _handleSick, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle laser scanner: " + _handleSick);

            VREPWrapper.simxGetObjectHandle(_clientId, "origo", out _handleRelative, simx_opmode.oneshot_wait);
        }
    }
}
