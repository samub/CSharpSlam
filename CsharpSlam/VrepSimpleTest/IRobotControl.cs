using System;

namespace CSharpSlam
{
    internal interface IRobotControl
    {
        int Connect(string host);

        void Disconnect();

        double[] GetWheelSpeed();

        void SetWheelSpeed(double r, double l);

        void SetWheelSpeed(double[] linAng);

        void SetSimulationState(SimulationCommand command);

        Layers GetLayers();

        bool SimulationIsRunning { get; set; }

        event EventHandler SimulationStateChanged;

        void OnSimulationStateChanged();
    }
}
