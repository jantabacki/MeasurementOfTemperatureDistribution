#ifndef StaticDisplay_h
#define StaticDisplay_h

#define LCD_PIN_A 10
#define LCD_PIN_B 11
#define LCD_PIN_C 12

#include <Arduino.h>
#include <LiquidCrystal595.h>

class StaticDisplay {
  private:
    static bool wasDisplayInitialized;
    static LiquidCrystal595 lcd;
  public:
    static void InitDisplay(int, int);
    static void WriteToDisplay(int, int, char);
    static void WriteToDisplay(int, int, String);
};

#endif
