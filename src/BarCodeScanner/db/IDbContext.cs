using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BarCodeScanner.db.Model;

using LiteDB;

namespace BarCodeScanner.db
{
    public interface IDbContext
    {
        public ILiteCollection<Worker> Workers { get; }
        public ILiteCollection<WorkTimeLog> Logs { get; }
    }
}
