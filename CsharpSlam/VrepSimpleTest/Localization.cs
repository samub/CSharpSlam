namespace CSharpSlam
{
    using System;
    using System.Threading;
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    internal class Localization
    {
        private Pose _pose;
        private float[] _pos = new float[3];
        private float[] _ori = new float[3];

        public event EventHandler PoseChanged;

        public int ClientId { private get; set; }

        public int HandleSick { private get; set; }

        public int HandleRelative { private get; set; }

        public double[,] CurrentRawDatas { private get; set; }
        public Layers Layers { private get; set; }

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

        public void GetPose()
        {
            VREPWrapper.simxGetObjectPosition(ClientId, HandleSick, HandleRelative, _pos, simx_opmode.oneshot_wait);
            VREPWrapper.simxGetObjectOrientation(ClientId, HandleSick, HandleRelative, _ori, simx_opmode.oneshot_wait);

            //mysterious formula
            if (_ori[0] < 0)
                _ori[1] = _ori[1] * -1 + (float)Math.PI / 2;
            else _ori[1] = _ori[1] + (float)Math.PI + (float)Math.PI / 2;

            Pose = new Pose((int)(_pos[0] * RobotControl.MapZoom), (int)(_pos[1] * RobotControl.MapZoom), 180.0 * _ori[2] / Math.PI);
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
        
    }
}
