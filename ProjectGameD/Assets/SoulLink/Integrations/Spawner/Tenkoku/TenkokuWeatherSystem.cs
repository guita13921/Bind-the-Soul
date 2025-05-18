#if SOULLINK_USE_TENKOKU
using Tenkoku.Core;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class TenkokuWeatherSystem : BaseWeatherSystem
    {
        private TenkokuModule _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<TenkokuModule>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return _weather.weather_SnowAmt;
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

            float tempValue = _weather.weather_temperature;
            tempValue = GetModifiedTemperature(tempValue, useElevation);

            // Normally Tenkoku is in Fahrenheit, but if the user has selected Celsius then convert the value before returning it
            if (_temperatureScale == TemperatureScale.Celsius)
            {
                return (tempValue - 32f) * (5f / 9f);
            } // if

            return tempValue;
        } // GetTemperature()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _weather.weather_RainAmt;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            // debug logging to see if things are working as expected
            //            Debug.Log("Wetness       = " + GetWetness());
            //            Debug.Log("Snow Strength = " + GetSnowStrength());
            //            Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()

    } // class TenkokuWeatherSystem
} // namespace Magique.SoulLink
#endif