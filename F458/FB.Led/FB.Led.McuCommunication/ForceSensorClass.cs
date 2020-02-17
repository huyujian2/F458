using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace FB.Led.McuCommunication
{
    class ForceSensorClass
    {
        static SerialPort Forceserial = new SerialPort("COM10", 19200, Parity.None, 8, StopBits.Two) { NewLine = "\r\n" };
        static byte[] byteAll = new byte[] { 0x01, 0x03, 0x9C, 0x48, 0x00, 0x08, 0xEA, 0x4A };
        static byte[] bufferAll = new byte[21];
        public static double CoverForceOne = 0;
        public static double CoverForceTwo = 0;
        public static bool OpenForceSensorPort()
        {
            Forceserial.Open();
            if (!Forceserial.IsOpen)
            {
                LogFile.WriteLogToFile("Open force sensor fail.");
                return false;
            }
            return true;
        }
        public static bool GetSensorData()
        {
            Forceserial.Write(byteAll, 0, 8);
            Thread.Sleep(60);
            Forceserial.Read(bufferAll, 0, 21);
            List<byte> Temp = new List<byte>();
            Temp.AddRange(bufferAll);
            Temp.AddRange(bufferAll);
            byte[] newbufferAll = new byte[Temp.Count];
            Temp.CopyTo(newbufferAll);
            int Index = 0;
            for (int i = 0; i <= 38; i++)
            {
                if (Convert.ToInt32(newbufferAll[i]) == 1 && Convert.ToInt32(newbufferAll[i + 1]) == 3 && Convert.ToInt32(newbufferAll[i + 2]) == 16)
                {
                    Index = i;
                    break;
                }
            }
            StringBuilder strvalue3 = new StringBuilder();
            StringBuilder strvalue4 = new StringBuilder();
            for (int t = 11 + Index; t <= 14 + Index; t++)
            {
                int intnum = Convert.ToInt32(newbufferAll[t]);
                if (intnum < 10)
                {
                    strvalue3.Append("0" + intnum.ToString("x"));
                }
                else
                {
                    strvalue3.Append(intnum.ToString("x"));
                }
            }
            for (int t = 15 + Index; t <= 18 + Index; t++)
            {
                int intnum = Convert.ToInt32(newbufferAll[t]);
                if (intnum < 10)
                {
                    strvalue4.Append("0" + intnum.ToString("x"));
                }
                else
                {
                    strvalue4.Append(intnum.ToString("x"));
                }
            }
            string g1 = "" + strvalue3;
            string g2 = "" + strvalue4;
            int p1 = 0;
            int p2 = 0;
            if (g1 != "")
            {
                p1 = int.Parse(g1, System.Globalization.NumberStyles.HexNumber);

            }
            if (g2 != "")
            {
                p2 = int.Parse(g2, System.Globalization.NumberStyles.HexNumber);

            }
            CoverForceOne = p1 / 100.0;
            CoverForceTwo = p2 / 100.0;
            return true;
        }
        public static bool CloseForceSensorPort()
        {
            Forceserial.Close();
            return true;
        }
    }
}
