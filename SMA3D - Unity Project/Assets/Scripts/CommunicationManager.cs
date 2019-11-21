using System;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CommunicationManager : MonoBehaviour
    {
        public TableRealBehaviour TableReal;
        public Dropdown PortsDropdown;
        public Slider MotorSpeed;
        public int ReadDelay = 10;
        public GameObject MotorSpeedGameObject;
        public SetTranslation ConnectTranslation;

        private bool _connected;
        private SerialPort _port;

        private Thread _readThread;

        private float _buildingWithoutCw;
        private float _buildingWithCw;
        private float _tableSpeed;

        private bool _disconnecting;

        void Start()
        {
            _connected = false;
            _disconnecting = false;
        }

        //Close connection on exit from scene
        void OnDestroy()
        {
            Disconnect();
        }

        //Create connection using COM port
        public void Connect()
        {
            if (_connected)
            {
                Disconnect();
                return;
            }

            if (PortsDropdown == null)
                return;

            try
            {
                //Create port
                _port = new SerialPort
                {
                    PortName = PortsDropdown.options[PortsDropdown.value].text,
                    BaudRate = 9600,
                    ReadTimeout = 2000,
                    WriteTimeout = 2000
                };

                //Connect to port
                _port.Open();
                _connected = true;
                _disconnecting = false;

                MotorSpeed.value = 0;

                //Create thread to read data
                if(_readThread != null && _readThread.IsAlive)
                    _readThread.Abort();

                _readThread = new Thread(() =>
                {
                    while (!_disconnecting)
                    {
                        //Read data from COM port
                        try
                        {
                            if (_port.IsOpen)
                            {
                                ParseData(_port.ReadLine());
                            }
                        }
                        catch (TimeoutException)
                        {
                            //Disconnect();
                        }

                        Thread.Sleep(ReadDelay);
                    }
                });
                _readThread.Start();

                DebugGUI.SetGraphProperties("WithCW", TranslationManager.Instance.GetTranslation("STRUCTURE_WITH"), -1, 1, 1, Color.white, true);
                DebugGUI.SetGraphProperties("WithoutCW", TranslationManager.Instance.GetTranslation("STRUCTURE_WITHOUT"), -1, 1, 2, Color.red, true);

                DebugGUI.SetGraphProperties("WithCW1", TranslationManager.Instance.GetTranslation("STRUCTURE_WITH"), -1, 1, 0, Color.white, true);
                DebugGUI.SetGraphProperties("WithoutCW1", TranslationManager.Instance.GetTranslation("STRUCTURE_WITHOUT"), -1, 1, 0, Color.red, true);
            }
            catch
            {
                _connected = false;
            }
        }

        //Disconnect if has valid connection
        public void Disconnect()
        {
            if (_connected)
            {
                _disconnecting = true;

                _port?.Close();
                _connected = false;

                _readThread?.Abort();
            }
        }

        public void Update()
        {
            ConnectTranslation.Key = _connected && !_disconnecting ? "DISCONNECT" : "CONNECT";
            MotorSpeedGameObject.SetActive(_connected && !_disconnecting);
            PortsDropdown.interactable = !_connected;

            if (!_connected || _disconnecting)
            {
                DebugGUI.RemoveGraph("WithCW");
                DebugGUI.RemoveGraph("WithCW1");
                DebugGUI.RemoveGraph("WithoutCW");
                DebugGUI.RemoveGraph("WithoutCW1");

                return;
            }

            DebugGUI.Graph("WithCW", _buildingWithCw);
            DebugGUI.Graph("WithoutCW", _buildingWithoutCw);

            DebugGUI.Graph("WithCW1", _buildingWithCw);
            DebugGUI.Graph("WithoutCW1", _buildingWithoutCw);
        }

        //Parse received data
        private void ParseData(string data)
        {
            Debug.Log(data);
            try
            {
                if (!string.IsNullOrEmpty(data) && data.Split('#').Length == 3)
                {
                    //Divide por 100 pois a unity faz o parse de 3.12 como 312
                    _buildingWithoutCw = float.Parse(data.Split('#')[0]) / 100;
                    _buildingWithCw = float.Parse(data.Split('#')[1]) / 100;
                    _tableSpeed = float.Parse(data.Split('#')[2]) / 100;

                    if (_buildingWithoutCw > 1)
                        _buildingWithoutCw = 1;
                    else if (_buildingWithoutCw < -1)
                        _buildingWithoutCw = -1;

                    if (_buildingWithCw > 1)
                        _buildingWithCw = 1;
                    else if (_buildingWithCw < -1)
                        _buildingWithCw = -1;

                    TableReal.UpdateValues(_buildingWithoutCw, _buildingWithCw, _tableSpeed);
                }
            }
            catch
            {
                TableReal.UpdateValues(0, 0, 0);
            }
        }

        public void SendPWM()
        {
            _port?.Write((int)MotorSpeed.value + "#");
        }

        private float Map(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
