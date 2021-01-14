using System.Windows;

using Autofac;
using Autofac.Extras.CommonServiceLocator;

using BarCodeScanner.Core;
using BarCodeScanner.ViewModel;

using CommonServiceLocator;

using DataBase;

using NLog;

namespace BarCodeScanner
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var builder = new ContainerBuilder();

            // RegisterViewModel
            builder.RegisterType<DashboardViewModel>();
            builder.RegisterType<WorkersViewModel>();
            builder.RegisterType<LogsViewModel>();

            //Services
            builder.RegisterType<BarCodeContext>().As<IBarCodeContext>().SingleInstance();
            builder.RegisterType<DbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterInstance(LogManager.GetCurrentClassLogger()).As<ILogger>();

            var locator = new AutofacServiceLocator(builder.Build());
            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}