using LiteDB;

namespace BarCodeScanner.db.Model
{
    public class Worker
    {
        public ObjectId Id { get; set; }
        public string FullName { get; set; }
        public string BarCode { get; set; }

        public override string ToString() => FullName;
    }
}
