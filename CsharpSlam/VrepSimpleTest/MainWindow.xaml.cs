namespace CSharpSlam
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using R = Properties.Resources;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const double MinToShow = 0.9;
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
            CanvScan.Children.Clear();
        }

        private void ButtonLaserScanTest_Click(object sender, RoutedEventArgs e)
        {
            double[,] WallLayer = RobotControl.GetLayers().WallLayer;

            double h = CanvScan.ActualHeight,
                   w = CanvScan.ActualWidth;

            double min = h < w ? h : w;

            double pixel = min / MapBuilder.MapSize;

            double left = (w - min) / 2,
                   top = (h - min) / 2;

            for (int x = 0; x < MapBuilder.MapSize; x++)
                for (int y = 0; y < MapBuilder.MapSize; y++)
                {
                    if (WallLayer[x, y] >= MinToShow)
                    {
                        Rectangle rect = new Rectangle();
                        rect.Stroke = Brushes.Black;
                        rect.StrokeThickness = 1;
                        rect.Width = 1;
                        rect.Height = 1;
                        Canvas.SetLeft(rect, x * pixel + left);
                        Canvas.SetTop(rect, y * pixel + top);
                        CanvScan.Children.Add(rect);
                    }
                }
        }
    }
}
