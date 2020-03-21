#include <ESP8266WiFi.h>
#include <TimeSynchronizer.h>
#include <Timer.h>
#include <ArduinoJson.h>
#include <WiFiUdp.h>

#define SERIAL_BAUD_RATE 115200
#define LOCAL_UDP_PORT 2390
#define WIFI_SSID "Router-Cisco"
#define WIFI_PASSWORD "RdCuXaAa"
#define MAX_RECEIVED_BUFFER_SIZE 131
#define HOST_ADDRESS "192.168.0.11"
#define HOST_PORT 1989
#define SIGNALING_LED 2

TimeSynchronizer timeSynchronizer;
Timer timer(1);

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
  printDataOnDisplay("Sending data");
  LEDHeartBeatValue = !LEDHeartBeatValue;
}

void printDataOnDisplay(String valueToDisplay) {
  if (valueToDisplay.length() <= 32) {
    byte bufferToSend[35];
    bufferToSend[0] = 3 + valueToDisplay.length();
    bufferToSend[1] = 1;
    for (int i = 0; i < valueToDisplay.length(); i++) {
      bufferToSend[i + 2] = (byte)valueToDisplay[i];
    }
    for (int i = 0; i < bufferToSend[0] - 1; i++) {
      bufferToSend[2 + valueToDisplay.length()] += bufferToSend[i];
    }
    Serial.write(bufferToSend, bufferToSend[0]);
  } else {
    return;
  }
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

bool checkIfTemperatureTelegram(byte *inputArray, byte inputArrayInterator) {
  if (inputArray[0] == inputArrayInterator) {
    if (inputArray[1] == 2) {
      byte checkSum = 0;
      for (int i = 0; i < inputArrayInterator - 1; i++) {
        checkSum += inputArray[i];
      }
      if (inputArray[inputArrayInterator - 1] == checkSum) {
        return sendTelegramToVisualisation(inputArray);
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

bool sendTelegramToVisualisation(byte *inputArray) {
  inputArray[1] = 3;
  inputArray[130] = inputArray[130] + 1;
  WiFiClient client;
  if (!client.connect(HOST_ADDRESS, HOST_PORT)) {
    return true;
  }
  client.write(inputArray, 131);
  return true;
}

byte receivedBufferIterator = 0;
byte receivedBuffer[MAX_RECEIVED_BUFFER_SIZE];
void checkIfTelegramIsAvailableToReceive() {
  if (Serial.available()) {
    while (Serial.available()) {
      receivedBuffer[receivedBufferIterator++] = Serial.read();
    }
    if (checkIfTemperatureTelegram(receivedBuffer, receivedBufferIterator)) {
      //found matching telegram
      receivedBufferIterator = 0;
      //else if statement is for check for next telegram type
    } else {
      //no matching telegram
    }
    if (receivedBufferIterator == MAX_RECEIVED_BUFFER_SIZE) {
      receivedBufferIterator = 0;
      while (Serial.available()) {
        Serial.read();
      }
    }
  }
}

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  pinMode(SIGNALING_LED, OUTPUT);
  udp.begin(LOCAL_UDP_PORT);
  int wifi_ctr = 0;
  timer.AddThread(&writeHeartBeatToLED, 1000);
  printDataOnDisplay("NTP connecting");
  while (!getTimeFromServer()) {
    timer.CheckThreads();
    yield();
  }
  printDataOnDisplay("NTP Success");
}

void loop() {
  checkIfTelegramIsAvailableToReceive();
  timer.CheckThreads();
}
