namespace CSharpSlam
{
    using System.Collections.Generic;

    /// <summary>
    ///     Represents the three component layer for the map.
    /// </summary>
    internal class Layers
    {
        /// <summary>
        /// Stores data from the walls of the room.
        /// </summary>
        public readonly double[,] WallLayer;
        
        /// <summary>
        /// Stores data from the empty areas.
        /// </summary>
        public readonly double[,] EmptyLayer;
        
        /// <summary>
        /// Stores data from the robot's path.
        /// </summary>
        public readonly List<Pose> RobotPathList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Layers" /> class.
        /// </summary>
        /// <param name="size">Size of the layer arrays. Default value is <see cref="MapBuilder.MapSize" />.</param>
        public Layers(int size = MapBuilder.MapSize)
        {
            WallLayer = new double[size, size];
            EmptyLayer = new double[size, size];
            RobotPathList = new List<Pose>();
        }
    }
}
