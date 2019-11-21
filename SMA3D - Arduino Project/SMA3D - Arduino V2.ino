#include <MPU6050_tockn.h>
#include <Wire.h>

MPU6050 mpuBuildingWithoutCW(Wire); //OX68
MPU6050 mpuBuildingWithCW(Wire, 0x69); //Ox69

unsigned long timer = 0;

//Current table speed
float tableSpeed = 0;

//Pinout Motor Shield
int enb = 2;
int ina = 3;
int inb = 4;
int pwm = 5;

void setup()
{
  Serial.begin(9600);

  Wire.begin();
  mpuBuildingWithoutCW.begin();
  mpuBuildingWithCW.begin();

  pinMode(enb, OUTPUT);
  pinMode(ina, OUTPUT);
  pinMode(inb, OUTPUT);
  pinMode(pwm, OUTPUT);

  digitalWrite(enb, HIGH);
  digitalWrite(ina, HIGH);
  digitalWrite(inb, LOW);
}

void loop()
{
  mpuBuildingWithoutCW.update();
  mpuBuildingWithCW.update();

  if (millis() - timer > 10)
  {
    //Send data
    Serial.println(String(mpuBuildingWithoutCW.getAccX()) + "#" + String(mpuBuildingWithCW.getAccX()) + "#" + String(tableSpeed));
    timer = millis();
  }
  //Read and set table speed
  String command = "";

  while (Serial.available() > 0)
  {
    char c = Serial.read();

    if (c != '#'){
      command.concat(c);
      Serial.println(command);}
    else if (command.length() > 0 && command.length() < 4)
    {
      tableSpeed = map(command.toInt(), 0, 100, 0, 255);
      analogWrite(pwm, tableSpeed);
    }
  }
}