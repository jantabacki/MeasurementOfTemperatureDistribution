using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureServer
{
    class TelegramBuffer
    {
        private object bufferPadLock = new object();
        private int bufferSize = 1600;
        private int bufferIterator = 0;
        private byte[] buffer = new byte[1600];

        public void AddByteToBuffer(byte newByte)
        {
            lock (bufferPadLock)
            {

                if (bufferIterator < bufferSize)
                {
                    buffer[bufferIterator++] = newByte;
                }
                else
                {
                    for (int i = 0; i < bufferSize - 1; i++)
                    {
                        buffer[i] = buffer[i + 1];
                    }
                    buffer[bufferSize - 1] = newByte;
                }
            }
        }

        public void CheckIfBufferContainsTelegram(byte telegramSize)
        {
            lock (bufferPadLock)
            {
                int telegramBodySize = 23;
                int startPositionOfTelegram = 0;
                int endPositionOfTelegram = 0;
                for (int i = 0; i < bufferIterator; i++)
                {
                    if (buffer[i] == telegramSize)
                    {
                        startPositionOfTelegram = i;
                        endPositionOfTelegram = i + telegramSize - 1;
                        if (endPositionOfTelegram < bufferIterator)
                        {
                            byte checkSum = 0;
                            for (int j = startPositionOfTelegram; j < endPositionOfTelegram; j++)
                            {
                                checkSum += buffer[j];
                            }
                            if (checkSum == buffer[endPositionOfTelegram])
                            {
                                if (endPositionOfTelegram - startPositionOfTelegram - 1 <= telegramBodySize)
                                {
                                    int telegramBodyIterator = 0;
                                    byte[] telegramBody = new byte[telegramBodySize];
                                    for (int j = startPositionOfTelegram + 1; j <= endPositionOfTelegram - 1; j++)
                                    {
                                        telegramBody[telegramBodyIterator++] = buffer[j];
                                    }
                                    TelegramAnalyzer.AnalizeTelegram(telegramBody, telegramBodySize);
                                }

                                int copyIterator = 0;
                                for (int j = endPositionOfTelegram + 1; j < bufferIterator; j++)
                                {
                                    buffer[copyIterator++] = buffer[j];
                                }
                                bufferIterator -= endPositionOfTelegram + 1;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
}
