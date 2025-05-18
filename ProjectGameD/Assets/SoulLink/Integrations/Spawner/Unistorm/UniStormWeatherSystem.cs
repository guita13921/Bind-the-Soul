#if UNISTORM_PRESENT
using UniStorm;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class UniStormWeatherSystem : BaseWeatherSystem
    {
        private UniStormSystem _weather;

        override public void GetComponents()
        {
            if (_weather != null) return;

            _weather = FindObjectOfType<UniStormSystem>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            if (Shader.GetGlobalFloat("_SnowStrength") > 0)
            {
                return Shader.GetGlobalFloat("_SnowStrength");
            } // if

            return base.GetSnowStrength();
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            if (Shader.GetGlobalFloat("_WetnessStrength") > 0)
            {
                return Shader.GetGlobalFloat("_WetnessStrength");
            } // if

            return base.GetWetness();
        } // GetWetness()

        public override bool HasTemperatureFeature()
        {
            return true;
        } // HasTemperatureFeature()

        override public float GetTemperature(bool useElevation = true)
        {
            if (_weather != null)
            {
                return _weather.Temperature;
            } // if

            var tempValue = base.GetTemperature(useElevation);
            tempValue = GetModifiedTemperature(tempValue, useElevation);

            // Normally UniStorm is in Fahrenheit, but if the user has selected Celsius then convert the value before returning it
            if (_temperatureScale == TemperatureScale.Celsius)
            {
                return (tempValue - 32f) * (5f / 9f);
            } // if

            return tempValue;
        } // GetTemperature()
    } // class UniStormWeatherSystem
} // namespace Magique.SoulLink
#endif