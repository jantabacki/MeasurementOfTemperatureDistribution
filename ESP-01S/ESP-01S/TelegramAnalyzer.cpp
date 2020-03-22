#include "TelegramAnalyzer.h"

void TelegramAnalyzer::AnalyzeTelegram(byte inputArray[], int inputArraySize)
{
  if (inputArray[0] == 1)
  {
    telegramTypeA(inputArray, inputArraySize);
  }
  else if (inputArray[0] == 2)
  {
    telegramTypeB(inputArray, inputArraySize);
  }
  else
  {
    //logic for unknown telegram
  }
}

//Temperature telegram received
void TelegramAnalyzer::telegramTypeA(byte telegramBody[], int inputArraySize)
{
  WiFiClient client;
  if (!client.connect(HOST_ADDRESS, HOST_PORT)) {
  }
  client.write(telegramBody, inputArraySize);
}

void TelegramAnalyzer::telegramTypeB(byte telegramBody[], int inputArraySize)
{
  //logic for telegram Type B
}
