/// <summary>
/// Koszonjuk szepen a GetPose implementaciot. 
/// kb 7 ora kodolas aran sikerult "kicsit" javitani a project szerkezeten.
/// Hogy ne szakitsuk meg a hagyomanyt en is kuldok nektek mikulasra gifet.
/// Elso reakciom mikor meglattam mit muveltetek:
/// http://i.imgur.com/TP0zh.gif
/// 
/// With love: Lokalizacio csapat
/// </summary>

namespace CSharpSlam
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    internal class MapBuilder
    {
        private Layers _layers;
        private int _xW, _yW;

        public event EventHandler RequestLaserScannerDataRefresh;

        public MapBuilder()
        {
            LaserData = new double[3, 685];
        }

        public Pose Pose { private get; set; }

        public double[,] LaserData { private get; set; }

        public void BuildLayers()
        {
            do
            {
                Debug.WriteLine("x: {0}, y: {1}, degree: {2}", Pose.X, Pose.Y, Pose.Degree);
                //LaserData = GetLaserScannerData();
                double theta = Pose.Degree / 180 * Math.PI;
                for (int i = 0; i < LaserData.GetLength(1); i++)
                {
                    double tmpx = (Math.Cos(theta) * LaserData[0, i]) - (Math.Sin(theta) * LaserData[1, i]);
                    double tmpy = (Math.Sin(theta) * LaserData[0, i]) + (Math.Cos(theta) * LaserData[1, i]);
                    LaserData[0, i] = tmpx * RobotControl.MapZoom;
                    LaserData[1, i] = tmpy * RobotControl.MapZoom;
                    //Debug.WriteLine("Laser Data: " + LaserData[0, i] + " " + LaserData[1, i]);
                }
                //if we successfully got the laserdatas we create each layer
                if (LaserData.GetLength(0) > 0)
                {
                    CreateWallLayer();
                    CreateEmptyLayer();
                    CreateRobotPathLayer();
                }
                //Temporary solution for the problem. Needs to be changed.
                Thread.Sleep(4500);
                OnRequestLaserScannerDataRefresh();
                Thread.Sleep(500);
            }
            while (true);
        }

        public Layers GetLayers()
        {
            return _layers;
        }

        private void OnRequestLaserScannerDataRefresh()
        {
            if (RequestLaserScannerDataRefresh != null)
            {
                RequestLaserScannerDataRefresh(this, EventArgs.Empty);
            }
            ////TODO: kitorolni a c#5 kodot es cserelni 6-ra
            ////RequestLaserScannerDataRefresh?.Invoke(this, EventArgs.Empty);
        }

        private void CreateWallLayer()
        {
            _layers.WallLayer = new double[RobotControl.MapSize, RobotControl.MapSize];

            for (int i = 0; i < LaserData.GetLength(1); i++)
            {
                //TODO: itt kaptam egy OverflowExceptiont konvertalasnal. ki kell vizsgalni.
                ////Nem futott a szimulacio vegigkattintgattam a gombokat, 
                ////stop -ot tobbszor is majd miutan elinditottam akkor dobta
                _xW = Convert.ToInt32(LaserData[1, i]);
                _yW = Convert.ToInt32(LaserData[0, i]);
                if (_xW > 0 && _yW > 0 && _xW < RobotControl.MapSize && _yW < RobotControl.MapSize)
                {
                    _layers.WallLayer[_xW, _yW] = (1.0 + _layers.WallLayer[_xW, _yW]) / 2;
                }
            }
            //write to csv file for testing 
            //WriteToCSV(Layers.WallLayer, "WallLayer");
        }

        private void CreateEmptyLayer()
        {
            //TODO:
        }

        private void CreateRobotPathLayer()
        {
            //TODO:
        }

        private void WriteToCSV(double[,] multiDimensionalArray, string fileName)
        {
            string path = fileName + ".csv";
            IEnumerable<string> enumerator = multiDimensionalArray.Cast<double>()
                .Select((s, i) => (i + 1) % RobotControl.MapSize == 0 ? string.Concat(s, Environment.NewLine) : string.Concat(s, ";"));

            string item = string.Join(string.Empty, enumerator.ToArray());
            File.WriteAllText(path, item);
        }
    }
}
