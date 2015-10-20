using remoteApiNETWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VrepSimpleTest
{
    class Control
    {
        public MapBuilder MapBuilder { get; set; }
        public Localization Localization { get; set; }

         int _clientID=-1;
         int _handleNeo0, _handleNeo1;
         int _handleLeftMotor0, _handleRightMotor0;
         int  _handleSick0;
         bool _connected = false;


        public Control()
        {
            MapBuilder = new MapBuilder();
            Localization = new Localization();
            init();
        }

        public  int  Connect() {
            if (!_connected) // If not connected - try to connect
            {
                try
                {
                    _clientID = VREPWrapper.simxStart("127.0.0.1", 19997, true, true, 5000, 5);
                }
                catch (DllNotFoundException ex)
                {
                    MessageBox.Show("remoteApi.dll missing");
                }

                if (_clientID != -1) // Successfully connected to V-REP
                {
                    Debug.WriteLine("Connected to V-REP");
                    _connected = true;
                    init();
                    return 0;
                }
                else // Connection trial failed
                {
                    Debug.WriteLine("Error connecting to V-REP");
                    MessageBox.Show("Error connecting to V-REP :(");
                    _connected = false;
                    return -1;
                }
            }
            else // If connected - try to disconnect 
            {
                VREPWrapper.simxFinish(_clientID);
                _connected = false;
                Debug.WriteLine("Disconnected from V-REP");

                return -2;
            }

        }

        public  void Disconnect()
        {
            if (_connected)
                VREPWrapper.simxFinish(_clientID);

        }

        private  void init() {
            
            VREPWrapper.simxGetObjectHandle(_clientID, "neobotix#0", out _handleNeo0, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle neobotix#0: " + _handleNeo0);
            VREPWrapper.simxGetObjectHandle(_clientID, "neobotix#1", out _handleNeo1, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle neobotix#1: " + _handleNeo1);
            VREPWrapper.simxGetObjectHandle(_clientID, "wheel_left#0", out _handleLeftMotor0, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle left motor #0: " + _handleLeftMotor0);
            VREPWrapper.simxGetObjectHandle(_clientID, "wheel_right#0", out _handleRightMotor0, simx_opmode.oneshot_wait);
            Debug.WriteLine("Handle right motor #0: " + _handleRightMotor0);
            VREPWrapper.simxGetObjectHandle(_clientID, "SICK_S300_fast#0", out _handleSick0, simx_opmode.oneshot_wait);

           


        }

     

        public void SetLeftWheelSpeed() { }
        public void SetRightWheelSpeed() { }

        public  void Forward() {
             VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleLeftMotor0, 2, simx_opmode.oneshot_wait);
               VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleRightMotor0, 2, simx_opmode.oneshot_wait);
        }
        public  void Backward() {
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleLeftMotor0, -2, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleRightMotor0, -2, simx_opmode.oneshot_wait);
        }
        public  void Left() {

            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleLeftMotor0, -10, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleRightMotor0, 10, simx_opmode.oneshot_wait);
        }
        public  void Right() {
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleLeftMotor0, 10, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleRightMotor0, -10, simx_opmode.oneshot_wait);
        }
        public  void Stop() {

            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleLeftMotor0, 0, simx_opmode.oneshot_wait);
            VREPWrapper.simxSetJointTargetVelocity(_clientID, _handleRightMotor0, 0, simx_opmode.oneshot_wait);
        }

        public  void ResetSimulation() {
            VREPWrapper.simxStopSimulation(_clientID, simx_opmode.oneshot_wait);
            Thread.Sleep(400);
            VREPWrapper.simxStartSimulation(_clientID, simx_opmode.oneshot_wait);

        }
    }
}
