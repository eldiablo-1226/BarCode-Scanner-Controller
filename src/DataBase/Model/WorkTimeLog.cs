using System;
using LiteDB;

namespace DataBase.Model
{
    public class WorkTimeLog
    {
        public WorkTimeLog() {}
        public WorkTimeLog(Worker workerBy, DateTime scanTime, ScanTypes scanType)
        {
            WorkerBy = workerBy;
            ScanTime = scanTime;
            ScanType = scanType;
        }

        public ObjectId Id { get; set; }
        public Worker WorkerBy { get; set; }
        public DateTime ScanTime { get; set; }
        public ScanTypes ScanType { get; set; }
    }

    public enum ScanTypes
    {
        Пришел,
        Ушел
    }
}
