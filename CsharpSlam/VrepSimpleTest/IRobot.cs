using remoteApiNETWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VrepSimpleTest
{
    interface iRobot {

        int Connect();
        void Disconnect();
        double[] GetWheelSpeed();
        double[,] GetLaserScannerData();
        void SetWheelSpeed(double R, double L);
        void SetWheelSpeed(double[] LinAng);

    }
    
}
