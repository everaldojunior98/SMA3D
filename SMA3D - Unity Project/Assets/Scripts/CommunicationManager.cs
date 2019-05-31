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
        public int ReadDelay = 250;

        private bool _connected;
        private SerialPort _port;

        private Thread _readThread;

        void Start()
        {
            _connected = false;
        }

        void Update()
        {
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
                    Thread.Sleep(Mathf.Abs(ReadDelay));
                });
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

        //Parse received data
        private void ParseData(string data)
        {
            if (!string.IsNullOrEmpty(data) && data.Split('#').Length == 3)
            {
                var buildingWithoutCw = data.Split('#')[0];
                var buildingWithCw = data.Split('#')[1];
                var tableSpeed = data.Split('#')[2];

                if(float.TryParse(buildingWithoutCw, out var accelWithoutCw) && float.TryParse(buildingWithCw, out var accelWithCw))
                    TableReal.UpdateValues(accelWithoutCw, accelWithCw, accelWithCw / accelWithoutCw);
                else
                    TableReal.UpdateValues(0, 0, 0);
            }
        }
    }
}
