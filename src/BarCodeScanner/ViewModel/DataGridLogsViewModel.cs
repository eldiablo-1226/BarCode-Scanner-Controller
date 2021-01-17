using System;
using System.Collections.Generic;
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

        [Reactive] public string[,] ArrayLogs { get; set; }

        public DataGridLogsViewModel(IDbContext db)
        {
            _workers = db.Workers.FindAll().ToList();
            _logs = db.Logs.FindAll().ToList().TakeByGroup();

            var columnCount = 1 + (_logs.Count * 2);
            var rowCount = 1 + _workers.Count;

            ArrayLogs = new string[columnCount, rowCount];

            RenderHeaderAndWorkerRow();
            
            Task.Run(() =>
            {
                try
                {
                    RenderLogs();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private void RenderHeaderAndWorkerRow()
        {
            ArrayLogs[0,0] = "ФИО \\ Дата";

            for (int i = 0; i < _workers.Count; i++)
            {
                ArrayLogs[0, i + 1] = _workers[i].FullName;
            }
        }

        private void RenderLogs()
        {
            int i = 1;
            foreach (var log in _logs)
            {
                WriteColumnLogs(i, log.Key, log.Value.ToList());
                i += 2;
            }
        }

        private void WriteColumnLogs(int column, DateTime dateTime, List<WorkTimeLog> logs)
        {
            try
            {
                int firstColumn = column;
                int secondColumn = column + 1;

                // Render Data
                ArrayLogs[secondColumn, 0] = dateTime.ToLongDateString();

                //Render firstRow Log
                var firstWorkers = logs.Where(x => x.ScanType == ScanTypes.Пришел).ToArray();

                var columnidf = 1;
                foreach (var worker in _workers)
                {
                    bool isFound = false;
                    foreach (var log in firstWorkers)
                    {
                        if (worker.Id == log.Id)
                        {
                            ArrayLogs[firstColumn, columnidf] = log.ScanTime.ToLongTimeString();
                            isFound = true;
                            break;
                        }
                    }
                    if (!isFound)
                        ArrayLogs[firstColumn, columnidf] = "-";

                    columnidf++;
                }

                //Render secondRow Log
                var secondWorkers = logs.Where(x => x.ScanType == ScanTypes.Ушел).ToArray();

                var columnids = 1;
                foreach (var worker in _workers)
                {
                    bool isFound = false;
                    foreach (var log in secondWorkers)
                    {
                        if (worker.Id == log.Id)
                        {
                            ArrayLogs[secondColumn, columnids] = log.ScanTime.ToLongTimeString();
                            isFound = true;
                            break;
                        }
                    }
                    if (!isFound)
                        ArrayLogs[secondColumn, columnids] = "-";

                    columnids++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
