using System;
using System.Collections.Generic;
using System.Linq;

using BarCodeScanner.Core;

using DataBase;
using DataBase.Model;
using ExcelExport;
using NLog;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BarCodeScanner.ViewModel
{
    public class LogsViewModel : ReactiveObject
    {
        private IDbContext _db;
        private ILogger _logger;

        public LogsViewModel(IDbContext dbContext, ILogger logger, IBarCodeContext barCode)
        {
            _db = dbContext;
            _logger = logger;

            //barCode.GetBarCodeString += s => LoadLogs();

            RemoveCommand = ReactiveCommand.Create(DeleteWorker, this.WhenAny(
                model => model.SelectedIteam,
                log => log != null));
            UpdateCommand = ReactiveCommand.Create(LoadLogs);
            ExcelImportCommand = ReactiveCommand.Create(ImportExcel);
            LoadLogs();
        }

        [Reactive] public WorkTimeLog SelectedIteam { get; set; }
        [Reactive] public IEnumerable<WorkTimeLog> LogsList { get; set; }

        [Reactive] public IReactiveCommand UpdateCommand { get; set; }
        [Reactive] public IReactiveCommand RemoveCommand { get; set; }
        [Reactive] public IReactiveCommand ExcelImportCommand { get; set; }

        private async void ImportExcel()
        {
            var service = new ExcelService(_db.Workers.FindAll().ToArray(), LogsList.ToArray());
            await service.StartImportExcel();
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