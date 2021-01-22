using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarCodeScanner.Core;
using BarCodeScanner.View;
using DataBase;
using DataBase.Model;
using NLog;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BarCodeScanner.ViewModel
{
    public class LogsViewModel : ReactiveObject
    {
        private readonly IDbContext _db;
        private readonly ILogger _logger;

        public LogsViewModel(IDbContext dbContext, ILogger logger)
        {
            _db = dbContext;
            _logger = logger;

            //barCode.GetBarCodeString += s => LoadLogs();

            RemoveCommand = ReactiveCommand.Create(DeleteWorker, this.WhenAny(
                model => model.SelectedIteam,
                log => log != null));
            UpdateCommand = ReactiveCommand.Create(LoadLogs);
            ExcelImportCommand = ReactiveCommand.Create(ImportExcel);
            ClearCommand = ReactiveCommand.Create(() => { _db.Logs.DeleteAll(); LoadLogs(); });
            LoadLogs();
        }

        [Reactive] public WorkTimeLog SelectedIteam { get; set; }
        [Reactive] public IEnumerable<WorkTimeLog> LogsList { get; set; }

        [Reactive] public IReactiveCommand UpdateCommand { get; set; }
        [Reactive] public IReactiveCommand RemoveCommand { get; set; }
        [Reactive] public IReactiveCommand ExcelImportCommand { get; set; }
        [Reactive] public IReactiveCommand ClearCommand { get; set; }


        private void ImportExcel()
        {
            try
            {
                var data = new DataGridLogs();
                data.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void DeleteWorker()
        {
            try
            {
                _db.Workers.Delete(SelectedIteam.Id);
                LoadLogs();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private void LoadLogs() =>
            LogsList = _db.Logs.FindAll().Reverse();
    }
}