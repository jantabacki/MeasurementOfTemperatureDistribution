using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLib
{
    public static class Logger
    {
        private static string loggerFileName = string.Empty;
        private static List<LogType> logLevel = null;

        static Logger()
        {
            List<LogType> defaultLogTypes = new List<LogType> {
            LogType.Error,
            LogType.Info,
            LogType.Success,
            LogType.Error
            };
            LoggerSettings(defaultLogTypes, "defaultLoggerFileName.txt");
        }

        public static void LoggerSettings(List<LogType> logLevel, string loggerFileName)
        {
            Logger.logLevel = logLevel;
            Logger.loggerFileName = loggerFileName;
        }

        public static void Log(LogType logType, string content)
        {
            string output = string.Empty;

            switch (logType)
            {
                case LogType.Info:
                    if (logLevel.Contains(LogType.Info))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        output = DateTime.Now + " [INFO] " + content;
                    }
                    break;
                case LogType.Success:
                    if (logLevel.Contains(LogType.Success))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        output = DateTime.Now + " [SUCCESS] " + content;
                    }
                    break;
                case LogType.Warning:
                    if (logLevel.Contains(LogType.Warning))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        output = DateTime.Now + " [WARNING] " + content;
                    }
                    break;
                case LogType.Error:
                    if (logLevel.Contains(LogType.Error))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        output = DateTime.Now + " [ERROR] " + content;
                    }
                    break;
                default:
                    break;
            }

            if (output != string.Empty)
            {
                Console.WriteLine(output);
                using (StreamWriter sw = File.AppendText(loggerFileName))
                {
                    sw.WriteLine(output);
                }
            }
        }
    }
}

namespace LoggerLib
{
    public enum LogType
    {
        Info, Warning, Error, Success
    }
}
