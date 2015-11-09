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
    using remoteApiNETWrapper;
    using R = Properties.Resources;

    internal class MapBuilder
    {
        private double[,] _laserData;
        private Layers _layers;
        private int _xW, _yW;
        private IntPtr _signalValuePtr;
        private int _signalLength;

        public MapBuilder()
        {
            _laserData = new double[3, 685];
        }

        public Pose Pose { private get; set; }

        public int ClientId { private get; set; }

        public void BuildLayers()
        {
            do
            {
                Debug.WriteLine("x: {0}, y: {1}, degree: {2}", Pose.X, Pose.Y, Pose.Degree);
                _laserData = GetLaserScannerData();
                double theta = Pose.Degree / 180 * Math.PI;
                for (int i = 0; i < _laserData.GetLength(1); i++)
                {
                    double tmpx = (Math.Cos(theta) * _laserData[0, i]) - (Math.Sin(theta) * _laserData[1, i]);
                    double tmpy = (Math.Sin(theta) * _laserData[0, i]) + (Math.Cos(theta) * _laserData[1, i]);
                    _laserData[0, i] = tmpx * RobotControl.MapZoom;
                    _laserData[1, i] = tmpy * RobotControl.MapZoom;
                    //Debug.WriteLine("Laser Data: " + LaserData[0, i] + " " + LaserData[1, i]);
                }
                //if we successfully got the laserdatas we create each layer
                if (_laserData.GetLength(0) > 0)
                {
                    CreateWallLayer();
                    CreateEmptyLayer();
                    CreateRobotPathLayer();
                }

                Thread.Sleep(5000);
            }
            while (true);
        }

        public Layers GetLayers()
        {
            return _layers;
        }

        private void CreateWallLayer()
        {
            _layers.WallLayer = new double[RobotControl.MapSize, RobotControl.MapSize];

            for (int i = 0; i < _laserData.GetLength(1); i++)
            {
                _xW = Convert.ToInt32(_laserData[1, i]);
                _yW = Convert.ToInt32(_laserData[0, i]);
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

        private double[,] GetLaserScannerData()
        {
            double[,] laserScannerData;
            // reading the laser scanner stream 
            VREPWrapper.simxReadStringStream(ClientId, R.measuredData0, ref _signalValuePtr, ref _signalLength, simx_opmode.streaming);

            //  Debug.WriteLine(String.Format("test: {0:X8} {1:D} {2:X8}", _signalValuePtr, _signalLength, _signalValuePtr+_signalLength));
            float[] f = new float[685 * 3];
            if (_signalLength >= f.GetLength(0))
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
