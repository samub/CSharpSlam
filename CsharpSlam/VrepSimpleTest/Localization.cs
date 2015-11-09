namespace CSharpSlam
{
    using System;
    using System.Threading;
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    internal class Localization
    {
        private Pose _pose;

        public event EventHandler PoseChanged;

        public int ClientId { private get; set; }

        public Pose Pose
        {
            get
            {
                return _pose;
            }

            private set
            {
                _pose = value;
                OnPoseChanged();
            }
        }

        public void CalculatePose()
        {
            do
            {
                float[] pos = GetPosition();
                float[] ori = GetOrientation();
                if (pos[0] != 0 && pos[1] != 0 && ori[0] != 0)
                {
                    Pose = new Pose((int)(pos[2] * RobotControl.MapZoom), (int)(pos[1] * RobotControl.MapZoom), 180.0 * ori[0] / Math.PI);
                }
                //Debug.WriteLine("Position: x: " + pos[2] + " y: " + pos[1] + " degree: " + ori[0]);
                Thread.Sleep(1000);
            }
            while (true);
        }

        private float[] GetOrientation()
        {
            float[] ori = new float[2];
            int handleNeo0, handleNeo1;
            VREPWrapper.simxGetObjectHandle(ClientId, R.neobotix0, out handleNeo0, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectHandle(ClientId, R.neobotix1, out handleNeo1, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectOrientation(ClientId, handleNeo0, handleNeo1, ori, simx_opmode.oneshot_wait);
            // x y
            return ori;
        }

        private float[] GetPosition()
        {
            int handleNeo0, handleNeo1;
            float[] pos = new float[3];
            VREPWrapper.simxGetObjectHandle(ClientId, R.neobotix0, out handleNeo0, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectHandle(ClientId, R.neobotix1, out handleNeo1, simx_opmode.oneshot_wait);

            VREPWrapper.simxGetObjectPosition(ClientId, handleNeo0, handleNeo1, pos, simx_opmode.oneshot_wait);
            //Debug.WriteLine("Position: x: " + pos[2] + " y: " + pos[1] + " z: " + pos[0]);
            return pos;
            //Debug.WriteLine(ori[0]+" "+ori[1]);
            //txtInfo.Text = "x: " + pos[2] + " y: " + pos[1] + " z: " + pos[0];

            //return new float[2] { pos[2], pos[1] };
        }

        private void OnPoseChanged()
        {
            if (PoseChanged != null)
            {
                PoseChanged(this, EventArgs.Empty);
            }
            ////TODO: kitorolni a c#5 kodot es cserelni 6-ra
            ////PoseChanged?.Invoke(this, EventArgs.Empty);
        }

        /*public  Pose GetPose(Type Type, Layers Layers) {
            Pose pose = new Pose();
            
            return pose;
        }*/
    }
}
