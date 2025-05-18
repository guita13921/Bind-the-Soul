using UnityEngine;
using System.Collections;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class WeatherRuleData
    {
        public WeatherCondition weatherCondition;
        public FilterRule weatherRule;
        public float threshold = 0.5f;
        public float minTemperature = -50f;
        public float maxTemperature = 100f;

        public void CopyFrom(WeatherRuleData source)
        {
            weatherCondition = source.weatherCondition;
            weatherRule = source.weatherRule;
            threshold = source.threshold;
            minTemperature = source.minTemperature;
        } // CopyFrom()
    } // class WeatherRuleData
} // namespace Magique.SoulLink

