using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CommunicationManager : MonoBehaviour
    {
        public Dropdown PortsDropdown;

        private bool _connected;
        private SerialPort _port;

        void Start()
        {
            _connected = false;
        }

        void Update()
        {
            if(!_connected || PortsDropdown == null)
                return;

            //Read data from COM port
            try
            {
                Debug.Log(_port.ReadLine());
            }
            catch (TimeoutException) { }
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
                _port.Close();
        }
    }
}
