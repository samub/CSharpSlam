namespace CSharpSlam
{
    internal interface IRobot
    {
        int Connect();
        
        void Disconnect();
        
        double[] GetWheelSpeed();
        //double[,] GetLaserScannerData();
        
        void SetWheelSpeed(double r, double l);

        void SetWheelSpeed(double[] linAng);
    }
}
