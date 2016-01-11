namespace CSharpSlam
{
    /// <summary>
    ///     Must be edited.
    /// </summary>
    public struct Pose
    {
        /// <summary>
        /// Must be edited.
        /// </summary>
        public readonly int X;
        
        /// <summary>
        /// Must be edited.
        /// </summary>
        public readonly int Y;
        
        /// <summary>
        /// Must be edited.
        /// </summary>
        public readonly double Degree;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pose" /> struct.  
        /// </summary>
        /// <param name="x">The x coordinate parameter.</param>
        /// <param name="y">The y coordinate parameter.</param>
        /// <param name="degree">The degree parameter.</param>
        public Pose(int x, int y, double degree)
        {
            X = x;
            Y = y;
            Degree = degree;
        }
    }
}
