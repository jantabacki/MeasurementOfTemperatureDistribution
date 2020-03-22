#ifndef InternalTime_h
#define InternalTime_h

#include "Arduino.h"

class InternalTime {
  private:
    static unsigned long previousMillis;
    static unsigned long currentMillis;
    static unsigned long totalSeconds;
    static int hour;
    static int minute;
    static int second;
    static void internalSynchronization();
    static void updateTimeVariables();
  public:
    static void ExternalSynchronization(unsigned long, unsigned long, unsigned long);
    static int GetHour();
    static int GetMinute();
    static int GetSecond();
    static unsigned long GetTotalSeconds();
};

#endif
