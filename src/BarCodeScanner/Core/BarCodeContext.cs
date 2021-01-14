using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;

#pragma warning disable 649

namespace BarCodeScanner.Core
{
    public class BarCodeContext : IBarCodeContext
    {
        private string _barCoteBufer = "";
        private readonly SerialPort SerialPort;

        private string _comPort = SerialPort.GetPortNames().LastOrDefault() ?? "";
        public string ComPort { get => _comPort; set => _comPort = value; }

        private Action<string> _getBarCodeEvent;
        public Action<string> GetBarCodeString { get => _getBarCodeEvent; set => _getBarCodeEvent += value; }

        public BarCodeContext()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_comPort))
                    throw new ArgumentException("Нет доступных портов для сканирование (не подключен сканер)");

                SerialPort = new SerialPort
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    RtsEnable = true,
                    PortName = _comPort
                };
                SerialPort.DataReceived += DataReceivedHandler;

                SerialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при поиске сканера пожалуйста переподключите сканер\r\nКод ошибки - " + e.Message
                    , "Error COM");
                throw;
            }
        }

        ~BarCodeContext()
        {
            if (SerialPort != null && SerialPort.IsOpen)
                SerialPort.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string barCodeString = sp.ReadExisting();
            if (barCodeString == "\r")
            {
                _getBarCodeEvent?.Invoke(_barCoteBufer);
                _barCoteBufer = string.Empty;
            }
            else if (barCodeString.Length > 1 && barCodeString.Contains("\r"))
            {
                barCodeString = barCodeString.Trim('\r');
                _getBarCodeEvent?.Invoke(barCodeString);
                _barCoteBufer = string.Empty;
            }
            else
            {
                _barCoteBufer = _barCoteBufer.Insert(_barCoteBufer.Length, barCodeString);
            }
        }
    }
}