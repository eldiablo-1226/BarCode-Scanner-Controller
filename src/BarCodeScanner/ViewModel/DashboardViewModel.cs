using System;
using System.Collections.Generic;
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
    public class DashboardViewModel : ReactiveObject
    {
        #region ContextDependenci
            private readonly IDbContext _db;
            private readonly IBarCodeContext _barCode;
            private readonly ILogger _logger;
        #endregion
        public DashboardViewModel(IDbContext dbContext, IBarCodeContext barCode, ILogger logger)
        {
            _db = dbContext;
            _barCode = barCode;
            _logger = logger;

            _barCode.GetBarCodeString += GetBarCodeEvent;
        }

        [Reactive] public string BarCode { get; set; }
        [Reactive] public string FullName { get; set; }
        [Reactive] public string LogText { get; set; }

        private void GetBarCodeEvent(string code)
        {
            try
            {
                var findedWorkers = _db.Workers.FindOne(w => w.BarCode == code);
                if (findedWorkers == null) return;

                BarCode = findedWorkers.BarCode;
                FullName = findedWorkers.FullName;

                WriteLog(findedWorkers);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private void WriteLog(Worker worker)
        {
            try
            {
                WorkTimeLog lastWrite = _db.Logs.Find((log =>
                    log.WorkerBy.Id == worker.Id && log.ScanTime.Day == DateTime.Now.Day)).LastOrDefault();

                ScanTypes scanType = ScanTypes.Пришел;
                if (lastWrite != null)
                    scanType = lastWrite.ScanType == ScanTypes.Пришел ? ScanTypes.Ушел : ScanTypes.Пришел;

                var logIteam = new WorkTimeLog(worker, DateTime.Now, scanType);

                _db.Logs.Insert(logIteam);
                LogText += $"{logIteam.ScanTime} - {logIteam.WorkerBy.FullName}\r";
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
