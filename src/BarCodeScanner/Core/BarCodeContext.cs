﻿using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
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
                MessageBox.Show("Ошибка при поиске сканера пожалуйста переподключите сканер\r\nКод ошибки - "+ e.Message
                    ,"Error COM");
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
        public static bool IsValidGtin(string code)
        {
            if (code != (new Regex("[^0-9]")).Replace(code, ""))
            {
                // is not numeric
                return false;
            }
            // pad with zeros to lengthen to 14 digits
            switch (code.Length)
            {
                case 8:
                    code = "000000" + code;
                    break;
                case 12:
                    code = "00" + code;
                    break;
                case 13:
                    code = "0" + code;
                    break;
                case 14:
                    break;
                default:
                    // wrong number of digits
                    return false;
            }
            // calculate check digit
            int[] a = new int[13];
            a[0] = int.Parse(code[0].ToString()) * 3;
            a[1] = int.Parse(code[1].ToString());
            a[2] = int.Parse(code[2].ToString()) * 3;
            a[3] = int.Parse(code[3].ToString());
            a[4] = int.Parse(code[4].ToString()) * 3;
            a[5] = int.Parse(code[5].ToString());
            a[6] = int.Parse(code[6].ToString()) * 3;
            a[7] = int.Parse(code[7].ToString());
            a[8] = int.Parse(code[8].ToString()) * 3;
            a[9] = int.Parse(code[9].ToString());
            a[10] = int.Parse(code[10].ToString()) * 3;
            a[11] = int.Parse(code[11].ToString());
            a[12] = int.Parse(code[12].ToString()) * 3;
            int sum = a[0] + a[1] + a[2] + a[3] + a[4] + a[5] + a[6] + a[7] + a[8] + a[9] + a[10] + a[11] + a[12];
            int check = (10 - (sum % 10)) % 10; 

            int last = int.Parse(code[13].ToString());
            return check == last;
        }
    }
}
