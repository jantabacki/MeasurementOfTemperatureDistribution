using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logger.Log(LogType.Info, "TelegramTester is starting...");
                Server server = new Server();
                server.WaitForRequest();
                Logger.Log(LogType.Error, "TelegramTester is stopped");
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, e.ToString());
            }
        }
    }
}
