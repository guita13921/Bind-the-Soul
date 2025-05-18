#if COZY_WEATHER

using DistantLands.Cozy;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// CozyWeatherSystem
// An integration with COZY: Stylized Weather 2 System by Distant Lands. 
//
// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class CozyWeatherSystem : BaseWeatherSystem
    {
        private CozyWeather _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = CozyWeather.instance;
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return CozyWeather.instance.cozyMaterials.snowAmount;
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

            float tempValue = (_temperatureScale == TemperatureScale.Celsius) ? _weather.currentTemperatureCelsius : _weather.currentTemperature;
            tempValue = GetModifiedTemperature(tempValue, useElevation);

            return tempValue;
        } // GetTemperature()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return CozyWeather.instance.cozyMaterials.wetness;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            // debug logging to see if things are working as expected
            //            Debug.Log("Wetness       = " + GetWetness());
            //            Debug.Log("Snow Strength = " + GetSnowStrength());
            //            Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()

    } // class CozyWeatherSystem
} // namespace Magique.SoulLink
#endif