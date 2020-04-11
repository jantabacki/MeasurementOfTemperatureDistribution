#ifndef TelegramAnalyzer_h
#define TelegramAnalyzer_h

#include <Arduino.h>
#include "StaticDisplay.h"

class TelegramAnalyzer {
  private:
    static void telegramTypeA(byte[], int);
    static void telegramTypeB(byte[], int);
  public:
    static void AnalyzeTelegram(byte[], int);
};

#endif
