#include <timer.h>

//Creates the timer to send the data to the labview
auto sendDataTimer = timer_create_default();

//Building oscillation without counterweight
float buildingWithoutCW = 0;
//Building oscillation with counterweight
float buildingWithCW = 0;

float tableSpeed = 0;

bool SendData(void *)
{
  Serial.println(String(buildingWithoutCW) + "#" + String(buildingWithCW) + "#" + String(tableSpeed));

  return true;
}

void setup()
{
  Serial.begin(9600);

  sendDataTimer.every(1000, SendData);

  //Ports
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(12, OUTPUT);
}

void loop()
{
  //Tick all timers
  sendDataTimer.tick();

  //Get data from sensor
  buildingWithoutCW = analogRead(A0);
  buildingWithCW = analogRead(A1);

  if (Serial.available())
  {
    String command = Serial.readStringUntil('\n');

    // Write config on arduino
    if (Split(command, '#', 0) == "W")
    {
      tableSpeed = map(Split(command, '#', 1).toInt(), 0, 100, 0, 255);
      analogWrite(12, tableSpeed);
    }
  }
}

//Function to split strings based on separator
String Split(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length() - 1;

  for (int i = 0; i <= maxIndex && found <= index; i++)
  {
    if (data.charAt(i) == separator || i == maxIndex)
    {
      found++;
      strIndex[0] = strIndex[1] + 1;
      strIndex[1] = (i == maxIndex) ? i + 1 : i;
    }
  }

  return found > index ? data.substring(strIndex[0], strIndex[1]) : "";
}
