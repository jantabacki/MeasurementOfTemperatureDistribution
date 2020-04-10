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

//Display telegram received
void TelegramAnalyzer::telegramTypeA(byte telegramBody[], int inputArraySize)
{
  int displayPosX = 0;
  int displayPosY = 0;
  for (int i = 1; i < inputArraySize; i++) {
    if (telegramBody[i] != 0) {
      StaticDisplay::WriteToDisplay(displayPosX, displayPosY, (char)telegramBody[i]);
      delay(1);
    }
    displayPosX++;
    if (displayPosX >= 16) {
      displayPosX = 0;
      displayPosY++;
    }
  }
}

void TelegramAnalyzer::telegramTypeB(byte telegramBody[], int inputArraySize)
{
  //logic for telegram Type B
}
