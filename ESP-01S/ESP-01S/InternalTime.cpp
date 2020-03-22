#include "InternalTime.h"

unsigned long InternalTime::previousMillis = 0;
unsigned long InternalTime::currentMillis = 0;
unsigned long InternalTime::totalSeconds = 0;
int InternalTime::hour = 0;
int InternalTime::minute = 0;
int InternalTime::second = 0;

void InternalTime::internalSynchronization() {
  currentMillis = millis();
  unsigned long timeSpan = currentMillis - previousMillis;
  previousMillis = currentMillis;
  unsigned long secondToAdd = timeSpan / 1000;
  unsigned long millisToSubstract = timeSpan % 1000;
  previousMillis -= millisToSubstract;
  totalSeconds += secondToAdd;
  updateTimeVariables();
}

void InternalTime::updateTimeVariables(){
  unsigned long restOfSeconds = totalSeconds % 3600;
  unsigned long fullHoursInSeconds = totalSeconds - restOfSeconds;
  hour = (fullHoursInSeconds % 86400) / 3600;
  unsigned long restOfSecondsToDisplay = restOfSeconds % 60;
  unsigned long fullMinutesInSeconds = restOfSeconds - restOfSecondsToDisplay;
  minute = fullMinutesInSeconds / 60;
  second = restOfSecondsToDisplay % 60;
}

unsigned long InternalTime::GetTotalSeconds(){
  internalSynchronization();
  return totalSeconds;  
}

void InternalTime::ExternalSynchronization(unsigned long externalHour, unsigned long externalMinute, unsigned long externalSecond) {
  totalSeconds = externalSecond + externalMinute * 60 + externalHour * 60 * 60;
  previousMillis = millis();
}

int InternalTime::GetHour() {
  internalSynchronization();
  return hour;
}

int InternalTime::GetMinute() {
  internalSynchronization();
  return minute;
}

int InternalTime::GetSecond() {
  internalSynchronization();
  return second;
}
