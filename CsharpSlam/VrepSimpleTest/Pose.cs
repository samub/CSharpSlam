namespace CSharpSlam
{
    public struct Pose
    {
        /*public enum Type
        {
            Raw, Calculated
        }*/
        public readonly int X;
        public readonly int Y;
        public readonly double Degree;

        public Pose(int x, int y, double degree)
        {
            X = x;
            Y = y;
            Degree = degree;
        }
    }
}
