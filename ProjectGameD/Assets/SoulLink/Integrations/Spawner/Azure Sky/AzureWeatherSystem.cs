#if SOULLINK_USE_AZURE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AzureSky;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class AzureWeatherSystem : BaseWeatherSystem
    {
        private AzureEffectsController _weather;
        private float _wetness = 0f;
        private float _snowIntensity = 0f;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<AzureEffectsController>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return _snowIntensity;
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _wetness;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            if (_weather == null) return;

            float combinedWetness = 0f;

            // For wetness we weight the 3 rain conditions with heavy rain having full weight, medium rain 1/2, and light rain 1/3.
            // Normaalize the value to ~1
            combinedWetness += _weather.lightRainIntensity / 3f;
            combinedWetness += _weather.mediumRainIntensity / 2f;
            combinedWetness += _weather.heavyRainIntensity;

            _wetness = combinedWetness / 1.5f;

            _snowIntensity = _weather.snowIntensity;

            // debug logging to see if things are working as expected
            //            Debug.Log("Wetness       = " + GetWetness());
            //            Debug.Log("Snow Strength = " + GetSnowStrength());
            //            Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()
    } // class AzureWeatherSystem
} // namespace Magique.SoulLink
#endif