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

        public float lerpSpeed = 30f;

        //Transforms to set oscillation position
        public Transform TableTransform;
        public Transform BuildingWithoutCwTransform;
        public Transform BuildingWithCwTransform;

        public Transform PendulumTransform;

        //Coefficient of oscillation of the material of the structure
        public float OscillationCoefficient = 20;

        //Table max amplitude
        private float _amplitude = 1.3f;

        //Var to control connection with Arduino
        private bool _isConnected;


        private float _accelWithout;
        private float _accelWith;
        private float _tableSpeed;

        private float _frequencyWithoutCw;
        private float _frequencyWithCw;
        private float _correctionFactor;

        //Correction vars
        private int i = 1;
        private float lastWith;
        private float lastWithout;

        //Frequency vars
        /*private int inversionWithCw;
        private float timeWithCw;
        private float lastValueWithCw;

        private int inversionWithoutCw;
        private float timeWithoutCw;
        private float lastValueWithoutCw;*/

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

            if (Math.Abs(accelWithCw) > lastWith)
                lastWith = Math.Abs(accelWithCw);

            if (Math.Abs(accelWithoutCw) > lastWithout)
                lastWithout = Math.Abs(accelWithoutCw);

            if (i % 100 == 0)
            {
                _correctionFactor = 1 - lastWith / lastWithout;
                lastWith = 0;
                lastWithout = 0;
            }

            /*if (accelWithCw < 0 && lastValueWithCw > 0)
                inversionWithCw++;
            else if (accelWithCw > 0 && lastValueWithCw < 0)
                inversionWithCw++;

            lastValueWithCw = accelWithCw;

            if (accelWithoutCw < 0 && lastValueWithoutCw > 0)
                inversionWithoutCw++;
            else if (accelWithoutCw > 0 && lastValueWithoutCw < 0)
                inversionWithoutCw++;

            lastValueWithoutCw = accelWithoutCw;*/

            i++;
        }

        void Update()
        {
            if (!_isConnected)
                return;

            /*//Calculates de frequency with cw
            if (inversionWithCw == 2)
            {
                _frequencyWithCw = 1 / (Time.time - timeWithCw);

                inversionWithCw = 0;
                timeWithCw = Time.time;
            }

            //Calculates de frequency without cw
            if (inversionWithoutCw == 2)
            {
                _frequencyWithoutCw = 1 / (Time.time - timeWithoutCw);

                inversionWithoutCw = 0;
                timeWithoutCw = Time.time;
            }*/

            //Fill HUD
            BuildingWithoutCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " +
                                         _frequencyWithoutCw.ToString("F2") + " Hz";

            BuildingWithCwText.text = TranslationManager.Instance.GetTranslation("FREQUENCY") + " " +
                                      _frequencyWithCw.ToString("F2") + " Hz";

            CorrectionFactorText.text = TranslationManager.Instance.GetTranslation("FACTOR") + " " +
                                        (float.IsInfinity(_correctionFactor) || float.IsNaN(_correctionFactor) || _correctionFactor < 0
                                            ? "0.00"
                                            : (_correctionFactor * 100).ToString("F2")) + "%";

            if (TableTransform != null && BuildingWithoutCwTransform != null && BuildingWithCwTransform != null)
            {
                TableTransform.position = Vector3.Lerp(TableTransform.position,
                    new Vector3(_accelWithout * _amplitude, TableTransform.position.y, TableTransform.position.z),
                    lerpSpeed * Time.deltaTime);


                PendulumTransform.rotation = Quaternion.Lerp(PendulumTransform.rotation,
                    Quaternion.Euler(new Vector3(PendulumTransform.eulerAngles.x, PendulumTransform.eulerAngles.y,
                        -OscillationCoefficient * _accelWithout * 2.5f)), lerpSpeed * Time.deltaTime);

                BuildingWithoutCwTransform.rotation = Quaternion.Lerp(BuildingWithoutCwTransform.rotation,
                    Quaternion.Euler(new Vector3(BuildingWithoutCwTransform.eulerAngles.x,
                        BuildingWithoutCwTransform.eulerAngles.y, -90 - OscillationCoefficient * _accelWithout)), lerpSpeed * Time.deltaTime);


                BuildingWithCwTransform.rotation = Quaternion.Lerp(BuildingWithCwTransform.rotation,
                    Quaternion.Euler(new Vector3(BuildingWithCwTransform.eulerAngles.x,
                        BuildingWithCwTransform.eulerAngles.y, -90 - OscillationCoefficient * _accelWith)), lerpSpeed * Time.deltaTime);
            }
        }
    }
}
