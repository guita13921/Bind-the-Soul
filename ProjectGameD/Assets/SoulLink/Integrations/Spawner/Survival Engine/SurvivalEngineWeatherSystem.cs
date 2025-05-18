#if SURVIVAL_ENGINE || FARMING_ENGINE
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if SURVIVAL_ENGINE
using SurvivalEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
public class SurvivalEngineWeatherSystem : BaseWeatherSystem
    {
        private WeatherSystem _weather;
#if SURVIVAL_ENGINE
        private PlayerCharacterHeat _heat;
#endif

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<WeatherSystem>();
#if SURVIVAL_ENGINE
            _heat = FindObjectOfType<PlayerCharacterHeat>();
#endif
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return 0f; // snow not supported

            // NOTE: To enable Snow, add Snow to the WeatherEffect enum, remove the return 0f code above, and uncomment the following line:
            //return _weather.HasWeatherEffect(WeatherEffect.Snow) ? 1f : 0f;
        } // GetSnowStrength()

        override public bool HasTemperatureFeature()
        {
            return true;
        } // HasTemperatureFeature()

        override public float GetTemperature(bool useElevation = true)
        {
#if SURVIVAL_ENGINE
            if (_heat == null)
            {
                return base.GetTemperature();
            } // if

            float tempValue = _heat.global_heat;
            tempValue = GetModifiedTemperature(tempValue, useElevation);

            // Normally Survival Engine is in Celsius, but if the user has selected Fahrenheit then convert the value before returning it
            if (_temperatureScale == TemperatureScale.Fahrenheit)
            {
                return (tempValue) * (9f / 5f) + 32f;
            } // if

            return _heat.global_heat;
#else
            return base.GetTemperature();
#endif
        } // GetTemperature()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _weather.HasWeatherEffect(WeatherEffect.Rain) ? 1f : 0f;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            // debug logging to see if things are working as expected
//            Debug.Log("Wetness       = " + GetWetness());
//            Debug.Log("Snow Strength = " + GetSnowStrength());
//            Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()

    } // class SurvivalEngineWeatherSystem
} // namespace Magique.SoulLink
#endif
