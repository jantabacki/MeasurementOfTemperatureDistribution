#ifndef TelegramAnalyzer_h
#define TelegramAnalyzer_h

#define LCD_PIN_A 10
#define LCD_PIN_B 11
#define LCD_PIN_C 12

#include <Arduino.h>
#include <LiquidCrystal595.h>

class TelegramAnalyzer {
  private:
    static void telegramTypeA(byte[], int);
    static void telegramTypeB(byte[], int);
    static bool wasDisplayInitialized;
    static void initDisplay(int, int);
  public:
    static LiquidCrystal595 lcd;
    static void AnalyzeTelegram(byte[], int);
};

#endif
