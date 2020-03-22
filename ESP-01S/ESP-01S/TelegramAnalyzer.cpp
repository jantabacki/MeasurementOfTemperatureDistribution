#include "TelegramAnalyzer.h"

void TelegramAnalyzer::AnalyzeTelegram(byte inputArray[], int inputArraySize)
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
    //logic for unknown telegram
  }
}

void TelegramAnalyzer::telegramTypeA(byte telegramBody[])
{
  //logic for telegram Type A
}

void TelegramAnalyzer::telegramTypeB(byte telegramBody[])
{
  //logic for telegram Type B
}
