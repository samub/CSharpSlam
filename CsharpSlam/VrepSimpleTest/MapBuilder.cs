using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VrepSimpleTest
{
    class MapBuilder
    {
        private const int MapSize = 2000;
        private double[,] LaserData;
        private Control Control;
        private Layers Layers;
        private Localization Localization;
        private int xW, yW;

        public MapBuilder(Control c) {
            this.Control = c;
            this.LaserData = new double[3, 685];
        }

        public void SetLocalization(Localization Localization)
        {
            this.Localization = Localization;
        }

        public void BuildLayers(){
            do
            {
                Pose p = Localization.GetPose();
                Debug.WriteLine("x: {0}, y: {1}, degree: {2}",p.x, p.y, p.degree);
                
                LaserData = Control.GetLaserScannerData();
                double theta = p.degree / 180 * Math.PI;
                for(int i = 0; i < LaserData.GetLength(1); i++)
                {
                    double tmpx = Math.Cos(theta) * LaserData[0, i] - Math.Sin(theta) * LaserData[1, i];
                    double tmpy = Math.Sin(theta) * LaserData[0, i] + Math.Cos(theta) * LaserData[1, i];
                    LaserData[0, i] = tmpx * Control.MapZoom;
                    LaserData[1, i] = tmpy * Control.MapZoom;
                    //Debug.WriteLine("Laser Data: " + LaserData[0, i] + " " + LaserData[1, i]);
                }
                //if we successfully got the laserdatas we create each layer

                if (LaserData.GetLength(0) > 0)
                {
                    CreateWallLayer();
                    CreateEmptyLayer();
                    CreateRobotPathLayer();
                }

                Thread.Sleep(5000);
            } while (true);
        }
        public  Layers GetLayers() {
            return Layers;
        }

        private void CreateWallLayer() {
            Layers.WallLayer = new double[MapSize, MapSize];
            
            for (int i = 0; i < LaserData.GetLength(1); i++) {
                xW = Convert.ToInt32((LaserData[1, i]));
                yW = Convert.ToInt32((LaserData[0, i]));
                if (xW > 0 && yW > 0 && xW < MapSize && yW < MapSize){
                    Layers.WallLayer[xW, yW] = (1.0 + Layers.WallLayer[xW, yW]) / 2;
                }
            }
            //write to csv file for testing 
            //WriteToCSV(Layers.WallLayer, "WallLayer");

        }
        private void CreateEmptyLayer() {
        }
        private void CreateRobotPathLayer() {
        }

        private void WriteToCSV(double[,] multiDimensionalArray, string FileName) {
            String path = FileName + ".csv";
            var enumerator = multiDimensionalArray.Cast<double>()
                .Select((s, i) => (i + 1) % MapSize == 0 ? string.Concat(s, Environment.NewLine) : string.Concat(s, ";"));

            var item = String.Join("", enumerator.ToArray<string>());
            File.WriteAllText(path, item);




        }



    }
}
