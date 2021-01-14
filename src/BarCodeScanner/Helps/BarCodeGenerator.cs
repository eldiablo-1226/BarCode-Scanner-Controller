using System;

namespace BarCodeScanner.Helps
{
    public static class BarCodeGenerator
    {
        //public static int Startbarcode = "5049661";
        private static Random _rd = new Random();

        private static readonly string CountryCode = "50";
        private static readonly string ManufacturerCode = "49561";

        public static string GenerateBarCode()
        {
            string workerCode = _rd.Next(10000, 99999).ToString();

            string sTemp = CountryCode + ManufacturerCode + workerCode;
            int iSum = 0;
            int iDigit;

            // Calculate the checksum digit here.
            for (int i = sTemp.Length; i >= 1; i--)
            {
                iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                if (i % 2 == 0)
                {   // odd
                    iSum += iDigit * 3;
                }
                else
                {   // even
                    iSum += iDigit * 1;
                }
            }

            string iCheckSum = ((10 - iSum % 10) % 10).ToString();

            return sTemp + iCheckSum;
        }
    }
}