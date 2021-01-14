using DataBase.Model;
using LiteDB;

namespace DataBase
{
    public class DbContext : IDbContext
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<Worker> _workers;
        private readonly ILiteCollection<WorkTimeLog> _logs;

        public ILiteCollection<Worker> Workers => _workers;
        public ILiteCollection<WorkTimeLog> Logs => _logs;

        public DbContext()
        {
            _db = new LiteDatabase("Workers.db");
            _workers = _db.GetCollection<Worker>();
            _logs = _db.GetCollection<WorkTimeLog>();
        }

        ~ DbContext()
        {
            _db.Dispose();
        }
    }
}
