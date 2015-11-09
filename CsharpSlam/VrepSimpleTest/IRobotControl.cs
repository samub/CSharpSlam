namespace CSharpSlam
{
    internal interface IRobotControl
    {
        int Connect();

        void Disconnect();

        double[] GetWheelSpeed();

        void SetWheelSpeed(double r, double l);

        void SetWheelSpeed(double[] linAng);

        void ResetSimulation();

        /// <summary>
        /// TODO: test function, should be removed later
        /// </summary>
        void GetLayers();
    }
}
