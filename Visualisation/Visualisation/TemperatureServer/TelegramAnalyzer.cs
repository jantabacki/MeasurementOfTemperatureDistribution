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
            //Parse telegram here
            //[1] [245 21 34 226] [1] [6] [0 231] [0 234] [0 236] [0 239] [0 241] [0 244] [0 245] [0 247]
            byte[] byteTimeStamp = new byte[] { telegramBody[1], telegramBody[2], telegramBody[3], telegramBody[4] };
            uint NTPtimeStamp = BitConverter.ToUInt32(byteTimeStamp, 0);
            DateTime timeStamp = new DateTime(1900, 1, 1, 0, 0, 0).AddSeconds(NTPtimeStamp);
            int posY = 0;
            for (int i = 7; i <= 22; i += 2)
            {
                TemperatureIndication temperatureIndication = new TemperatureIndication(
                timeStamp, telegramBody[6],
                posY++,
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