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
using System.Windows.Shapes;
using BarCodeScanner.ViewModel;
using CommonServiceLocator;

namespace BarCodeScanner.View
{
    /// <summary>
    /// Логика взаимодействия для DataGridLogs.xaml
    /// </summary>
    public partial class DataGridLogs : Window
    {
        public DataGridLogs()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Current.GetInstance<DataGridLogsViewModel>();
        }
    }
}
