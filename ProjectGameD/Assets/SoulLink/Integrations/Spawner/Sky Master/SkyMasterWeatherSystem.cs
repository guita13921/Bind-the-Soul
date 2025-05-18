#if SOULLINK_USE_SKYMASTER

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

using Artngame.SKYMASTER;

namespace Magique.SoulLink
{
    public class SkyMasterWeatherSystem : BaseWeatherSystem
    {
        private SkyMasterManager _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<SkyMasterManager>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null || _weather.currentWeather == null)
            {
                return base.GetSnowStrength();
            } // if

            if (_weather.currentWeather.Snow)
            {
                return _weather.WeatherSeverity / 10f;
            } // if

            return 0f;
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (_weather == null || _weather.currentWeather == null)
            {
                return base.GetWetness();
            } // if

            if (_weather.currentWeather.Rain)
            {
                return _weather.WeatherSeverity / 10f;
            } // if

            return 0f;
        } // GetWetness()
    } // class SkyMasterWeatherSystem()
} // namespace Magique.SoulLink
#endif