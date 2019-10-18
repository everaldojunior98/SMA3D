#include <MPU6050_tockn.h>
#include <Wire.h>

MPU6050 mpuBuildingWithoutCW(Wire); //OX68
MPU6050 mpuBuildingWithCW(Wire, 0x69); //Ox69

unsigned long timer = 0;

//Current table speed
float tableSpeed = 0;

void setup()
{
  Serial.begin(9600);
  
  Wire.begin();
  mpuBuildingWithoutCW.begin();
  mpuBuildingWithCW.begin();
}

void loop()
{
  mpuBuildingWithoutCW.update();
  mpuBuildingWithCW.update();

  if(millis() - timer > 10)
  {
    //Send data
    Serial.println(String(mpuBuildingWithoutCW.getAccX()) + "#" + String(mpuBuildingWithCW.getAccX()) + "#" + String(tableSpeed));
    timer = millis();
  }

  if (Serial.available())
  {
    //Read and set table speed
    String command = Serial.readStringUntil('\n');

    tableSpeed = map(command.toInt(), 0, 100, 0, 255);
    analogWrite(12, tableSpeed);
  }
}