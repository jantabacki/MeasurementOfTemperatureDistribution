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
byte TelegramAnalyzer::visualisationDataPacket[VISUALISATION_DATA_SIZE];
void TelegramAnalyzer::telegramTypeA(byte telegramBody[], int inputArraySize)
{
  int startVisualisationDataPacketIterator = visualisationDataPacketIterator;
  unsigned long currentTime = InternalTime::GetTotalSeconds();
  visualisationDataPacket[visualisationDataPacketIterator++] = 25;
  visualisationDataPacket[visualisationDataPacketIterator++] = 1;
  visualisationDataPacket[visualisationDataPacketIterator++] = currentTime & 0xFF;
  visualisationDataPacket[visualisationDataPacketIterator++] = (currentTime >> 8) & 0xFF;
  visualisationDataPacket[visualisationDataPacketIterator++] = (currentTime >> 16) & 0xFF;
  visualisationDataPacket[visualisationDataPacketIterator++] = (currentTime >> 24) & 0xFF;
  int telegramBodyIterator = 0;
  for (int i = visualisationDataPacketIterator; i < visualisationDataPacketIterator + inputArraySize; i++)
  {
    visualisationDataPacket[i] = telegramBody[telegramBodyIterator++];
  }
  visualisationDataPacketIterator += inputArraySize;
  byte checkSum = 0;
  for (int i = startVisualisationDataPacketIterator; i < visualisationDataPacketIterator; i++)
  {
    checkSum += visualisationDataPacket[i];
  }
  visualisationDataPacket[visualisationDataPacketIterator++] = checkSum;
  if (visualisationDataPacketIterator >= VISUALISATION_DATA_SIZE)
  {
    WiFiClient client;
    if (!client.connect(HOST_ADDRESS, HOST_PORT)) {
    }
    client.write(visualisationDataPacket, VISUALISATION_DATA_SIZE);
    visualisationDataPacketIterator = 0;
  }
}

void TelegramAnalyzer::telegramTypeB(byte telegramBody[], int inputArraySize)
{
  //logic for telegram Type B
}
