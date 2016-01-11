namespace CSharpSlam
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    /// <summary>
    ///     Enumeration for the simulation commands.
    /// </summary>
    public enum SimulationCommand
    {
        /// <summary>Represents the stop command.</summary>
        Stop = 0,

        /// <summary>Represents the start command.</summary>
        Start = 1,

        /// <summary>Represents the reset command.</summary>
        Reset = 2
    }

    /// <summary>
    ///     Represents the control functions of a Neobotix robot and the visual appearance on the computer screen.
    /// </summary>
    internal class RobotControl : IRobotControl
    {
        /// <summary>Default zoom value of the map.</summary>
        public const int MapZoom = 50;

        /// <summary>The client ID.</summary>
        private int _clientId = -1;

        /// <summary>Handles for the motors.</summary>
        private int _handleLeftMotor, _handleRightMotor;

        /// <summary>Handle for the laser scanner.</summary>
        private int _handleSick;

        /// <summary>Handle for the origo.</summary>
        private int _handleRelative;

        /// <summary>Variable for the connection state.</summary>
        private bool _connected;

        /// <summary>Pointer to a pointer receiving the value of the signal.</summary>
        private IntPtr _signalValuePtr;

        /// <summary>The value of the signal length.</summary>
        private int _signalLength;

        /// <summary>Variable to able to draw the map on a new thread.</summary>
        private Thread _mapBuilderThread;

        /// <summary>Variable for the simulation state.</summary>
        private bool _simulationIsRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RobotControl" /> class.
        /// </summary>
        public RobotControl()
        {
            MapBuilder = new MapBuilder();
            Localization = new Localization();
        }

        /// <summary>
        ///     Event handler for the simulation state change.    
        /// </summary>
        public event EventHandler SimulationStateChanged;

        /// <summary>
        ///     Gets or sets the state of the simulation.
        /// </summary>
        public bool SimulationIsRunning
        {
            get
            {
                return _simulationIsRunning;
            }

            set
            {
                _simulationIsRunning = value;
                OnSimulationStateChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the reference of the MapBuilder class.
        /// </summary>
        private MapBuilder MapBuilder { get; set; }

        /// <summary>
        ///     Gets or sets the reference of the Localization class.
        /// </summary>
        private Localization Localization { get; set; }

        /// <summary>
        ///     Trigger for the simulation state change event.
        /// </summary>
        public void OnSimulationStateChanged()
        {
            SimulationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Function for connecting to the robot.
        /// </summary>
        /// <param name="host">Address of the robot.</param>
        /// <returns>Returns error codes about the success of the connection.</returns>
        public int Connect(string host)
        {
            // If not connected - try to connect
            if (!_connected)
            {
                try
                {
                    _clientId = VREPWrapper.simxStart(host, 19997, true, true, 5000, 5);
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

                    return 0;
                }

                Debug.WriteLine("Error connecting to V-REP");
                MessageBox.Show("Error connecting to V-REP");
                _connected = false;

                return -1;
            }

            Disconnect();
            Debug.WriteLine("Disconnected from V-REP");
            _connected = false;

            return -2;
        }

        /// <summary>
        ///     Disconnects the robot.
        /// </summary>
        public void Disconnect()
        {
            if (!_connected)
            {
                return;
            }

            SetSimulationState(SimulationCommand.Stop);
            Thread.Sleep(1000);
            VREPWrapper.simxFinish(-1);
        }

        /// <summary>
        ///     Starts, stops or resets the V-REP EDU simulation according to the given command.
        /// </summary>
        /// <param name="command">The given command.</param>
        public void SetSimulationState(SimulationCommand command)
        {
            switch (command)
            {
                case SimulationCommand.Stop:
                    StopSimulation();
                    break;
                case SimulationCommand.Start:
                    StartSimulation();
                    break;
                case SimulationCommand.Reset:
                    //StopSimulation();
                    VREPWrapper.simxStopSimulation(_clientId, simx_opmode.oneshot_wait);
                    Thread.Sleep(400);
                    //StartSimulation();
                    VREPWrapper.simxStartSimulation(_clientId, simx_opmode.oneshot_wait);
                    break;
            }
        }

        public double[] GetWheelSpeed()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sets the speed of the joints of the robot.
        /// </summary>
        /// <param name="r">The right joint.</param>
        /// <param name="l">The left joint.</param>
        public void SetWheelSpeed(double r, double l)
        {
            VREPWrapper.simxSetJointTargetVelocity(_clientId, _handleLeftMotor, (float)r, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientId, _handleRightMotor, (float)l, simx_opmode.oneshot_wait);
        }

        public void SetWheelSpeed(double[] linAng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: test function, should be removed later
        /// </summary>
        public Layers GetLayers()
        {
            return MapBuilder.Layers;
        }

        /// <summary>
        ///     Clears the layer data collected for the map.
        /// </summary>
        public void ClearMap()
        {
            MapBuilder.Layers = new Layers();
        }

        /// <summary>
        ///     Starts the simulation in the V-REP EDU application and begins drawing the map on a new thread.
        /// </summary>
        private void StartSimulation()
        {
            VREPWrapper.simxGetObjectHandle(_clientId, R.WheelLeft0, out _handleLeftMotor, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectHandle(_clientId, R.WheelRight0, out _handleRightMotor, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectHandle(_clientId, R.SICKS300, out _handleSick, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectHandle(_clientId, R.Origo, out _handleRelative, simx_opmode.oneshot_wait);

            Debug.WriteLine("Handle left motor: " + _handleLeftMotor);
            Debug.WriteLine("Handle right motor: " + _handleRightMotor);
            Debug.WriteLine("Handle laser scanner: " + _handleSick);

            Localization.ClientId = _clientId;
            Localization.HandleSick = _handleSick;
            Localization.HandleRelative = _handleRelative;
            MapBuilder.RequestLaserScannerDataRefresh += RequestLaserScannerDataRefresh;
            MapBuilder.CalculatePose += CalculatePose;

            _mapBuilderThread = new Thread(MapBuilder.BuildLayers);

            try
            {
                _mapBuilderThread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            VREPWrapper.simxStartSimulation(_clientId, simx_opmode.oneshot_wait);
            SimulationIsRunning = true;
        }

        /// <summary>
        ///     Stops the simulation in the V-REP EDU application and aborts the map drawing thread.
        /// </summary>
        private void StopSimulation()
        {
            VREPWrapper.simxStopSimulation(_clientId, simx_opmode.oneshot_wait);
            SimulationIsRunning = false;
            _mapBuilderThread?.Abort();
            Localization.PoseChanged -= PoseChanged;
            MapBuilder.RequestLaserScannerDataRefresh -= RequestLaserScannerDataRefresh;
            MapBuilder.CalculatePose -= CalculatePose;
        }

        /// <summary>
        ///     Collects the laser scanner data from the simulator.
        /// </summary>
        /// <returns>Returns the collected laser scanner data.</returns>
        private double[,] GetLaserScannerData()
        {
            double[,] laserScannerData;
            // reading the laser scanner stream 
            simx_error s = VREPWrapper.simxReadStringStream(_clientId, R.MeasuredData0, ref _signalValuePtr, ref _signalLength, simx_opmode.streaming);

            if (s != simx_error.noerror)
            {
                return new double[0, 0];
            }

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
        ///     Switch Pose data between Localization and Mapbuilder classes.
        ///     Localization calculates the new Pose data from the given layers
        ///     by the Mapbuilder and then give it back to the sender.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void CalculatePose(object sender, EventArgs e)
        {
            Localization.CurrentRawDatas = MapBuilder.LaserData;
            Localization.Layers = MapBuilder.Layers;
            Localization.GetPose();
            MapBuilder.Pose = Localization.Pose;
        }

        /// <summary>
        ///     Switch Pose data between Localization and Mapbuilder classes when
        ///     any changes happened.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PoseChanged(object sender, EventArgs e)
        {
            MapBuilder.Pose = Localization.Pose;
        }

        /// <summary>
        ///     Request to refresh the laser scanner data.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void RequestLaserScannerDataRefresh(object sender, EventArgs e)
        {
            MapBuilder.LaserData = GetLaserScannerData();
        }
    }
}
