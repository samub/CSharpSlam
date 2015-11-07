using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VrepSimpleTest
{
    
     class Localization
    {
        Control Control;
        MapBuilder MapBuilder;
        Pose Pose;

        /// <summary>
        /// Boldog karácsonyt :)
        /// https://sarkanydavid.com/files/sadsa/halt_and_catch_fire.gif
        /// Szeretettel: MapBuilder csapat
        /// </summary>
        public void CalculatePose(){
            do {
                float[] pos = Control.GetPosition();
                float[] ori = Control.GetOriantation();
                if(pos[0] != 0 && pos[1] != 0 && ori[0] != 0)
                    this.Pose = new Pose((int)(pos[2] * Control.MapZoom), (int)(pos[1] * Control.MapZoom), 180.0 * ori[0] / Math.PI);
                //Debug.WriteLine("Position: x: " + pos[2] + " y: " + pos[1] + " degree: " + ori[0]);
                Thread.Sleep(1000);
            } while (true);
        }

        public void SetMapbuilder(MapBuilder MapBuilder){
            this.MapBuilder = MapBuilder;
        }

        public Localization(Control control) {
            this.Control = control;
        }

        public Pose GetPose() {
            return Pose;
        }

        /*public  Pose GetPose(Type Type, Layers Layers) {
            Pose pose = new Pose();
            
            return pose;
        }*/

      
    }
}
