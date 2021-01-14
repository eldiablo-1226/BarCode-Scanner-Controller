using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelExport
{
    public class ExcelWritter
    {
        public readonly string FilePath;
        private readonly IDbContext _db;

        public ExcelWritter(IDbContext db, string filePath = "ExcelLog.xlsx")
        {
            FilePath = filePath;

            _db = db;

            if (!File.Exists(FilePath)) CreateFileExcel();
        }

        private void CreateFileExcel()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Main");


        }

    }
}
