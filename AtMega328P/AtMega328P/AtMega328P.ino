#include <InterruptTimer.h>
#include "TelegramBuffer.h"
#include "StaticDisplay.h"

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

int getValueFromTermistor(int posX, int posY) {
  digitalWrite(SHIFT_REGISTER_PIN_C, LOW);
  shiftOut(SHIFT_REGISTER_PIN_A, SHIFT_REGISTER_PIN_B, LSBFIRST, 1 << posX);
  digitalWrite(SHIFT_REGISTER_PIN_C, HIGH);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_A, posY & 0x01);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_B, (posY >> 1) & 0x01);
  digitalWrite(TERMISTOR_MATRIX_OUTPUT_ADDRESS_C, (posY >> 2) & 0x01);
  return analogRead(TERMISTOR_MATRIX_OUTPUT_ANALOG_PIN);
}

volatile bool LEDHeartBeatValue = false;
void writeHeartBeatToLED() {
  if (LEDHeartBeatValue) {
    digitalWrite(SIGNALING_LED, HIGH);
  } else {
    digitalWrite(SIGNALING_LED, LOW);
  }
  LEDHeartBeatValue = !LEDHeartBeatValue;
}

volatile int termistorMatrixIteratorY = 0;
void sendDataFromTermistorMatrix() {
  noInterrupts();
  byte bufferToSend[20];
  bufferToSend[0] = 20;
  bufferToSend[1] = 1;
  bufferToSend[2] = termistorMatrixIteratorY;
  int bufferToSendIterator = 3;
  for (int i = 0; i <= 7; i++)
  {
    int measuredValue = getValueFromTermistor(i, termistorMatrixIteratorY);
    bufferToSend[bufferToSendIterator++] = highByte(measuredValue);
    bufferToSend[bufferToSendIterator++] = lowByte(measuredValue);
  }
  byte checkSum = 0;
  for (int i = 0; i < 19; i++)
  {
    checkSum += bufferToSend[i];
  }
  bufferToSend[19] = checkSum;
  termistorMatrixIteratorY++;
  if (termistorMatrixIteratorY > 7)
  {
    termistorMatrixIteratorY = 0;
  }
  Serial.write(bufferToSend, 20);
  interrupts();
}

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  StaticDisplay::InitDisplay(16, 2);
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

  StaticDisplay::WriteToDisplay(0, 1, "Device is ready");
}

void loop() {
  while (Serial.available()) {
    TelegramBuffer::AddByteToBuffer(Serial.read());
  }
  TelegramBuffer::CheckIfBufferContainsTelegram(35);
}
