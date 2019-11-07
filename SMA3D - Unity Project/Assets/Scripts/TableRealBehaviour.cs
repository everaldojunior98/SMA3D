/* 
 * Description: Shows the behavior of the structure based on the assembled physical model.
 * Author: Everaldo Junior
 * Date: 19/05/2019
 */

using System;
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

        public float lerpSpeed = 2.5f;

        //Transforms to set oscillation position
        public Transform TableTransform;
        public Transform BuildingWithoutCwTransform;
        public Transform BuildingWithCwTransform;

        public Transform PendulumTransform;

        //Coefficient of oscillation of the material of the structure
        public float OscillationCoefficient = 20;

        //Table max amplitude
        private float _amplitude = 0.35f;

        //Var to control connection with Arduino
        private bool _isConnected;


        private float _accelWithout;
        private float _accelWith;
        private float _tableSpeed;

        private float _frequencyWithoutCw;
        private float _frequencyWithCw;
        private float _correctionFactor;


        private int _inversionsWithCw;
        private int _initialInversionTimeWithCw;
        private float _timer;
        private int _inversionsWithoutCw;
        private int _initialInversionTimeWithoutCw;

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

            if (Math.Abs(_accelWith) < 0.02f)
                _inversionsWithCw++;

            if (Math.Abs(_accelWithout) < 0.02f)
                _inversionsWithoutCw++;
        }

        void Update()
        {
            if (!_isConnected)
                return;

            _timer += Time.deltaTime;

            //Calculates de frequency with cw
            if (_inversionsWithCw == 2)
            {
                _inversionsWithCw = 0;
                _frequencyWithCw = 1f / ((int) _timer % 60 - _initialInversionTimeWithCw);

                _initialInversionTimeWithCw = (int) _timer % 60;
            }

            //Calculates de frequency with cw
            if (_inversionsWithoutCw == 2)
            {
                _inversionsWithoutCw = 0;
                _frequencyWithoutCw = 1f / ((int)_timer % 60 - _initialInversionTimeWithoutCw);

                _initialInversionTimeWithoutCw = (int)_timer % 60;
            }

            //Calculate de correction factor
            _correctionFactor = 1 - _frequencyWithCw / _frequencyWithoutCw;

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " +
                                         _frequencyWithoutCw.ToString("F2") + " Hz";

            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " +
                                      _frequencyWithCw.ToString("F2") + " Hz";

            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " " +
                                        (float.IsInfinity(_correctionFactor) || float.IsNaN(_correctionFactor)
                                            ? "0.00"
                                            : (_correctionFactor * 100).ToString("F2")) + "%";

            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                TableTransform.position = Vector3.Lerp(TableTransform.position, new Vector3(_accelWithout * _amplitude, TableTransform.position.y, TableTransform.position.z), lerpSpeed * Time.deltaTime);

                PendulumTransform.eulerAngles = Vector3.Lerp(PendulumTransform.eulerAngles, new Vector3(PendulumTransform.eulerAngles.x, PendulumTransform.eulerAngles.y, - OscillationCoefficient * _accelWithout * 2), lerpSpeed * Time.deltaTime);

                BuildingWithoutCwTransform.eulerAngles = Vector3.Lerp(BuildingWithoutCwTransform.eulerAngles, new Vector3(BuildingWithoutCwTransform.eulerAngles.x, BuildingWithoutCwTransform.eulerAngles.y, -90 - 10 * _accelWithout), lerpSpeed* Time.deltaTime);
                BuildingWithCwTransform.eulerAngles = Vector3.Lerp(BuildingWithCwTransform.eulerAngles, new Vector3(BuildingWithCwTransform.eulerAngles.x, BuildingWithCwTransform.eulerAngles.y, -90 - 10 * _accelWith), lerpSpeed* Time.deltaTime);
            }
        }
    }
}
