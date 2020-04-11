using LoggerLib;
using System;
using TemperatureIndicationLib;

namespace TemperatureServer
{
    internal class TelegramAnalyzer
    {
        public static void AnalizeTelegram(byte[] inputArray, int inputArraySize)
        {
            if (inputArray[0] == 1)
            {
                telegramTypeA(inputArray);
            }
            else if (inputArray[0] == 2)
            {
                telegramTypeB(inputArray);
            }
            else
            {
                Logger.Log(LogType.Error, "Unknown telegram type -> ");
                string logString = string.Empty;
                foreach (byte singleByte in inputArray)
                {
                    logString += $"{singleByte} ";
                }
                Logger.Log(LogType.Error, logString);
            }
        }

        private static void telegramTypeA(byte[] telegramBody)
        {
            Logger.Log(LogType.Info, "Received Telegram ->");
            string logString = string.Empty;
            foreach (byte singleByte in telegramBody)
            {
                logString += $"{singleByte} ";
            }
            Logger.Log(LogType.Info, logString);
            byte[] byteTimeStamp = new byte[] { telegramBody[1], telegramBody[2], telegramBody[3], telegramBody[4] };
            uint NTPtimeStamp = BitConverter.ToUInt32(byteTimeStamp, 0);
            DateTime timeStamp = new DateTime(1900, 1, 1, 0, 0, 0).AddSeconds(NTPtimeStamp);
            int posX = 0;
            for (int i = 7; i <= 22; i += 2)
            {
                TemperatureIndication temperatureIndication = new TemperatureIndication(
                timeStamp, posX++,
                telegramBody[6],
                BitConverter.ToInt16(new byte[] {
                    telegramBody[i+1],
                    telegramBody[i] }, 0));
                TemperatureSaver.AddTemperatureIndication(temperatureIndication);
            }
        }

        private static void telegramTypeB(byte[] telegramBody)
        {

        }
    }
}