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

        private readonly IWorkbook _workbook;
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

            //set with first row
            _excelSheet.SetColumnWidth(0, 20 * 256);

            //Init style
            _cellStyle = SetDefaultStyle(_workbook.CreateCellStyle());
        }

        private ICellStyle SetDefaultStyle(
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
            for (int i = 0; i < _workers.Length; i++)
            {
                var column = _excelSheet.CreateRow(0).CreateCell(i + 1);
                column.CellStyle = cellStyle;
                column.SetCellValue(_workers[i].FullName);
            }
        }

        private void RenderLogs()
        {
            var sort = _logs
                .GroupBy(x => x.ScanTime.Date)
                .TakeByGroup(2);
            int i = 1;
            foreach (var log in sort)
            {
                WriteColumnLogs(i, log.Key, log.Value.ToList());
                i += 2;
            }
        }

        private void WriteColumnLogs(int row, DateTime dateTime, List<WorkTimeLog> logs)
        {
            var firstRow = _excelSheet.CreateRow(row);
            var secondRow = _excelSheet.CreateRow(row + 1);

            // Row Style
            var firstRowStyle = _cellStyle; firstRowStyle.BorderLeft = BorderStyle.Thick;
            var secondRowStyle = _cellStyle; secondRowStyle.BorderLeft = BorderStyle.Thick;
            
            // Date Style
            var dateStyle = SetDefaultStyle(_cellStyle, BorderStyle.Thick);

            // Render Data
            var data = secondRow.CreateCell(0);
            data.CellStyle = dateStyle;
            data.SetCellValue(dateTime.ToShortDateString());

            //Render firstRow Log
            var firstWorkers = logs.Where(x => x.ScanType == ScanTypes.Пришел);

            var columnidf = 1;
            foreach (var worker in _workers)
            {
                foreach (var log in firstWorkers)
                {
                    if (worker.Id == log.Id)
                    {
                        var colmn = firstRow.CreateCell(columnidf);
                        colmn.CellStyle = firstRowStyle;
                        colmn.SetCellValue(log.ScanTime.ToShortTimeString());
                        break;
                    }
                }

                columnidf++;
            }

            //Render secondRow Log
            var secondWorkers = logs.Where(x => x.ScanType == ScanTypes.Ушел);

            var columnids = 1;
            foreach (var worker in _workers)
            {
                foreach (var log in secondWorkers)
                {
                    if (worker.Id == log.Id)
                    {
                        var colmn = firstRow.CreateCell(columnids);
                        colmn.CellStyle = firstRowStyle;
                        colmn.SetCellValue(log.ScanTime.ToShortTimeString());
                        break;
                    }
                }

                columnids++;
            }
        }

        private void SaveExcel(string filePath)
        {
            var stream = File.Create(filePath);
            _workbook.Write(stream);

            _workbook.Close();
            stream.Close();
        }
    }
}