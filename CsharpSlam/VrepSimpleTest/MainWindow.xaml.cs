namespace CSharpSlam
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using R = Properties.Resources;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Media.Imaging;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const double MinToShow = 0.9;
        public static DispatcherTimer timer;
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
                    timer.Stop();
                    break;
                default:
                    ButtonConnect.Background = Brushes.LightSeaGreen;
                    StackControls.Visibility = Visibility.Visible;
                    ButtonConnect.Content = R.Disconnect;
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(5000);
                    timer.Tick += MapUpdate;
                    timer.Start();
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
            //CanvScan.Children.Clear();
        }

        private void ButtonLaserScanTest_Click(object sender, RoutedEventArgs e)
        {
            /*Layers layers = RobotControl.GetLayers();
            PixelFormat pf = PixelFormats.Rgb24;
            int width, height, rawStride;
            byte[] pixelData;

            width =  MapBuilder.MapSize;
            height = MapBuilder.MapSize;
            rawStride = (width * pf.BitsPerPixel + 7) / 8;
            pixelData = new byte[rawStride * height];
            
            //SetPixel(5, 5, Colors.Red, pixelData, rawStride);


            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (layers.RobotPathLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.Red, pixelData, rawStride);
                    }
                    else if (layers.WallLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.LightBlue, pixelData, rawStride);
                    }
                    else if (layers.EmptyLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.Gray, pixelData, rawStride);
                    }
                }
                


            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, pixelData, rawStride);
            ImageScan.Source = bitmap;*/
        }

        void MapUpdate(object o, EventArgs e)
        {
            MapRender();
        }
        void MapRender()
        {
            Layers layers = RobotControl.GetLayers();
            PixelFormat pf = PixelFormats.Rgb24;
            int width, height, rawStride;
            byte[] pixelData;

            width = /*10;*/ MapBuilder.MapSize;
            height = /*10; */ MapBuilder.MapSize;
            rawStride = (width * pf.BitsPerPixel + 7) / 8;
            pixelData = new byte[rawStride * height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (cbWallLayer.IsChecked == true && layers.WallLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.LightBlue, pixelData, rawStride);
                    }
                    else if (cbEmptyLayer.IsChecked == true && layers.EmptyLayer[x, y] >= MinToShow)
                    {
                        SetPixel(x, y, Colors.Gray, pixelData, rawStride);
                    }
                }
            if(cbRobotLayer.IsChecked == true)
            foreach (Pose p in layers.RobotPathList)
                {
                    for (int x = p.X - 5; x < p.X + 5; x++)
                        for (int y = p.Y - 5; y < p.Y + 5; y++)
                            if (y >= 0 && y < MapBuilder.MapSize && x >= 0 && x < MapBuilder.MapSize)
                                SetPixel(x, y, Colors.Red, pixelData, rawStride);
                }
            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, pixelData, rawStride);
            ImageScan.Source = bitmap;
        }

        void SetPixel(int x, int y, Color c, byte[] buffer, int rawStride)
        {
            int xIndex = x * 3;
            int yIndex = y * rawStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void cbChange(object sender, RoutedEventArgs e)
        {
            MapRender();
        }
    }
}
