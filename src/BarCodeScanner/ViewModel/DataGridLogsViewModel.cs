using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataBase;
using DataBase.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BarCodeScanner.Helps;

namespace BarCodeScanner.ViewModel
{
    public class DataGridLogsViewModel : ReactiveObject
    {
        private readonly List<Worker> _workers;
        private readonly Dictionary<DateTime, IEnumerable<WorkTimeLog>> _logs;

        [Reactive] public ObservableCollection<ObservableCollection<string>> ArrayLogs { get; set; }
        [Reactive] public ObservableCollection<string> HeaderData { get; set; }
        [Reactive] public ObservableCollection<string> DateTimeLog { get; set; }

        public DataGridLogsViewModel(IDbContext db)
        {
            _workers = db.Workers.FindAll().ToList();
            _logs = db.Logs.FindAll().ToList().TakeByGroup();

            ArrayLogs = new ObservableCollection<ObservableCollection<string>>();
            HeaderData= new ObservableCollection<string>();
            DateTimeLog = new ObservableCollection<string>();

            RenderHeaderAndWorkerRow();

            RenderLogs();
        }

        private void RenderHeaderAndWorkerRow()
        {
            //HeaderData.Add("ФИО \\ Дата");

            foreach (var worker in _workers)
            {
                HeaderData.Add(worker.FullName);
            }
            //ArrayLogs.Add(new ObservableCollection<string>(HeaderData));
        }

        private void RenderLogs()
        {
            foreach (var log in _logs)
            {
                var resault = WriteColumnLogs(log.Key, log.Value.ToList()).Reverse().ToArray();

                ArrayLogs.Add(new ObservableCollection<string>(resault[0]));
                ArrayLogs.Add(new ObservableCollection<string>(resault[1]));
            }
        }

        private IEnumerable<List<string>> WriteColumnLogs(DateTime dateTime, List<WorkTimeLog> logs)
        {
            List<string> firstColumn = new List<string>();
            List<string> secondColumn = new List<string>();

            // Render Data
            DateTimeLog.Add(dateTime.ToLongDateString());
            DateTimeLog.Add("у");

            //Render firstRow Log
            var firstWorkers = logs
                .Where(x => x.ScanType == ScanTypes.Пришел)
                .ToArray();

            foreach (var worker in _workers)
            {
                bool isFound = false;
                foreach (var log in firstWorkers)
                {
                    if (worker.Id == log.WorkerBy.Id)
                    {
                        firstColumn.Add(log.ScanTime.ToLongTimeString());
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    firstColumn.Add("-");
            }

            //Render secondRow Log
            var secondWorkers = logs
                .Where(x => x.ScanType == ScanTypes.Ушел)
                .ToArray();

            foreach (var worker in _workers)
            {
                bool isFound = false;
                foreach (var log in secondWorkers)
                {
                    if (worker.Id == log.WorkerBy.Id)
                    {
                        secondColumn.Add(log.ScanTime.ToLongTimeString());
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    secondColumn.Add("-");
            }

            return new List<List<string>>{secondColumn, firstColumn};
        }

    }
}
