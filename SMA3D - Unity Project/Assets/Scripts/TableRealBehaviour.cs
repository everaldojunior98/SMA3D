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

        //Transforms to set oscillation position
        public Transform TableTransform;
        public Transform BuildingWithoutCwTransform;
        public Transform BuildingWithCwTransform;

        public Transform PendulumTransform;

        private float _correctionFactor;

        //Coefficient of oscillation of the material of the structure
        public float OscillationCoefficient = 20;

        private float _frequencyWithoutCw;
        private float _frequencyWithCw;

        //Timer to calculate frequency
        private float _timer = 1;

        //Vars to calculate frequency
        private float _lastAccelWithoutCw;
        private int _inversionsWithoutCw;
        private float _periodWithoutCw;
        private float _lastPeriodWithoutCw;

        //Vars to calculate frequency
        private float _lastAccelWithCw;
        private int _inversionsWithCw;
        private float _periodWithCw;
        private float _lastPeriodWithCw;

        //Table max amplitude
        private float _amplitude = 0.35f;

        //Var to control connection with Arduino
        private bool _isConnected;

        void Start()
        {
            _isConnected = false;

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " 0.00%";
        }

        public void UpdateValues(float accelWithoutCw, float accelWithCw, float correctionFactor)
        {
            _isConnected = true;
            _correctionFactor = correctionFactor;

            //Calculate period for structure without CW
            if (accelWithoutCw < 0 && _lastAccelWithoutCw > 0)
                _inversionsWithoutCw++;
            else if (accelWithoutCw > 0 && _lastAccelWithoutCw < 0)
                _inversionsWithoutCw++;

            if (_inversionsWithoutCw == 2)
            {
                _periodWithoutCw = Time.time - _lastPeriodWithoutCw;
                _inversionsWithoutCw = 0;
                _lastPeriodWithoutCw = Time.time;
            }

            _lastAccelWithoutCw = accelWithoutCw;

            //Calculate period for structure with CW
            if (accelWithCw < 0 && _lastAccelWithCw > 0)
                _inversionsWithCw++;
            else if (accelWithCw > 0 && _lastAccelWithCw < 0)
                _inversionsWithCw++;

            if (_inversionsWithCw == 2)
            {
                _periodWithCw = Time.time - _lastPeriodWithCw;
                _inversionsWithCw = 0;
                _lastPeriodWithCw = Time.time;
            }

            _lastAccelWithCw = accelWithCw;
        }

        void Update()
        {
            if (!_isConnected)
                return;

            //Calculates frequency
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _frequencyWithoutCw = 1 / _periodWithoutCw;
                _frequencyWithCw = 1 / _periodWithCw;
                
                _timer = 1;
            }

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " + _frequencyWithoutCw.ToString("F2") + " Hz";
            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " + _frequencyWithCw.ToString("F2") + " Hz";
            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " " + (_correctionFactor * 100).ToString("F2") + "%";

            //Calculate the oscillation based on equation ASin(wt) and set objects position
            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                var posByTimeWithoutCw = _amplitude * Mathf.Sin(2 * Mathf.PI * _frequencyWithoutCw * Time.time);
                var posByTimeWithCw = _amplitude * Mathf.Sin(2 * Mathf.PI * _frequencyWithCw * Time.time);
                TableTransform.position =
                    new Vector3(posByTimeWithoutCw, TableTransform.position.y, TableTransform.position.z);

                PendulumTransform.eulerAngles = new Vector3(PendulumTransform.eulerAngles.x,
                    PendulumTransform.eulerAngles.y, OscillationCoefficient * posByTimeWithoutCw);

                BuildingWithoutCwTransform.eulerAngles = new Vector3(BuildingWithoutCwTransform.eulerAngles.x,
                    BuildingWithoutCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTimeWithoutCw);
                BuildingWithCwTransform.eulerAngles = new Vector3(BuildingWithCwTransform.eulerAngles.x,
                    BuildingWithCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTimeWithCw);
            }
        }
    }
}
