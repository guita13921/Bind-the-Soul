using System.Collections;
using UnityEngine;

// SoulLinkWeatherSystem
// A very basic weather system for demo purposes only. This is not intended to be used in a real game, but to serve as an
// example. It is recommended that you use a full solution such as the following:
//
// Azure Sky by 7Stars
// Unistorm by Black Horizon Studios
// Enviro by Hendrik Haupt 
// Weather Maker by Digital Ruby
// Landscape Builder by SCSM
// Gaia 2/Gaia Pro by Procedural Worlds
//
// Integrations for the above assets are provided with SoulLink.

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkWeatherSystem : BaseWeatherSystem
    {
        public enum WeatherEvent
        {
            Clear,
            Rain,
            Snow
        }

        [SerializeField]
        private GameObject _rainSystem;

        [SerializeField]
        private GameObject _snowSystem;

        [SerializeField]
        private bool _cycleWeather = true;

        [SerializeField]
        private float _weatherEventDuration = 45f; // in seconds

        private int _currentWeatherIndex = 0;
        private float _rainStrength = 0f;
        private float _snowStrength = 0f;

        /// <summary>
        /// Awake
        /// </summary>
        override public void Awake()
        {
            base.Awake();

            // Turn off weather particle effects by default
            if (_rainSystem != null)
            {
                _rainSystem.SetActive(false);
            } // if

            if (_snowSystem != null)
            {
                _snowSystem.SetActive(false);
            } // if
        } // Awake()

        /// <summary>
        /// Start
        /// </summary>
        void Start()
        {
            if (_cycleWeather)
            {
                StartCoroutine(RandomizeWeather());
            } // if
        } // Start()

        /// <summary>
        /// RandomizeWeather
        /// </summary>
        /// <returns></returns>
        IEnumerator RandomizeWeather()
        {
            while (true)
            {
                yield return new WaitForSeconds(_weatherEventDuration);

                // Cycle to next weather event
                ++_currentWeatherIndex;

                if (_currentWeatherIndex > 2) _currentWeatherIndex = 0;

                if (_rainSystem != null)
                {
                    _rainSystem.SetActive(false);
                } // if
                if (_snowSystem != null)
                {
                    _snowSystem.SetActive(false);
                } // if

                _rainStrength = 0f;
                _snowStrength = 0f;

                switch (_currentWeatherIndex)
                {
                    case (int)WeatherEvent.Clear:
                        break;
                    case (int)WeatherEvent.Rain:
                        if (_rainSystem != null)
                        {
                            _rainSystem.SetActive(true);
                        } // if
                        _rainStrength = 0.5f;
                        break;
                    case (int)WeatherEvent.Snow:
                        if (_snowSystem != null)
                        {
                            _snowSystem.SetActive(true);
                        } // if
                        _snowStrength = 0.5f;
                        break;
                } // switch
            } // while
        } // RandomizeWeather()

        /// <summary>
        /// GetSnowStrength
        /// </summary>
        /// <returns></returns>
        override public float GetSnowStrength()
        {
            return _snowStrength;
        } // GetSnowStrength()

        /// <summary>
        /// GetWetness
        /// </summary>
        /// <returns></returns>
        override public float GetWetness()
        {
            return _rainStrength;
        } // GetWetness()
    } // class SulLinkWeatherSystem
} // Magique.SoulLink