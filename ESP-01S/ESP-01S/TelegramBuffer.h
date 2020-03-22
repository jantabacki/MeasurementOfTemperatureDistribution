#ifndef TelegramBuffer_h
#define TelegramBuffer_h

#define BUFFER_SIZE 1024
#define MAX_TELEGRAM_BODY_SIZE 5

#include <Arduino.h>
#include "TelegramAnalyzer.h"

class TelegramBuffer {
  private:
    static int bufferIterator;
    static byte buffer[BUFFER_SIZE];
  public:
    static void AddByteToBuffer(byte);
    static void CheckIfBufferContainsTelegram(byte);
};

#endif
