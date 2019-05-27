/* 
 * Description: Shows the behavior of the structure based on the assembled physical model.
 * Author: Everaldo Junior
 * Date: 19/05/2019
 */

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TableRealBehaviour : MonoBehaviour
    {
        //Fields in HUD
        public Text BuildingWithoutCwText;
        public Text BuildingWithCwText;
        public Text CorrectionFactorText;

        //Transforms to set oscilation position
        public Transform TableTransform;
        public Transform BuildingWithoutCwTransform;
        public Transform BuildingWithCwTransform;

        public Transform PendulumTransform;

        private float _correctionFactor;

        //Coefficient of oscillation of the material of the structure
        public float OscillationCoefficient = 20;

        private float _frequencyWithoutCw;
        private float _frequencyWithCw;

        //Table max amplitude
        private float _amplitude = 0.35f;

        //Var to control connection with Arduino
        private bool _isConnected;

        void Start()
        {
            _isConnected = false;
        }

        public void UpdateValues(float frequencyWithoutCw, float frequencyWithCw, float correctionFactor)
        {
            _isConnected = true;
            _frequencyWithoutCw = frequencyWithoutCw;
            _frequencyWithCw = frequencyWithCw;
            _correctionFactor = correctionFactor;
        }

        void Update()
        {
            if (!_isConnected)
                return;

            //Fill HUD
            BuildingWithoutCwText.text = "Frequência: " + _frequencyWithoutCw.ToString("F2") + " Hz";
            BuildingWithCwText.text = "Frequência: " + _frequencyWithCw.ToString("F2") + " Hz";
            CorrectionFactorText.text = "Fator: " + (_correctionFactor * 100).ToString("F2") + "%";

            //Calculate the oscilation based on equation ASin(wt) and set objects position
            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                var posByTimeWithoutCw = _amplitude * Mathf.Sin(2 * Mathf.PI * _frequencyWithoutCw * Time.time);
                var posByTimeWithCw = _amplitude * Mathf.Sin(2 * Mathf.PI * _frequencyWithCw * Time.time);
                TableTransform.position = new Vector3(posByTimeWithoutCw, TableTransform.position.y, TableTransform.position.z);

                PendulumTransform.eulerAngles = new Vector3(PendulumTransform.eulerAngles.x, PendulumTransform.eulerAngles.y,  OscillationCoefficient * posByTimeWithoutCw);

                BuildingWithoutCwTransform.eulerAngles = new Vector3(BuildingWithoutCwTransform.eulerAngles.x, BuildingWithoutCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTimeWithoutCw);
                BuildingWithCwTransform.eulerAngles = new Vector3(BuildingWithCwTransform.eulerAngles.x, BuildingWithCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTimeWithCw);
            }
        }
    }
}
