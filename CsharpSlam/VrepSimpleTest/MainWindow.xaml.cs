namespace CSharpSlam
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using R = Properties.Resources;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const double MinToShow = 0.9;
        private const double DefaultRobotSpeed = 5D;

        private static DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            SetDockPanelStateEnabled(false);
            RobotControl = new RobotControl();
            RobotControl.SimulationStateChanged += RobotControl_SimulationStateChanged;
            ComboBoxRobotType.Items.Add(R.Localhost);
            ComboBoxRobotType.Items.Add(R.TestRobotIP_001);
            ComboBoxRobotType.SelectedIndex = 0;
            TextBoxRobotSpeed.Text = SliderRobotSpeed.Value.ToString();
            UpdateRobotControlPanel();
        }

        private void RobotControl_SimulationStateChanged(object sender, EventArgs e)
        {
            UpdateRobotControlPanel();
        }

        private IRobotControl RobotControl { get; set; }

        private void SetDockPanelStateEnabled(bool enabled)
        {
            DockRobotControls.IsEnabled = enabled;
            DockCanvas.IsEnabled = enabled;
            DockRobotControls.Opacity = enabled ? 1 : 0.25;
            DockCanvas.Opacity = enabled ? 1 : 0.25;
            ComboBoxRobotType.IsEnabled = !enabled;
            ComboBoxRobotType.Opacity = !enabled ? 1 : 0.25;
        }

        private void SetButtons(int result)
        {
            switch (result)
            {
                case -1:
                    ButtonConnect.Background = Brushes.IndianRed;
                    ButtonConnect.Content = R.Connect;
                    SetDockPanelStateEnabled(false);
                    break;
                case -2:
                    ButtonConnect.Background = Brushes.IndianRed;
                    ButtonConnect.Content = R.Connect;
                    SetDockPanelStateEnabled(false);
                    _timer.Stop();
                    break;
                default:
                    ButtonConnect.Background = Brushes.LightSeaGreen;
                    ButtonConnect.Content = R.Disconnect;
                    SetDockPanelStateEnabled(true);
                    _timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(5000)
                    };
                    _timer.Tick += MapUpdate;
                    _timer.Start();
                    break;
            }
        }

        private void UpdateRobotControlPanel()
        {
            foreach (var item in StackRobotControls.Children)
            {
                if ((item is Button || item is CheckBox) && item != ButtonSimulationState)
                {
                    ((Control)item).IsEnabled = RobotControl.SimulationIsRunning;
                }
            }

            ButtonSimulationState.Content = RobotControl.SimulationIsRunning ? R.StopSimulation : R.StartSimulation;
            ButtonSimulationState.Background = RobotControl.SimulationIsRunning ? Brushes.LightSeaGreen : Brushes.IndianRed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            RobotControl.Disconnect();
            RobotControl.SimulationStateChanged -= RobotControl_SimulationStateChanged;
            Application.Current.Shutdown();
        }

        //private void ButtonClearCanvas_Click(object sender, RoutedEventArgs e)
        //{
        //    //CanvScan.Children.Clear();
        //}

        //private void ButtonLaserScanTest_Click(object sender, RoutedEventArgs e)
        //{
        //    /*Layers layers = RobotControl.GetLayers();
        //    PixelFormat pf = PixelFormats.Rgb24;
        //    int width, height, rawStride;
        //    byte[] pixelData;

        //    width =  MapBuilder.MapSize;
        //    height = MapBuilder.MapSize;
        //    rawStride = (width * pf.BitsPerPixel + 7) / 8;
        //    pixelData = new byte[rawStride * height];

        //    //SetPixel(5, 5, Colors.Red, pixelData, rawStride);
        //    for (int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++)
        //        {
        //            if (layers.RobotPathLayer[x, y] >= MinToShow)
        //            {
        //                SetPixel(x, y, Colors.Red, pixelData, rawStride);
        //            }
        //            else if (layers.WallLayer[x, y] >= MinToShow)
        //            {
        //                SetPixel(x, y, Colors.LightBlue, pixelData, rawStride);
        //            }
        //            else if (layers.EmptyLayer[x, y] >= MinToShow)
        //            {
        //                SetPixel(x, y, Colors.Gray, pixelData, rawStride);
        //            }
        //        }
        //    BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, pixelData, rawStride);
        //    ImageScan.Source = bitmap;*/
        //}

        private void MapUpdate(object o, EventArgs e)
        {
            MapRender();
        }

        private void MapRender()
        {
            //Lekérdezzük a layereket majd létrehozunk egy annyi képpontból álló bitmapet mint amekkora a felbontása a térképünknek, majd a bitmapen kirajzoljuk a térkép adatait a beállítások szerint.
            Layers layers = RobotControl.GetLayers();
            PixelFormat pf = PixelFormats.Rgb24;

            const int width = MapBuilder.MapSize;
            const int height = MapBuilder.MapSize;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[rawStride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (CheckBoxWallLayer.IsChecked == true && layers.WallLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.LightBlue, pixelData, rawStride);
                    }
                    else if (CheckBoxEmptyLayer.IsChecked == true && layers.EmptyLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.Gray, pixelData, rawStride);
                    }
                }
            }

            if (CheckBoxRobotPathLayer.IsChecked == true)
            {
                foreach (Pose pose in layers.RobotPathList)
                {
                    for (int x = pose.X - 5; x < pose.X + 5; x++)
                    {
                        for (int y = pose.Y - 5; y < pose.Y + 5; y++)
                        {
                            if (y >= 0 && y < MapBuilder.MapSize && x >= 0 && x < MapBuilder.MapSize)
                            {
                                SetPixel(x, y, Colors.Red, pixelData, rawStride);
                            }
                        }
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, pixelData, rawStride);
            ImageMap.Source = bitmap;
        }

        private static void SetPixel(int x, int y, Color color, byte[] buffer, int rawStride)
        {
            int xIndex = x * 3;
            int yIndex = y * rawStride;
            buffer[xIndex + yIndex] = color.R;
            buffer[xIndex + yIndex + 1] = color.G;
            buffer[xIndex + yIndex + 2] = color.B;
        }

        private void CheckBoxCheckStateChanged(object sender, RoutedEventArgs e)
        {
            MapRender();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            SetButtons(RobotControl.Connect(ComboBoxRobotType.Text));
        }

        private void ButtonSimulationStart_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetSimulationState(RobotControl.SimulationIsRunning ? SimulationCommand.Stop : SimulationCommand.Start);
        }

        private void ButtonResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetSimulationState(SimulationCommand.Reset);
            SliderRobotSpeed.Value = DefaultRobotSpeed;
        }

        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(SliderRobotSpeed.Value, SliderRobotSpeed.Value);
        }

        private void ButtonBackward_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(-SliderRobotSpeed.Value, -SliderRobotSpeed.Value);
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(-SliderRobotSpeed.Value, SliderRobotSpeed.Value);
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(SliderRobotSpeed.Value, -SliderRobotSpeed.Value);
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            RobotControl.SetWheelSpeed(0, 0);
        }

        private void SliderRobotSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TextBoxRobotSpeed != null)
            {
                TextBoxRobotSpeed.Text = SliderRobotSpeed.Value.ToString(CultureInfo.CurrentCulture);
            }
        }
    }
}
