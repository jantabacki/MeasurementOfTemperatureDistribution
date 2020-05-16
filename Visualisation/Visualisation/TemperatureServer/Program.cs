using LoggerLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TemperatureServer
{
    class Program
    {
        static void Main()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                List<LogType> logTypes = new List<LogType>();
                if (appSettings["LogInfo"].Equals("true"))
                {
                    logTypes.Add(LogType.Info);
                }
                if (appSettings["LogSuccess"].Equals("true"))
                {
                    logTypes.Add(LogType.Success);
                }
                if (appSettings["LogWarning"].Equals("true"))
                {
                    logTypes.Add(LogType.Warning);
                }
                if (appSettings["LogError"].Equals("true"))
                {
                    logTypes.Add(LogType.Error);
                }
                Logger.LoggerSettings(logTypes, appSettings["LogFileName"]);
                Logger.Log(LogType.Info, "TemperatureServer is starting...");
                Server server = new Server();
                Thread serverThread = new Thread(() =>
                {
                    server.WaitForRequest();
                });
                serverThread.Start();
                while (!Console.ReadLine().Equals("exit"))
                {

                }
                serverThread.Abort();
                server.CheckTelegramBuffer.Abort();
                Logger.Log(LogType.Warning, "TemperatureServer is stopped");
                Logger.Log(LogType.Info, "Saving measured temperatures");
                TemperatureSaver.SerializeMeasuredTemperatures(appSettings["TemperatureServerSaveFileName"]);
                Logger.Log(LogType.Info, "Saving has been completed");
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, e.ToString());
            }
        }
    }
}
