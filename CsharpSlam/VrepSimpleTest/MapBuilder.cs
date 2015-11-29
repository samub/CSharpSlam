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
                //Lézer szkenner adatok lekérdezése
                //Ha hibás 1mp várakozás
                RequestLaserScannerDataRefresh?.Invoke(this, EventArgs.Empty);
                if (LaserData.GetLength(0) == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                //Túl közeli adatok kiszűrése
                List<double[]> ld = new List<double[]>();
                for (int i = 0; i < LaserData.GetLength(1); i++)
                {
                    if (Math.Sqrt(LaserData[0, i] * LaserData[0, i] + LaserData[1, i] * LaserData[1, i]) > 0.8)
                        ld.Add(new double[] { LaserData[0, i], LaserData[1, i] });
                }
                LaserData = CreateRectangularArray(ld);

                //Pozició kalkulálás a Lokalizáció osztály segítségével.
                CalculatePose?.Invoke(this, EventArgs.Empty);

                Debug.WriteLine("x: {0}, y: {1}, degree: {2}", Pose.X, Pose.Y, Pose.Degree);

                //A lézerszkenner adatainak transzformálása (eltolása / elforgatása) a pozició szerint.
                double theta = Pose.Degree / 180 * Math.PI;
                theta *= -1;
                for (int i = 0; i < LaserData.GetLength(1); i++)
                {
                    double tmpx = Math.Cos(theta) * LaserData[0, i] - Math.Sin(theta) * LaserData[1, i];
                    double tmpy = Math.Sin(theta) * LaserData[0, i] + Math.Cos(theta) * LaserData[1, i];
                    LaserData[0, i] = tmpx * RobotControl.MapZoom + Pose.X;
                    LaserData[1, i] = tmpy * RobotControl.MapZoom + Pose.Y;
                }
                
                //Layerek kalkulációja.
                CreateWallLayer();
                CreateEmptyLayer();
                CreateRobotPathLayer();
                
                Thread.Sleep(1500);
            } while (true);
        }

        private void CreateWallLayer()
        {
            //A lézerrel mért,adatok szerint növeljük az adott kordinátán lévő fal valószínűségét.
            for (int i = 0; i < LaserData.GetLength(1); i++)
            {
                try {
                    Int32 xW = Convert.ToInt32((LaserData[0, i])) + centerX;
                    Int32 yW = Convert.ToInt32((LaserData[1, i])) + centerY;
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
        }
        private void CreateEmptyLayer()
        {
            int xx = centerX + Pose.X;
            int yy = centerY + Pose.Y;
            //A lézerrel mért,adatok szerint növeljük a robot és a fal közötti pontokon az üresség valószínűségét.
            for (int i = 0; i < LaserData.GetLength(1); i++)
            {
                try
                {
                    int x = xx;
                    int y = yy;
                    Int32 x2 = Convert.ToInt32((LaserData[0, i])) + centerX;
                    Int32 y2 = Convert.ToInt32((LaserData[1, i])) + centerY;

                    //bresenham's algoritmus
                    int w = x2 - x;
                    int h = y2 - y;
                    int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
                    if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
                    if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
                    if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
                    int longest = Math.Abs(w);
                    int shortest = Math.Abs(h);
                    if (!(longest > shortest))
                    {
                        longest = Math.Abs(h);
                        shortest = Math.Abs(w);
                        if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                        dx2 = 0;
                    }
                    int numerator = longest >> 1;
                    for (int z = 0; z <= longest; z++)
                    {
                        if(x != x2 && y != y2)
                            Layers.EmptyLayer[x, y] = (1.0 + Layers.EmptyLayer[x, y]) / 2;
                        numerator += shortest;
                        if (!(numerator < longest))
                        {
                            numerator -= longest;
                            x += dx1;
                            y += dy1;
                        }
                        else
                        {
                            x += dx2;
                            y += dy2;
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine("MapBuilder-CreateEmptyLayer() Exception: " + e.Message);
                    continue;
                }
            }
        }
        private void CreateRobotPathLayer()
        {
            //Hozzáadjuk a robot aktuális pozicióját a listához.
            Pose p = new Pose(Pose.X + centerX, Pose.Y + centerY, Pose.Degree);
            if(!Layers.RobotPathList.Contains(p))
                Layers.RobotPathList.Add(p);
        }

        private void WriteToCSV(double[,] multiDimensionalArray, string FileName)
        {
            String path = FileName + ".csv";
            String item = String.Empty;
            for (int x = 0; x < multiDimensionalArray.GetLength(0); x++)
            {
                for (int y = 0; y < multiDimensionalArray.GetLength(1); y++)
                {
                    item += multiDimensionalArray[x, y] + ";";
                }
                item += (Environment.NewLine);
            }
                    File.WriteAllText(path, item);
        }

        static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            //Listából 2 dimaneziós tömb
            if (arrays.Count == 0)
                return new T[0, 0];
            // TODO: Validation and special-casing for arrays.Count == 0
            int minorLength = arrays[0].Length;
            T[,] ret = new T[minorLength, arrays.Count];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[j, i] = array[j];
                }
            }
            return ret;
        }

    }
}
