namespace CSharpSlam
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    internal class RobotControl : IRobot
    {
        public const int MapZoom = 50;
        public const int MapSize = 2000;
        private int _clientId = -1;
        private int _handleNeo;
        private int _handleLeftMotor, _handleRightMotor;
        private int _handleSick;
        private bool _connected;
        private Thread _mapBuilderThread;
        private Thread _localizationThread;

        public RobotControl()
        {
            MapBuilder = new MapBuilder
            {
                ClientId = _clientId
            };

            Localization = new Localization
            {
                ClientId = _clientId
            };

            Localization.PoseChanged += Localization_PoseChanged;
            InitHandlers();
        }
        
        public MapBuilder MapBuilder { get; set; }
        
        private Localization Localization { get; set; }

        public int Connect()
        {
            // If not connected - try to connect
            if (!_connected) 
            {
                try
                {
                    _clientId = VREPWrapper.simxStart(R.localhost, 19997, true, true, 5000, 5);
                    Localization.ClientId = _clientId;
                    MapBuilder.ClientId = _clientId;
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

                    _mapBuilderThread = new Thread(MapBuilder.BuildLayers);
                    _localizationThread = new Thread(Localization.CalculatePose);

                    try
                    {
                        _localizationThread.Start();
                        Thread.Sleep(1000);
                        _mapBuilderThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
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
            Localization.PoseChanged -= Localization_PoseChanged;
            _mapBuilderThread.Abort();
            _localizationThread.Abort();
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

        private void Localization_PoseChanged(object sender, EventArgs e)
        {
            MapBuilder.Pose = Localization.Pose;
        }

        private void InitHandlers()
        {
            //VREPWrapper.simxGetObjectHandle(_clientID, "neobotix#0", out _handleNeo, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle neobotix#0: " + _handleNeo);
            VREPWrapper.simxGetObjectHandle(_clientId, R.wheelLeft0, out _handleLeftMotor, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle left motor #0: " + _handleLeftMotor);
            VREPWrapper.simxGetObjectHandle(_clientId, R.wheelRight0, out _handleRightMotor, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle right motor #0: " + _handleRightMotor);
            VREPWrapper.simxGetObjectHandle(_clientId, R.SICKS300, out _handleSick, simx_opmode.oneshot_wait);
        }
    }
}
