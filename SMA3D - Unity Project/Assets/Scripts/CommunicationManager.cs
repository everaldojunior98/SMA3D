using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CommunicationManager : MonoBehaviour
    {
        public TableRealBehaviour TableReal;
        public Dropdown PortsDropdown;
        public int ReadDelay = 10;

        private bool _connected;
        private SerialPort _port;

        private Thread _readThread;

        private float _buildingWithoutCw;
        private float _buildingWithCw;

        void Start()
        {
            _connected = false;
        }

        //Close connection on exit from scene
        void OnDestroy()
        {
            Disconnect();
        }

        //Create connection using COM port
        public void Connect()
        {
            if(PortsDropdown == null)
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

                //Create thread to read data
                if(_readThread != null && _readThread.IsAlive)
                    _readThread.Abort();

                _readThread = new Thread(() =>
                {
                    while (true)
                    {
                        //Read data from COM port
                        try
                        {
                            Debug.Log(_port.ReadLine());
                            ParseData(_port.ReadLine());
                        }
                        catch (TimeoutException)
                        {
                            Disconnect();
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
            if (_connected && _port.IsOpen)
            {
                _port.Close();
                _connected = false;

                if (_readThread != null && _readThread.IsAlive)
                    _readThread.Abort();
            }
        }

        public void Update()
        {
            if(!_connected)
                return;

            DebugGUI.Graph("WithCW", _buildingWithCw);
            DebugGUI.Graph("WithoutCW", _buildingWithoutCw);

            DebugGUI.Graph("WithCW1", _buildingWithCw);
            DebugGUI.Graph("WithoutCW1", _buildingWithoutCw);
        }

        public float tableSpeed;
        //Parse received data
        private void ParseData(string data)
        {
            if (!string.IsNullOrEmpty(data) && data.Split('#').Length == 3)
            {
                _buildingWithoutCw = float.Parse(data.Split('#')[0])/100;
                _buildingWithCw = float.Parse(data.Split('#')[1])/100;
                //var tableSpeed = float.Parse(data.Split('#')[2]);

                TableReal.UpdateValues(_buildingWithoutCw, _buildingWithCw, tableSpeed);
            }
        }
    }
}
