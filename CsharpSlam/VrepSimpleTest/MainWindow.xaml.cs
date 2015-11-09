namespace CSharpSlam
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using R = Properties.Resources;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            StackControls.Visibility = Visibility.Hidden;
            RobotControl = new RobotControl();
        }

        private IRobotControl RobotControl { get; set; }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            SetButtons(RobotControl.Connect());
        }

        private void SetButtons(int result)
        {
            switch (result)
            {
                case -1:
                    ButtonConnect.Background = Brushes.IndianRed;
                    StackControls.Visibility = Visibility.Hidden;
                    break;
                case -2:
                    ButtonConnect.Background = Brushes.IndianRed;
                    ButtonConnect.Content = R.Reconnect;
                    StackControls.Visibility = Visibility.Hidden;
                    break;
                default:
                    ButtonConnect.Background = Brushes.LightSeaGreen;
                    StackControls.Visibility = Visibility.Visible;
                    ButtonConnect.Content = R.Disconnect;
                    break;
            }
        }

        private void ButtonFwd_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(5, 5);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(-5, -5);
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(0, 0);
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(5, 0);
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(0, 5);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //TODO: az app nem szunteti meg a processt kilepesnel es hagyja futni a szimulaciot
            RobotControl.Disconnect();
            Application.Current.Shutdown();
        }

        private void ButtonResetSim_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.ResetSimulation();
        }

        private void ButtonClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            //TODO:
        }

        private void ButtonLaserScanTest_Click(object sender, RoutedEventArgs e)
        {
            //test function, should be removed later
            RobotControl.GetLayers();
        }
    }
}
