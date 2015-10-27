using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrepSimpleTest
{
     class Localization
    {
        Control Control;

        public Localization(Control control) {
            this.Control = control;
        }

        public  Pose GetPose(Type Type, Layers Layers) {
            Pose pose = new Pose();
            
            return pose;
        }

      
    }
}
