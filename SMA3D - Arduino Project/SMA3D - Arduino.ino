/*
  StartMpuConnection and RetrieveBuildingAcceleration are based on Shuvashish Sarker code.
  Link: https://gitlab.com/shuvashish/batikkrom.com/blob/master/MuttipleMPU/MuttipleMPU.ino
*/

#include <timer.h>
#include <Wire.h>
#include <SoftwareSerial.h>

//MPU address
const int mpuBuildingWithoutCW = 0x68;
const int mpuBuildingWithCW = 0x69;

//Creates the timer to send the data to the labview
auto sendDataTimer = timer_create_default();

//Building oscillation without counterweight
float buildingWithoutCW = 0;
//Building oscillation with counterweight
float buildingWithCW = 0;

float tableSpeed = 0;

//Frequency withoutCW
int withoutCWInversions = 0;
unsigned long int withoutCWLastTime = 0;
float withoutCWLastAccel = 0;
float withoutCWPeriod = 0;

//Frequency withCW
int withCWInversions = 0;
unsigned long int withCWLastTime = 0;
float withCWLastAccel = 0;
float withCWPeriod = 0;

SoftwareSerial SerialLabview(3, 2); // RX, TX

bool SendData(void *)
{
  Serial.println(String(buildingWithoutCW) + "#" + String(buildingWithCW) + "#" + String(tableSpeed));
  SerialLabview.println(String(buildingWithoutCW) + "#" + String(buildingWithCW) + "#" + String(tableSpeed));

  return true;
}

void setup()
{
  Serial.begin(9600);
  SerialLabview.begin(9600);

  sendDataTimer.every(250, SendData);

  //Start mpu connecton
  StartMpuConnection(mpuBuildingWithoutCW);
  StartMpuConnection(mpuBuildingWithCW);

  //Ports
  pinMode(12, OUTPUT);
}

void loop()
{
  //Tick all timers
  sendDataTimer.tick();

  //Get data from sensor
  buildingWithoutCW = RetrieveBuildingAcceleration(mpuBuildingWithoutCW);
  buildingWithCW = RetrieveBuildingAcceleration(mpuBuildingWithCW);

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
  
  delay(100);
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

//Function to start connection with MPU
void StartMpuConnection(int mpuAddress)
{
  //Create comm with mpu
  Wire.begin();

  //Start transmission with mpu I2C address
  Wire.beginTransmission(mpuAddress);
  //Accessing the register 6B
  Wire.write(0x6B);
  //Setting SLEEP register to 0.
  Wire.write(0b00000000);
  Wire.endTransmission();

  //Start transmission with mpu I2C address
  Wire.beginTransmission(mpuAddress);
  //Accessing the register 1B (Gyroscope)
  Wire.write(0x1B);
  //Setting the gyro to full scale +/- 250deg./s
  Wire.write(0x00000000);
  Wire.endTransmission();

  Wire.beginTransmission(mpuAddress);
  //Accessing the register 1C (Acccelerometer)
  Wire.write(0x1C);
  //Setting the accel to +/- 2g
  Wire.write(0b00000000);
  Wire.endTransmission();
}

//Function to retrieve and process MPU acceleration
float RetrieveBuildingAcceleration(int mpuAddress)
{
  Wire.beginTransmission(mpuAddress);
  Wire.write(0x3B);
  Wire.endTransmission();
  Wire.requestFrom(mpuAddress, 6);
  while (Wire.available() < 6);

  //Retrieve accel from MPU
  long accelX = Wire.read() << 8 | Wire.read();
  long accelY = Wire.read() << 8 | Wire.read();
  long accelZ = Wire.read() << 8 | Wire.read();

  Wire.beginTransmission(mpuAddress);
  Wire.write(0x43);
  Wire.endTransmission();
  Wire.requestFrom(mpuAddress, 6);
  while (Wire.available() < 6);

  //Retrieve gyro from MPU
  float gyroX = Wire.read() << 8 | Wire.read();
  float gyroY = Wire.read() << 8 | Wire.read();
  float gyroZ = Wire.read() << 8 | Wire.read();

  //Process accel data
  float gForceX = accelX / 16384.0;

  return gForceX;
}