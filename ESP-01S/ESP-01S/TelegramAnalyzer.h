#ifndef TelegramAnalyzer_h
#define TelegramAnalyzer_h

#define HOST_ADDRESS "192.168.0.11"
#define HOST_PORT 1989

#include <Arduino.h>
#include <ESP8266WiFi.h>

class TelegramAnalyzer {
  private:
    static void telegramTypeA(byte[], int);
    static void telegramTypeB(byte[], int);
  public:
    static void AnalyzeTelegram(byte[], int);
};

#endif
