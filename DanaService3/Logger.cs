using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

namespace DanaService3
{
    public static class Logger
    {
        public static void AddToLog(string s)
        {
            string sLogFile;
            sLogFile = ConfigurationManager.AppSettings["LogFile"];
            string sNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
            File.AppendText(sNow + s + "\n");
        }
    }
}