using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpSlam
{
    class MapBuilder
    {
        public const int MapSize = 2000;
        private int centerX, centerY;

        public Layers Layers { get; private set; }
        public Pose Pose { private get; set; }
        public double[,] LaserData { get; set; }

        public event EventHandler RequestLaserScannerDataRefresh;
        public event EventHandler CalculatePose;

        public MapBuilder()
        {
            this.LaserData = new double[0, 0];
            Layers = new Layers();
            centerX = centerY = MapSize / 2;
        }

        
        public void BuildLayers()
        {
            //Első várakozás
            Thread.Sleep(3000);
            do
            {
                //Debug.WriteLine("x: {0}, y: {1}, degree: {2}", Pose.X, Pose.Y, Pose.Degree);

                RequestLaserScannerDataRefresh?.Invoke(this, EventArgs.Empty);
                if (LaserData.GetLength(0) == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                CalculatePose?.Invoke(this, EventArgs.Empty);

                double theta = Pose.Degree / 180 * Math.PI;
                theta *= -1;
                for (int i = 0; i < LaserData.GetLength(1); i++)
                {
                    double tmpx = Math.Cos(theta) * LaserData[0, i] - Math.Sin(theta) * LaserData[1, i];
                    double tmpy = Math.Sin(theta) * LaserData[0, i] + Math.Cos(theta) * LaserData[1, i];
                    LaserData[0, i] = tmpx * RobotControl.MapZoom;
                    LaserData[1, i] = tmpy * RobotControl.MapZoom;
                    //Debug.WriteLine("Laser Data: " + LaserData[0, i] + " " + LaserData[1, i]);
                }
                //if we successfully got the laserdatas we create each layer
                
                CreateWallLayer();
                CreateEmptyLayer();
                CreateRobotPathLayer();
                

                Thread.Sleep(1500);
            } while (true);
        }

        private void CreateWallLayer()
        {
            for (int i = 0; i < LaserData.GetLength(1); i++)
            {
                try {
                    Int32 xW = Convert.ToInt32((LaserData[1, i])) + centerX;
                    Int32 yW = Convert.ToInt32((LaserData[0, i])) + centerY;
                    if (xW > 0 && yW > 0 && xW < MapSize && yW < MapSize)
                    {
                        Layers.WallLayer[xW, yW] = (1.0 + Layers.WallLayer[xW, yW]) / 2;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("MapBuilder-CreateWallLayer() Exception: "+e.Message);
                };
            }
            //write to csv file for testing 
            //WriteToCSV(Layers.WallLayer, "WallLayer");

        }
        private void CreateEmptyLayer()
        {
        }
        private void CreateRobotPathLayer()
        {
        }

        private void WriteToCSV(double[,] multiDimensionalArray, string FileName)
        {
            String path = FileName + ".csv";
            var enumerator = multiDimensionalArray.Cast<double>()
                .Select((s, i) => (i + 1) % MapSize == 0 ? string.Concat(s, Environment.NewLine) : string.Concat(s, ";"));

            var item = String.Join("", enumerator.ToArray<string>());
            File.WriteAllText(path, item);
        }

    }
}
