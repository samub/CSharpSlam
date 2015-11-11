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
        
        Layers GetLayers();
    }
}
