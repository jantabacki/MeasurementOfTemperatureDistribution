#include "TelegramAnalyzer.h"

LiquidCrystal595 TelegramAnalyzer::lcd(LCD_PIN_A, LCD_PIN_B, LCD_PIN_C);
bool TelegramAnalyzer::wasDisplayInitialized = false;

void TelegramAnalyzer::initDisplay(int x, int y) {
  if (!wasDisplayInitialized) {
    lcd.begin(x, y);
    wasDisplayInitialized = true;
  }
}

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
  initDisplay(16, 2);
  int displayPosX = 0;
  int displayPosY = 0;
  for (int i = 1; i < inputArraySize; i++) {
    lcd.setCursor(displayPosX, displayPosY);
    if (telegramBody[i] != 0) {
      lcd.print((char)telegramBody[i]);
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
