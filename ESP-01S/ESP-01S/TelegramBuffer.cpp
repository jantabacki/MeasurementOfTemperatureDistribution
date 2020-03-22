#include "TelegramBuffer.h"

int TelegramBuffer::bufferIterator = 0;
byte TelegramBuffer::buffer[BUFFER_SIZE];

void TelegramBuffer::AddByteToBuffer(byte newByte)
{
  if (bufferIterator < BUFFER_SIZE)
  {
    buffer[bufferIterator++] = newByte;
  }
  else
  {
    for (int i = 0; i < BUFFER_SIZE - 1; i++)
    {
      buffer[i] = buffer[i + 1];
    }
    buffer[BUFFER_SIZE - 1] = newByte;
  }
}

void TelegramBuffer::CheckIfBufferContainsTelegram(byte telegramSize)
{
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
          if (endPositionOfTelegram - startPositionOfTelegram - 1 <= MAX_TELEGRAM_BODY_SIZE)
          {
            int telegramBodyIterator = 0;
            byte telegramBody[MAX_TELEGRAM_BODY_SIZE];
            for (int j = startPositionOfTelegram + 1; j <= endPositionOfTelegram - 1; j++)
            {
              telegramBody[telegramBodyIterator++] = buffer[j];
            }
            TelegramAnalyzer::AnalyzeTelegram(telegramBody, MAX_TELEGRAM_BODY_SIZE);
          }
          for (int j = 0; j < bufferIterator - endPositionOfTelegram; j++)
          {
            for (int k = endPositionOfTelegram + 1; k < bufferIterator; k++)
            {
              buffer[j] = buffer[k];
            }
          }
          bufferIterator -= endPositionOfTelegram + 1;
        }
      }
    }
  }
}
