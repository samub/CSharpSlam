using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using remoteApiNETWrapper;

namespace VrepSimpleTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        
       
        Control Control;
    
        

        public MainWindow()
        {
            InitializeComponent();
            stackControls.Visibility = Visibility.Hidden;
            
            Control = new Control();
            

        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            int result =Control.Connect();
            SetButtons(result);

        }

        public void SetButtons(int result) {

            if (result == -1)
            {
                buttonConnect.Background = Brushes.IndianRed;
                stackControls.Visibility = Visibility.Hidden;
            }
            else if (result == -2)
            {
                buttonConnect.Background = Brushes.IndianRed;
                buttonConnect.Content = "Reconnect";
                stackControls.Visibility = Visibility.Hidden;
              

            }
            else
            {

                buttonConnect.Background = Brushes.LightSeaGreen;
                stackControls.Visibility = Visibility.Visible;
                buttonConnect.Content = "Disconnect";

            }

        }


        private void buttonFwd_Click(object sender, RoutedEventArgs e)
        {
            Control.SetWheelSpeed(5, 5);

        }

        private void buttonBck_Click(object sender, RoutedEventArgs e)
        {
            Control.SetWheelSpeed(-5, -5);

        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            Control.SetWheelSpeed(0,0);
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            Control.SetWheelSpeed(5, 0);

        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            Control.SetWheelSpeed(0, 5);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
     
            Control.Disconnect();
        }

        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void buttonResetSim_Click(object sender, RoutedEventArgs e)
        {
            Control.ResetSimulation();
        }

        private void buttonClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            canvScan.Children.Clear();
        }
    }
}
