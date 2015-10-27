namespace VrepSimpleTest
{
    struct Pose
    {
        public enum Type
        {
            Raw, Calculated
        }
        public int x;
        public int y;
        public double degree;
    }
}