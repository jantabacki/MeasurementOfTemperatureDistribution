#include <ESP8266WiFi.h>
#include <TimeSynchronizer.h>
#include <Timer.h>
#include <WiFiUdp.h>
#include "TelegramBuffer.h"

#define SERIAL_BAUD_RATE 9600
#define LOCAL_UDP_PORT 2390
#define WIFI_SSID "Router-Cisco"
#define WIFI_PASSWORD "RdCuXaAa"
#define SIGNALING_LED 2

TimeSynchronizer timeSynchronizer;
Timer timer(2);

byte packetBuffer[48];
WiFiUDP udp;
void sendNTPpacket(IPAddress& address) {
  memset(packetBuffer, 0, 48);
  packetBuffer[0] = 0b11100011;
  packetBuffer[1] = 0;
  packetBuffer[2] = 6;
  packetBuffer[3] = 0xEC;
  udp.beginPacket(address, 123);
  udp.write(packetBuffer, 48);
  udp.endPacket();
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

bool LCDHeartBeatValue = false;
void writeHeartBeatToLCD() {
  LCDHeartBeatValue = !LCDHeartBeatValue;
  byte displayTelegramToSend[35];
  displayTelegramToSend[0] = 35;
  displayTelegramToSend[1] = 1;
  displayTelegramToSend[2] = (byte)'E';
  displayTelegramToSend[3] = (byte)'S';
  displayTelegramToSend[4] = (byte)'P';
  displayTelegramToSend[5] = (byte)' ';
  if (LCDHeartBeatValue) {
    displayTelegramToSend[6] = (byte)'X';
  } else {
    displayTelegramToSend[6] = (byte)'+';
  }
  for (int i = 7; i < 35; i++) {
    displayTelegramToSend[i] = 0;
  }
  byte checkSum = 0;
  for (int j = 0; j < 34; j++)
  {
    checkSum += displayTelegramToSend[j];
  }
  displayTelegramToSend[34] = checkSum;
  Serial.write(displayTelegramToSend, 35);
}

bool getTimeFromServer() {
  if (WiFi.status() != WL_CONNECTED) {
    return false;
  }
  IPAddress timeServerIP;
  WiFi.hostByName("pl.pool.ntp.org", timeServerIP);
  sendNTPpacket(timeServerIP);
  int startTime = millis();
  int timeout = 2500;
  int cb = udp.parsePacket();
  while (!cb) {
    if (millis() - startTime > timeout) {
      return false;
    }
    cb = udp.parsePacket();
  }
  udp.read(packetBuffer, 48);
  unsigned long highWord = word(packetBuffer[40], packetBuffer[41]);
  unsigned long lowWord = word(packetBuffer[42], packetBuffer[43]);
  unsigned long secsSince1900 = highWord << 16 | lowWord;
  timeSynchronizer.ExternalSynchronization(0, 0, secsSince1900);
  return true;
}

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  pinMode(SIGNALING_LED, OUTPUT);
  udp.begin(LOCAL_UDP_PORT);
  int wifi_ctr = 0;
  timer.AddThread(&writeHeartBeatToLED, 1000);
  timer.AddThread(&writeHeartBeatToLCD, 1000);
  while (!getTimeFromServer()) {
    timer.CheckThreads();
    yield();
  }
}

void loop() {
  while (Serial.available()) {
    TelegramBuffer::AddByteToBuffer(Serial.read());
  }
  TelegramBuffer::CheckIfBufferContainsTelegram(18);
  timer.CheckThreads();
}
