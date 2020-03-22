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
int TelegramAnalyzer::visualisationDataPacketIterator = 0;
byte TelegramAnalyzer::visualisationDataPacket[1024];
void TelegramAnalyzer::telegramTypeA(byte telegramBody[], int inputArraySize)
{
  int telegramBodyIterator = 0;
  for (int i = visualisationDataPacketIterator; i < visualisationDataPacketIterator + inputArraySize; i++)
  {
    visualisationDataPacket[i] = telegramBody[telegramBodyIterator++];
  }
  visualisationDataPacketIterator += inputArraySize;
  if (visualisationDataPacketIterator >= 1024)
  {
    WiFiClient client;
    if (!client.connect(HOST_ADDRESS, HOST_PORT)) {
    }
    client.write(visualisationDataPacket, 1024);
    visualisationDataPacketIterator = 0;
  }
}

void TelegramAnalyzer::telegramTypeB(byte telegramBody[], int inputArraySize)
{
  //logic for telegram Type B
}
