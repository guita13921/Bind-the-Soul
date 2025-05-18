using UnityEngine;
using UnityEngine.UI;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkSpawnerDebug : MonoBehaviour
    {
        [SerializeField]
        private Text _totalSpawnsText;

        [SerializeField]
        private Text _currentTimeText;

        [SerializeField]
        private Text _rainStrengthText;

        [SerializeField]
        private Text _snowStrengthText;

        [SerializeField]
        private Text _temperatureText;

        [SerializeField]
        private Text _dateText;

        [SerializeField]
        private Text _seasonText;

        private BaseWeatherSystem _weather;

        private void Awake()
        {
            _weather = FindObjectOfType<BaseWeatherSystem>();
            if (_weather == null)
            {
                if (_rainStrengthText != null)
                {
                    _rainStrengthText.transform.parent.gameObject.SetActive(false);
                } // if

                if (_snowStrengthText != null)
                {
                    _snowStrengthText.transform.parent.gameObject.SetActive(false);
                } // if

                if (_temperatureText != null)
                {
                    _temperatureText.transform.parent.gameObject.SetActive(false);
                } // if
            } // if
        } // Awake()

        void Update()
        {
            if (_totalSpawnsText != null)
            {
                _totalSpawnsText.text = SoulLinkSpawner.Instance.GetTotalSpawns("").ToString(); 
            } // if

            if (_currentTimeText != null)
            {
                _currentTimeText.text = SoulLinkSpawner.Instance.CurrentTimeString;
            } // if

            if (_dateText != null)
            {
                _dateText.text = SoulLinkSpawner.Instance.CurrentDateString;
            } // if

            if (_seasonText != null)
            {
                _seasonText.text = SoulLinkSpawner.Instance.GameTime.GetSeason().ToString();
            } // if

            if (_weather != null)
            {
                if (_rainStrengthText != null)
                {
                    _rainStrengthText.text = _weather.GetWetness().ToString("F2");
                } // if

                if (_snowStrengthText != null)
                {
                    _snowStrengthText.text = _weather.GetSnowStrength().ToString("F2");
                } // if

                if (_temperatureText != null)
                {
                    _temperatureText.text = _weather.GetTemperature(true).ToString("F2") + (_weather.GetTemperatureScale() == TemperatureScale.Fahrenheit ? " F" : " C");
                } // if
            } // if
        } // Update()
    } // class SoulLinkSpawnerDebug
} // namespace Magique.SoulLink