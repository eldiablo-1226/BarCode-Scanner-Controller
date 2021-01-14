using DataBase.Model;
using LiteDB;

namespace DataBase
{
    public interface IDbContext
    {
        public ILiteCollection<Worker> Workers { get; }
        public ILiteCollection<WorkTimeLog> Logs { get; }
    }
}
