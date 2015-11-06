using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrepSimpleTest
{
    class MapBuilder
    {
        private const int MapSize = 640;
        private const int MapZoom = 50;
        private double[,] LaserData;
        private Control Control;
        private Layers Layers;
        private int xW, yW;

        public MapBuilder(Control c) {
            this.Control = c;
            this.LaserData = new double[3, 685];
           

        }
        public  Layers GetLayers() {
            

            LaserData = Control.GetLaserScannerData();
            // transform laserdata ...

            //if we successfully got the laserdatas we create each layer
            
            if (LaserData.GetLength(0) > 0)
            {
                CreateWallLayer();
                CreateEmptyLayer();
                CreateRobotPathLayer();
            }

            return Layers;
        }

        private void CreateWallLayer() {
            Layers.WallLayer = new double[MapSize, MapSize];
            
            for (int i = 0; i < LaserData.GetLength(1); i++) {
                xW = Convert.ToInt32(MapZoom * (LaserData[1, i]));
                yW = Convert.ToInt32(MapZoom * (LaserData[0, i]));
                if (xW > 0 && yW > 0 && xW < MapSize && MapSize < 685){
                    Layers.WallLayer[xW, yW] = 1;
                }
            }
            //write to csv file for testing 
            WriteToCSV(Layers.WallLayer, "WallLayer");

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
