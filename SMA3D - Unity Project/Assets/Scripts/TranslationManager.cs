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
                {"DISCONNECT", Tuple.Create("DISCONNECT", "DESCONECTAR")},
                {"PENDULUM", Tuple.Create("PENDULUM", "PÊNDULO")},
                {"SIMULATION", Tuple.Create("SIMULATION", "SIMULADOR")},
                {"NATURAL_FREQUENCY", Tuple.Create("Natural frequency (Hz)", "Frequência natural (Hz)")},
                {"CORRECTION_FACTOR", Tuple.Create("Correction factor (%)", "Fator de correção (%)")},
                {"PENDULUM_LENGTH", Tuple.Create("Pendulum length (m)", "Comprimento do pêndulo (m)")},
                {"SIMULATE", Tuple.Create("SIMULATE", "SIMULAR")},
                {"LENGTH", Tuple.Create("LENGTH:", "Comprimento:")},
                {"SOURCE", Tuple.Create("SOURCE CODE", "CÓDIGO FONTE")},
                {"MOTOR_SPEED", Tuple.Create("Motor Speed", "Velocidade do motor")},
                {"REAL", Tuple.Create("REAL", "REAL")},
                {
                    "DESC_SIMULATION",
                    Tuple.Create("DESIGN AND TEST THE INERTIAL COUNTER-COUNTRY SYSTEM",
                        "PROJETE E TESTE O SISTEMA DE CONTRAPESO INERCIAL")
                },
                {
                    "DESC_REAL",
                    Tuple.Create("CONNECT AND SUPERVISION THE BEHAVIOR OF STRUCTURES",
                        "CONECTE E SUPERVISIONE O COMPORTAMENTO DAS ESTRUTURAS")
                },
                {
                    "DESC_SOURCE",
                    Tuple.Create("SOURCE CODE IS AVAILABLE ON GITHUB", "CÓDIGO FONTE DISPONÍVEL NO GITHUB")
                },
                {
                    "DESC_INFO",
                    Tuple.Create(
                        "Students:\nEveraldo Chaves de Oliveira Junior\nGabriel Totola Loyola\nPedro de Oliveira Ramaldes Fafá Borges\n\n\n",
                        "Alunos:\nEveraldo Chaves de Oliveira Junior\nGabriel Totola Loyola\nPedro de Oliveira Ramaldes Fafá Borges\n\n\n")
                },
                {
                    "TITLE",
                    Tuple.Create("Damping monitoring system of a structure through inertial counterweight",
                        "Sistema de Monitoramento do Amortecimento de uma estrutura através de contrapeso inercial")
                },
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
