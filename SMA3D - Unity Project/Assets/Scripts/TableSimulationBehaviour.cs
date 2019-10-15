/* 
 * Description: Simulate structure oscillation and calculates pendulum lenght e correction factor.
 * Author: Everaldo Junior
 * Date: 19/05/2019
 */

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TableSimulationBehaviour : MonoBehaviour
    {
        //Fields in UI
        public Text UiFrequencyWithoutCwText;
        public Text UiCorrectionFactor;
        public Text UiPendulumLenght;

        //Fields in HUD
        public Text BuildingWithoutCwText;
        public Text BuildingWithCwText;
        public Text CorrectionFactorText;
        public Text PendulumLenghtText;

        //Transforms to set oscilation position
        public Transform TableTransform;
        public Transform BuildingWithoutCwTransform;
        public Transform BuildingWithCwTransform;

        public Transform PendulumTransform;

        public Transform PendulumBarTransform;
        public Transform PendulumMassTransform;

        public float CorrectionFactor = 0.2f;

        //Coefficient of oscillation of the material of the structure
        public float OscillationCoefficient = 2;

        //Frequency with and without counterweight
        public float FrequencyWithoutCw;
        public float FrequencyWithCw;

        public float PendulumLength;
        private float _lastPendulumLength;

        //Table max amplitude
        private float _amplitude = 0.35f;

        //variables to control simulation state
        private bool _isSimulating;
        private bool _usingPendulumLength;

        void Start()
        {
            _isSimulating = false;
            _usingPendulumLength = false;

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " 0.00 Hz";
            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " 0.00%";
            PendulumLenghtText.text = TranslationManager.Instance.GetTranslation("LENGTH") + " 0.00 m";
        }

        void Update()
        {
            if (!_isSimulating)
                return;

            if (_usingPendulumLength)
            {
                //if user set pendulum length calculate de pendulum frequency = 1/(2*pi*(l/g)^(1/2)) and correction factor
                FrequencyWithCw = FrequencyWithoutCw - 1 / (2 * Mathf.PI * Mathf.Sqrt(PendulumLength/9.8f));
                CorrectionFactor = 1 - FrequencyWithCw / FrequencyWithoutCw;
            }
            else
            {
                //if use correction factor calculate pendulum length
                PendulumLength = CalculatePendulumLength(CorrectionFactor * FrequencyWithoutCw);
                FrequencyWithCw = (1 - CorrectionFactor) * FrequencyWithoutCw;
            }

            //Set the pendulum length
            if (_lastPendulumLength != PendulumLength)
            {
                _lastPendulumLength = PendulumLength;
                PendulumBarTransform.localScale = new Vector3(PendulumBarTransform.localScale.x, PendulumLength * 100, PendulumBarTransform.localScale.z);
                PendulumMassTransform.localPosition = new Vector3(PendulumMassTransform.localPosition.x, -PendulumLength*10, PendulumMassTransform.localPosition.z);
            }

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " + FrequencyWithoutCw.ToString("F2") + " Hz";
            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " + FrequencyWithCw.ToString("F2") + " Hz";
            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " " + (CorrectionFactor * 100).ToString("F2") + "%";
            PendulumLenghtText.text = TranslationManager.Instance.GetTranslation("LENGTH") + " " + PendulumLength.ToString("F2") + " m";

            //Calculate the oscillation based on equation ASin(wt) and set objects position
            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                var posByTime = _amplitude * Mathf.Sin(2 * Mathf.PI * FrequencyWithoutCw * Time.time);
                TableTransform.position = new Vector3(posByTime, TableTransform.position.y, TableTransform.position.z);

                PendulumTransform.eulerAngles = new Vector3(PendulumTransform.eulerAngles.x, PendulumTransform.eulerAngles.y,  OscillationCoefficient * posByTime);

                BuildingWithoutCwTransform.eulerAngles = new Vector3(BuildingWithoutCwTransform.eulerAngles.x, BuildingWithoutCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTime);
                BuildingWithCwTransform.eulerAngles = new Vector3(BuildingWithCwTransform.eulerAngles.x, BuildingWithCwTransform.eulerAngles.y, -90 - OscillationCoefficient * posByTime * (1 - CorrectionFactor));
            }
        }

        public void StartSimulation()
        {
            var canExecute = true;
            if (float.TryParse(UiFrequencyWithoutCwText.text.Replace(".", ","), out var freq) && freq > 0)
                FrequencyWithoutCw = freq;
            else
                canExecute = false;

            if (!string.IsNullOrEmpty(UiCorrectionFactor.text) &&
                float.TryParse(UiCorrectionFactor.text.Replace(".", ","), out var correc) && correc <= 100 &&
                correc >= 0)
            {
                _usingPendulumLength = false;
                CorrectionFactor = correc / 100;
            }
            else if (!string.IsNullOrEmpty(UiPendulumLenght.text) &&
                     float.TryParse(UiPendulumLenght.text.Replace(".", ","), out var pendulum) && pendulum >= 0)
            {
                _usingPendulumLength = true;
                PendulumLength = pendulum;
            }
            else
                canExecute = false;

            _isSimulating = canExecute;
        }

        private float CalculatePendulumLength(float frequency)
        {
            return 9.8f * Mathf.Pow(1 / (2 * Mathf.PI * frequency), 2);
        }
    }
}
