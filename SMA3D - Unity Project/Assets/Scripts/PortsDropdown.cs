using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PortsDropdown : MonoBehaviour
    {
        private Dropdown _dropdown;
        void Start()
        {
            _dropdown = GetComponent<Dropdown>();
        }

        void Update()
        {
            //Return if dropdown is null
            if(_dropdown == null)
                return;

            //Clear dropdown
            _dropdown.options.Clear();

            //Add all available ports on dropdown
            foreach (var portName in SerialPort.GetPortNames())
                _dropdown.options.Add(new Dropdown.OptionData(portName));
        }
    }
}
