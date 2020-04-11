#include "StaticDisplay.h"

LiquidCrystal595 StaticDisplay::lcd(LCD_PIN_A, LCD_PIN_B, LCD_PIN_C);
bool StaticDisplay::wasDisplayInitialized = false;

void StaticDisplay::InitDisplay(int x, int y) {
  noInterrupts();
  if (!wasDisplayInitialized) {
    lcd.begin(x, y);
    wasDisplayInitialized = true;
  }
  interrupts();
}

void StaticDisplay::WriteToDisplay(int posX, int posY, char value)
{
  noInterrupts();
  lcd.setCursor(posX, posY);
  lcd.print(value);
  interrupts();
}

void StaticDisplay::WriteToDisplay(int posX, int posY, String value)
{
  noInterrupts();
  lcd.setCursor(posX, posY);
  for (int i = 0; i < value.length(); i++) {
    lcd.print(value[i]);
  }
  interrupts();
}
