using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataBase;
using DataBase.Model;
using ExcelExport.Helper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelExport
{
    public class ExcelService
    {
        public readonly string FilePath;
        private readonly Worker[] _workers;
        private readonly WorkTimeLog[] _logs;

        private IWorkbook _workbook;
        private ISheet _excelSheet;

        private readonly ICellStyle _cellStyle;

        public ExcelService(Worker[] workers, WorkTimeLog[] logs, string filePath = "ExcelLog.xlsx")
        {
            _workers = workers;
            _logs = logs;

            FilePath = filePath;

            //Init Excel provider
            _workbook = new XSSFWorkbook();
            _excelSheet = _workbook.CreateSheet("Main");

            //set with first column
            _excelSheet.SetColumnWidth(0, 20 * 256);

            //Init style
            _cellStyle = SetDefaultStyle(_workbook.CreateCellStyle());
        }

        private static ICellStyle SetDefaultStyle(
            ICellStyle style, 
            BorderStyle border = BorderStyle.Thin)
        {
            style.BorderBottom = border;
            style.BorderLeft = border;
            style.BorderRight = border;
            style.BorderTop = border;

            style.BottomBorderColor = IndexedColors.Black.Index;
            style.LeftBorderColor = IndexedColors.Black.Index;
            style.RightBorderColor = IndexedColors.Black.Index;
            style.TopBorderColor = IndexedColors.Black.Index;

            style.Alignment = HorizontalAlignment.Right;

            return style;
        }

        public Task StartImportExcel()
        {
            return Task.Run(() =>
            {
                RenderHeaderAndWorkerRow();
                RenderLogs();
                SaveExcel(FilePath);
            });
        }

        private void RenderHeaderAndWorkerRow()
        {
            //Set Style
            ICellStyle cellStyle = _cellStyle;
            cellStyle.Alignment = HorizontalAlignment.Center;

            //Set Header
            var header = _excelSheet.CreateRow(0).CreateCell(0);
            header.SetCellValue("ФИО \\ Дата");
            header.CellStyle = cellStyle;

            //Write workers
            cellStyle.Alignment = HorizontalAlignment.Left;
            //var column = _excelSheet.CreateRow(0);
            for (int i = 0; i < _workers.Length; i++)
            {
                var column = _excelSheet.CreateRow(i + 1).CreateCell(0);
                column.CellStyle = cellStyle;
                column.SetCellValue(_workers[i].FullName);
            }
        }

        private void RenderLogs()
        {
            var sort = _logs.TakeByGroup();
            int i = 1;
            foreach (var log in sort)
            {
                WriteColumnLogs(i, log.Key, log.Value.ToList());

                _excelSheet.SetColumnWidth(i, 11 * 256);
                _excelSheet.SetColumnWidth(i + 1, 11 * 256);

                SaveExcel(FilePath);
                i += 2;
            }
        }

        private void WriteColumnLogs(int column, DateTime dateTime, List<WorkTimeLog> logs)
        {
            int firstColumn = column;
            int secondColumn = column + 1;

            // Row Style
            var firstRowStyle = _cellStyle; firstRowStyle.BorderLeft = BorderStyle.Thick;
            var secondRowStyle = _cellStyle; secondRowStyle.BorderLeft = BorderStyle.Thick;
            
            // Date Style
            var dateStyle = SetDefaultStyle(_cellStyle, BorderStyle.Thick);

            // Render Data
            var data = _excelSheet.CreateRow(0).CreateCell(secondColumn);
            data.CellStyle = dateStyle;
            data.SetCellValue(dateTime.ToLongDateString());

            //Render firstRow Log
            var firstWorkers = logs.Where(x => x.ScanType == ScanTypes.Пришел).ToArray();

            var columnidf = 1;
            foreach (var worker in _workers)
            {
                var colmn = _excelSheet.CreateRow(columnidf).CreateCell(firstColumn);
                colmn.CellStyle = firstRowStyle;
                bool isFound = false;
                foreach (var log in firstWorkers)
                {
                    if (worker.Id == log.Id)
                    {
                        colmn.SetCellValue(log.ScanTime.ToLongTimeString());
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    colmn.SetCellValue("-  ");

                columnidf++;
            }

            //Render secondRow Log
            var secondWorkers = logs.Where(x => x.ScanType == ScanTypes.Ушел).ToArray();

            var columnids = 1;
            foreach (var worker in _workers)
            {
                var colmn = _excelSheet.CreateRow(columnids).CreateCell(secondColumn);
                colmn.CellStyle = firstRowStyle;
                bool isFound = false;
                foreach (var log in secondWorkers)
                {
                    if (worker.Id == log.Id)
                    {
                        colmn.SetCellValue(log.ScanTime.ToLongTimeString());
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    colmn.SetCellValue("-  ");

                columnids++;
            }
        }

        private void SaveExcel(string filePath)
        {
            var stream = File.Open(filePath, FileMode.OpenOrCreate);
            _workbook.Write(stream);

            _workbook.Close();
            stream.Close();
        }
    }
}