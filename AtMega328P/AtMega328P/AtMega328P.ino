#include <InterruptTimer.h>
#include "TelegramBuffer.h"

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
#define SERIAL_BAUD_RATE 9600

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

int termistorMatrixIteratorX = 0;
void sendDataFromTermistorMatrix() {
  byte bufferToSend[18];
  bufferToSend[0] = 18;
  bufferToSend[1] = 1;
  bufferToSend[2] = termistorMatrixIteratorX;
  int bufferToSendIterator = 3;
  for (int i = 0; i <= 7; i++) {
    int measuredValue = getValueFromTermistor(termistorMatrixIteratorX, i);
    bufferToSend[bufferToSendIterator++] = highByte(measuredValue);
    bufferToSend[bufferToSendIterator++] = lowByte(measuredValue);
  }
  byte checkSum = 0;
  for (int i = 0; i < 17; i++) {
    checkSum += bufferToSend[i];
  }
  bufferToSend[17] = checkSum;
  Serial.write(bufferToSend, 18);
  termistorMatrixIteratorX++;
  termistorMatrixIteratorX = termistorMatrixIteratorX % 8;
}

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
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

  Timer::CreateSpace(2);

  Timer::AddThread(&writeHeartBeatToLED, 1000);
  Timer::EnableThread(&writeHeartBeatToLED);
  Timer::AddThread(&sendDataFromTermistorMatrix, 30);
  Timer::EnableThread(&sendDataFromTermistorMatrix);
}

void loop() {
  while (Serial.available()) {
    TelegramBuffer::AddByteToBuffer(Serial.read());
  }
  TelegramBuffer::CheckIfBufferContainsTelegram(35);
}
