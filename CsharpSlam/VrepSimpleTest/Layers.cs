using System.Collections.Generic;

namespace CSharpSlam
{
    class Layers
    {
        public Layers() : this(MapBuilder.MapSize) { }
        public Layers(int size)
        {
            WallLayer = new double[size, size];
            EmptyLayer = new double[size, size];
            RobotPathList = new List<Pose>();
        }
        public double[,] WallLayer;
        public double[,] EmptyLayer;
        public List<Pose> RobotPathList;
    }
}
