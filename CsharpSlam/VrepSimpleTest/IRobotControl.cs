namespace CSharpSlam
{
    using System;

    /// <summary>
    ///     This interface describes every important function for the Neobotix robot simulation.
    /// </summary>
    internal interface IRobotControl
    {
        /// <summary>
        ///     Function for connecting to the robot.
        /// </summary>
        /// <param name="host">Address of the robot.</param>
        /// <returns>Returns error codes about the success of the connection.</returns>
        int Connect(string host);

        /// <summary>
        ///     Disconnects the robot.
        /// </summary>
        void Disconnect();

        double[] GetWheelSpeed();

        /// <summary>
        ///     Sets the speed of the joints of the robot.
        /// </summary>
        /// <param name="r">The right joint.</param>
        /// <param name="l">The left joint.</param>
        void SetWheelSpeed(double r, double l);

        void SetWheelSpeed(double[] linAng);

        /// <summary>
        ///     Starts, stops or resets the V-REP EDU simulation according to the given command.
        /// </summary>
        /// <param name="command">The given command.</param>
        void SetSimulationState(SimulationCommand command);
        
        /// <summary>
        ///     Clears the layer data collected for the map.
        /// </summary>
        void ClearMap();

        Layers GetLayers();

        /// <summary>
        ///     Gets or sets the state of the simulation.
        /// </summary>
        bool SimulationIsRunning { get; set; }
        
        /// <summary>
        ///     Event handler for the simulation state change.    
        /// </summary>
        event EventHandler SimulationStateChanged;

        /// <summary>
        ///     Trigger for the simulation state change event.
        /// </summary>
        void OnSimulationStateChanged();

    }
}
