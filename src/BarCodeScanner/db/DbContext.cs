using BarCodeScanner.db.Model;
using LiteDB;

namespace BarCodeScanner.db
{
    public class DbContext : IDbContext
    {
        private readonly LiteDatabase Db;
        private readonly ILiteCollection<Worker> _workers;
        private readonly ILiteCollection<WorkTimeLog> _logs;

        public ILiteCollection<Worker> Workers => _workers;
        public ILiteCollection<WorkTimeLog> Logs => _logs;

        public DbContext()
        {
            Db = new LiteDatabase("Workers.db");
            _workers = Db.GetCollection<Worker>();
            _logs = Db.GetCollection<WorkTimeLog>();
        }

        ~ DbContext()
        {
            Db.Dispose();
        }
    }
}
