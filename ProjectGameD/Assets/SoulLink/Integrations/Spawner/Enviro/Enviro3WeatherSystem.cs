#if ENVIRO_3
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using Enviro;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Enviro3WeatherSystem : BaseWeatherSystem
    {
        private EnviroManager _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<EnviroManager>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return _weather.Environment.Settings.snow;
        } // GetSnowStrength()

        override public bool HasTemperatureFeature()
        {
            return true;
        } // HasTemperatureFeature()

        override public float GetTemperature(bool useElevation = true)
        {
            if (_weather == null)
            {
                return base.GetTemperature();
            } // if

            float tempValue = _weather.Environment.Settings.temperature;

            // Additional temperature controls if enabled
            tempValue = GetModifiedTemperature(tempValue, useElevation);

            // Normally Enviro is in Celsius, but if the user has selected Fahrenheit then convert the value before returning it
            if (_temperatureScale == TemperatureScale.Fahrenheit)
            {
                return (tempValue) * (9f / 5f) + 32f;
            } // if

            return _weather.Environment.Settings.temperature;
        } // GetTemperature()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _weather.Environment.Settings.wetness;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            // debug logging to see if things are working as expected
//            Debug.Log("Wetness       = " + GetWetness());
//            Debug.Log("Snow Strength = " + GetSnowStrength());
//            Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()

    } // class Enviro3WeatherSystem
} // namespace Magique.SoulLink
#endif