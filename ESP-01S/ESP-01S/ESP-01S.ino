#include <ESP8266WiFi.h>
#include <TimeSynchronizer.h>
#include <Timer.h>
#include <ArduinoJson.h>
#include <WiFiUdp.h>

#define SERIAL_BAUD_RATE 9600
#define LOCAL_UDP_PORT 2390
#define WIFI_SSID "Router-Cisco"
#define WIFI_PASSWORD "RdCuXaAa"
#define MAX_RECEIVED_BUFFER_SIZE 128
#define HOST_ADDRESS "192.168.0.11"
#define HOST_PORT 1989

TimeSynchronizer timeSynchronizer;

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

void printDataOnDisplay(String valueToDisplay) {
  if (valueToDisplay.length() <= 32) {
    byte bufferToSend[35];
    bufferToSend[0] = 3 + valueToDisplay.length();
    bufferToSend[1] = 1;
    for (int i = 0; i < valueToDisplay.length(); i++) {
      bufferToSend[i + 2] = (byte)valueToDisplay[i];
    }
    for (int i = 0; i < 2 + valueToDisplay.length(); i++) {
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

void setup() {
  Serial.begin(SERIAL_BAUD_RATE);
  const char* ssid     = WIFI_SSID;
  const char* password = WIFI_PASSWORD;
  WiFi.begin(ssid, password);
  udp.begin(LOCAL_UDP_PORT);
  int wifi_ctr = 0;
  byte getTimeRetries = 0;
  while (!getTimeFromServer()) {
    String outputForDisplay = "Get Time Retry";
    outputForDisplay += (char)0;
    outputForDisplay += (char)0;
    outputForDisplay += String(getTimeRetries);
    printDataOnDisplay(outputForDisplay);
    getTimeRetries++;
  }
  printDataOnDisplay("NTP Success");
}

bool checkIfTemperatureTelegram(byte inputArray[MAX_RECEIVED_BUFFER_SIZE], byte inputArrayInterator) {
  if (inputArray[0] == inputArrayInterator) {
    if (inputArray[1] == 1) {
      byte checkSum = 0;
      for (int i = 0; i <= inputArrayInterator - 1; i++) {
        checkSum += inputArray[i];
      }
      if (inputArray[inputArrayInterator - 1] == checkSum) {
        byte bufferToSend[7];
        bufferToSend[0] = 7;
        bufferToSend[1] = 3;
        bufferToSend[2] = inputArray[2];
        bufferToSend[3] = inputArray[3];
        bufferToSend[4] = inputArray[4];
        bufferToSend[5] = inputArray[5];
        for (int i = 0 ; i < 6; i++) {
          bufferToSend[6] += bufferToSend[i];
        }
        WiFiClient client;
        if (!client.connect(HOST_ADDRESS, HOST_PORT)) {
          return true;
        }
        client.write(bufferToSend, 7);
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

void sendTelegramToVisualisation() {

}

void checkIfTelegramIsAvailableToReceive() {
  if (Serial.available()) {
    byte receivedBufferIterator = 0;
    byte receivedBuffer[MAX_RECEIVED_BUFFER_SIZE];
    unsigned long startTime = millis();
    int timeoutMS = 50;
    do
    {
      receivedBuffer[receivedBufferIterator++] = Serial.read();
    } while (!(receivedBuffer[0] == receivedBufferIterator) && millis() - startTime < timeoutMS);
    if (checkIfTemperatureTelegram(receivedBuffer, receivedBufferIterator)) {
      return;
    } else {
      return;
    }
  }
}

void loop() {
  checkIfTelegramIsAvailableToReceive();
}
