#if LANDSCAPE_BUILDER
using LandscapeBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class LBWeatherSystem : BaseWeatherSystem
    {
        private LBLighting _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<LBLighting>();
            if (_weather)
            {
            } // if
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return _weather.CurrentSnowStrength;
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _weather.CurrentRainStrength;
        } // GetWetness()

        override protected void UpdateWeatherSystem()
        {
            // debug logging to see if things are working as expected
            //Debug.Log("Wetness       = " + GetWetness());
            //Debug.Log("Snow Strength = " + GetSnowStrength());
            //Debug.Log("Temperature   = " + GetTemperature());
        } // UpdateWeatherSystem()

    } // class LBWeatherSystem
} // namespace Magique.SoulLink
#endif