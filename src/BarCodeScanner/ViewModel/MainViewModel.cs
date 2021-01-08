using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using CommonServiceLocator;

using ReactiveUI;

namespace BarCodeScanner.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        public DashboardViewModel Dashboard { get; init; }
        public WorkersViewModel Workers { get; init; }
        public LogsViewModel Logs { get; init; }

        public MainViewModel()
        {
            Dashboard = ServiceLocator.Current.GetInstance<DashboardViewModel>();
            Workers = ServiceLocator.Current.GetInstance<WorkersViewModel>();
            Logs = ServiceLocator.Current.GetInstance<LogsViewModel>();
        }
    }
}
