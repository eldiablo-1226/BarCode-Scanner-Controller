﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarCodeScanner.Core;
using BarCodeScanner.db;
using BarCodeScanner.db.Model;
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

            barCode.GetBarCodeString += async s => { await Task.Delay(500); LoadLogs(); };

            RemoveCommand = ReactiveCommand.Create(DeleteWorker, this.WhenAny(
                model => model.SelectedIteam,
                log => log != null));

            LoadLogs();
        }
        [Reactive] public WorkTimeLog SelectedIteam { get; set; }
        [Reactive] public ObservableCollection<WorkTimeLog> LogsList { get; set; }
        [Reactive] public IReactiveCommand RemoveCommand { get; set; }


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
            LogsList = new ObservableCollection<WorkTimeLog>(_db.Logs.FindAll().Reverse());

    }
}