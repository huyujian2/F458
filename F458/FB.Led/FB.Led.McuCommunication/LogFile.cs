using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FB.Led.McuCommunication
{
    class LogFile
    {
        static FileStream LogOut;
        static FileStream FileOut;
        static StreamWriter myLogOut;
        static StreamWriter myFileOut;
        public static string OpenLogFile(FileMode mode)
        {
            string logpath = ".\\BojayLog.txt";
            LogOut = new FileStream(logpath, mode);
            myLogOut = new StreamWriter(LogOut);
            return "ok";
        }
        public static string WriteLogToFile(string strLog)
        {
            myLogOut.WriteLine(strLog);
            return "ok";
        }
        public static string CloseLogFile()
        {
            myLogOut.Flush();
            myLogOut.Close();
            LogOut.Close();
            return "ok";
        }
        public static string OpenFile(string path)
        {
            FileOut = new FileStream(path, FileMode.Create);
            myFileOut = new StreamWriter(LogOut);
            return "ok";
        }
        public static string WriteToFile(string str)
        {
            myFileOut.WriteLine(str);
            return "ok";
        }
        public static string CloseFile()
        {
            myFileOut.Flush();
            myFileOut.Close();
            FileOut.Close();
            return "ok";
        }
    }
}
