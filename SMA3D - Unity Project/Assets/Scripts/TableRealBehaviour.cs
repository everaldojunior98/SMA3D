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


        private float _accelWithout;
        private float _accelWith;
        private float _tableSpeed;

        void Start()
        {
            _isConnected = false;

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " 0.00%";
        }

        public void UpdateValues(float accelWithoutCw, float accelWithCw, float tableSpeed)
        {
            _isConnected = true;

            _accelWithout = accelWithoutCw;
            _accelWith = accelWithCw;
            _tableSpeed = tableSpeed;
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

            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                TableTransform.position = new Vector3(_accelWithout * _amplitude, TableTransform.position.y, TableTransform.position.z);

                PendulumTransform.eulerAngles = new Vector3(PendulumTransform.eulerAngles.x, PendulumTransform.eulerAngles.y, - OscillationCoefficient * _accelWithout * 2);

                BuildingWithoutCwTransform.eulerAngles = new Vector3(BuildingWithoutCwTransform.eulerAngles.x, BuildingWithoutCwTransform.eulerAngles.y, -90 - 10 * _accelWithout);
                BuildingWithCwTransform.eulerAngles = new Vector3(BuildingWithCwTransform.eulerAngles.x, BuildingWithCwTransform.eulerAngles.y, -90 - 10 * _accelWith);
            }
        }
    }
}
