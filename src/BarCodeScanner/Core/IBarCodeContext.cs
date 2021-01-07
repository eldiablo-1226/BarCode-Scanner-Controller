using System;

namespace BarCodeScanner.Core
{
    public interface IBarCodeContext
    {
        public string ComPort { get; set; }
        public Action<string> GetBarCodeString { get; set; }
    }
}
