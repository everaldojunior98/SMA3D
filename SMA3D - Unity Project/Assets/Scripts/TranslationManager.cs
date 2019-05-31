using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TranslationManager : MonoBehaviour
    {
        public static TranslationManager Instance;

        //[KEY][EN][PT]
        private Dictionary<string, Tuple<string, string>> _translationsDictionary;
        private string _currentLanguage;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);

                SetupTranslations();

                _currentLanguage = PlayerPrefs.GetString("Language", "EN-US");
            }
            else
            {
                Destroy(this);
            }
        }

        private void SetupTranslations()
        {
            _translationsDictionary = new Dictionary<string, Tuple<string, string>>
            {
                {"FREQUENCY", Tuple.Create("Frequency:", "Frequência:")},
                {"STRUCTURE_WITHOUT", Tuple.Create("STRUCTURE WITHOUT COMPENSATOR", "ESTRUTURA SEM COMPENSADOR")},
                {"STRUCTURE_WITH", Tuple.Create("STRUCTURE WITH COMPENSATOR", "ESTRUTURA COM COMPENSADOR")},
                {"CORRECTION", Tuple.Create("CORRECTION", "CORREÇÃO")},
                {"CONFIGURATION", Tuple.Create("CONFIGURATION", "CONFIGURAÇÃO")},
                {"PORT", Tuple.Create("Port", "Porta")},
                {"CONNECT", Tuple.Create("CONNECT", "CONECTAR")},
                {"FACTOR", Tuple.Create("Factor:", "Fator:")}
            };
        }

        public string GetTranslation(string key)
        {
            var word = "null";
            if (_translationsDictionary.ContainsKey(key))
                word = _currentLanguage == "EN-US"
                    ? _translationsDictionary[key].Item1 : _translationsDictionary[key].Item2;
            return word;
        }

        public void ChangeLanguage(string lang)
        {
            PlayerPrefs.SetString("Language", lang);
            PlayerPrefs.Save();
            _currentLanguage = PlayerPrefs.GetString("Language", "EN-US");
        }
    }
}
