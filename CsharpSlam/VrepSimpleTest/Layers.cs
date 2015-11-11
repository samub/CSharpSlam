namespace CSharpSlam
{
    class Layers
    {
        public Layers() : this(MapBuilder.MapSize) { }
        public Layers(int size)
        {
            WallLayer = new double[size, size];
            EmptyLayer = new double[size, size];
            RobotPathLayer = new double[size, size];
        }
        public double[,] WallLayer;
        public double[,] EmptyLayer;
        public double[,] RobotPathLayer;
    }
}
