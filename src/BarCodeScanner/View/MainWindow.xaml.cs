using System;
using System.Windows;
using BarCodeScanner.ViewModel;
using System.Drawing;
using System.IO;

namespace BarCodeScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            NotifyIcon.Icon = new Icon(new FileStream("icons8_barcode.ico", FileMode.Open));
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                NotifyIcon.Visibility = Visibility.Visible;
            }
        }

        private void OpenClickTaskBar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            NotifyIcon.Visibility = Visibility.Hidden;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
