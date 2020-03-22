#ifndef TelegramAnalyzer_h
#define TelegramAnalyzer_h

#define HOST_ADDRESS "192.168.0.11"
#define HOST_PORT 1989

#define VISUALISATION_DATA_SIZE 200

#include <Arduino.h>
#include <ESP8266WiFi.h>
#include "InternalTime.h"

class TelegramAnalyzer {
  private:
    static int visualisationDataPacketIterator;
    static byte visualisationDataPacket[VISUALISATION_DATA_SIZE];
    static void telegramTypeA(byte[], int);
    static void telegramTypeB(byte[], int);
  public:
    static void AnalyzeTelegram(byte[], int);
};

#endif
