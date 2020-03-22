#ifndef TelegramAnalyzer_h
#define TelegramAnalyzer_h

#include <Arduino.h>

class TelegramAnalyzer {
  private:
    static void telegramTypeA(byte[]);
    static void telegramTypeB(byte[]);
  public:
    static void AnalyzeTelegram(byte[], int);
};

#endif
