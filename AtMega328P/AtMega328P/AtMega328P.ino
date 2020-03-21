#include <LiquidCrystal595.h>
#include <InterruptTimer.h>

#define SHIFT_REGISTER_ENABLE_PIN 9
#define MULTIPLEXER_ENABLE_PIN 5
#define TERMISTOR_MATRIX_OUTPUT_ANALOG_PIN A0
#define TERMISTOR_MATRIX_OUTPUT_ADDRESS_A 2
#define TERMISTOR_MATRIX_OUTPUT_ADDRESS_B 3
#define TERMISTOR_MATRIX_OUTPUT_ADDRESS_C 4
#define SHIFT_REGISTER_PIN_A 6
#define SHIFT_REGISTER_PIN_B 7
#define SHIFT_REGISTER_PIN_C 8
#define SIGNALING_LED 13
#define LCD_PIN_A 12
#define LCD_PIN_B 11
#define LCD_PIN_C 10
#define SERIAL_BAUD_RATE 9600
#define MAX_RECEIVED_BUFFER_SIZE 256

LiquidCrystal595 lcd(LCD_PIN_A, LCD_PIN_B, LCD_PIN_C);

void writeToDisplay(int posX, int posY, char character) {
  lcd.setCursor(posX, posY);
  lcd.print(character);
  delay(1);
}

int getValueFromTermistor(byte posX, byte posY) {
  digitalWrite(SHIFT_REGISTER_PIN_C, LOW);
  byte posYtoShift = 1;
  if (posY != 0) {
    posYtoShift << posY;
  }
  shiftOut(SHIFT_REGISTER_PIN_A, SHIFT_REGISTER_PIN_B, MSBFIRST, posYtoShift << posY);
  digitalWrite(SHIFT_REGISTER_PIN_C, HIGH);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_A, posX & 0x01);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_B, (posX >> 1) & 0x01);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_C, (posX >> 2) & 0x01);
  return analogRead(TERMISTOR_MATRIX_OUTPUT_ANALOG_PIN);
}

bool LEDHeartBeatValue = false;
void writeHeartBeatToLED() {
  if (LEDHeartBeatValue) {
    digitalWrite(SIGNALING_LED, HIGH);
  } else {
    digitalWrite(SIGNALING_LED, LOW);
  }
  LEDHeartBeatValue = !LEDHeartBeatValue;
}

bool displayHreartBeatValue = false;
void writeHeartBeatToDisplay() {
  if (displayHreartBeatValue) {
    writeToDisplay(15, 0, '+');
  } else {
    writeToDisplay(15, 0, 'X');
  }
  displayHreartBeatValue = !displayHreartBeatValue;
}

byte receivedBufferIterator = 0;
byte receivedBuffer[MAX_RECEIVED_BUFFER_SIZE];
void checkIfTelegramIsAvailableToReceive() {
  if (Serial.available()) {
    receivedBuffer[receivedBufferIterator++] = Serial.read();
    if (checkIfDisplayTelegram(receivedBuffer, receivedBufferIterator)) {
      receivedBufferIterator = 0;
    } else {
      writeToDisplay(0, 0, 'E');
    }
    if (receivedBufferIterator == MAX_RECEIVED_BUFFER_SIZE) {
      receivedBufferIterator = 0;
    }
  }
}

bool checkIfDisplayTelegram(byte *inputArray, byte inputArrayInterator) {
  if (inputArray[0] == inputArrayInterator) {
    if (inputArray[1] == 1) {
      byte checkSum = 0;
      for (int i = 0; i < inputArrayInterator - 1; i++) {
        checkSum += inputArray[i];
      }
      if (inputArray[inputArrayInterator - 1] == checkSum) {
        int displayIteratorX = 0;
        int displayIteratorY = 0;
        for (int i = 2; i <= inputArrayInterator - 2; i++) {
          if (inputArray[i] != 0) {
            writeToDisplay(displayIteratorX, displayIteratorY, (char)inputArray[i]);
          }
          displayIteratorX++;
          if (displayIteratorX >= 16) {
            displayIteratorX = 0;
            displayIteratorY++;
          }
        }
        return true;
      } else {
        return false;
      }
    } else {
      return false;
    }
  }
  else {
    return false;
  }
}

void sendDataFromTermistorMatrix() {
  byte bufferToSend[131];
  bufferToSend[0] = 131;
  bufferToSend[1] = 2;
  int bufferToSendIterator = 2;
  for (int  i = 0; i < 8; i++) {
    for (int j = 0; j < 8; j++) {
      int measuredValue = getValueFromTermistor(i, j);
      bufferToSend[bufferToSendIterator++] = highByte(measuredValue);
      bufferToSend[bufferToSendIterator++] = lowByte(measuredValue);
    }
  }
  byte checkSum = 0;
  for (int i = 0; i < 130; i++) {
    checkSum += bufferToSend[i];
  }
  bufferToSend[130] = checkSum;
  Serial.write(bufferToSend, 131);
}

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  lcd.begin(16, 2);
  pinMode(SHIFT_REGISTER_ENABLE_PIN, OUTPUT);
  pinMode(MULTIPLEXER_ENABLE_PIN, OUTPUT);
  pinMode(TERMISTOR_MATRIX_OUTPUT_ANALOG_PIN, INPUT);
  pinMode(TERMISTOR_MATRIX_OUTPUT_ADDRESS_A, OUTPUT);
  pinMode(TERMISTOR_MATRIX_OUTPUT_ADDRESS_B, OUTPUT);
  pinMode(TERMISTOR_MATRIX_OUTPUT_ADDRESS_C, OUTPUT);
  pinMode(SHIFT_REGISTER_PIN_A, OUTPUT);
  pinMode(SHIFT_REGISTER_PIN_B, OUTPUT);
  pinMode(SHIFT_REGISTER_PIN_C, OUTPUT);
  pinMode(SIGNALING_LED, OUTPUT);

  digitalWrite(MULTIPLEXER_ENABLE_PIN, LOW);
  digitalWrite(SHIFT_REGISTER_ENABLE_PIN, LOW);

  Timer::CreateSpace(4);

  Timer::AddThread(&writeHeartBeatToLED, 1000);
  Timer::EnableThread(&writeHeartBeatToLED);
  Timer::AddThread(&writeHeartBeatToDisplay, 1000);
  Timer::EnableThread(&writeHeartBeatToDisplay);
  Timer::AddThread(&checkIfTelegramIsAvailableToReceive, 10);
  Timer::EnableThread(&checkIfTelegramIsAvailableToReceive);
  Timer::AddThread(&sendDataFromTermistorMatrix, 1000);
  Timer::EnableThread(&sendDataFromTermistorMatrix);
}

void loop() {}
