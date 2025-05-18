#if WEATHER_MAKER_PRESENT
using DigitalRuby.WeatherMaker;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WeatherMakerWeatherSystem : BaseWeatherSystem
    {
        private WeatherMakerPrecipitationManagerScript _weather;

        override public void GetComponents()
        {
            base.GetComponents();

            if (_weather != null) return;

            _weather = FindObjectOfType<WeatherMakerPrecipitationManagerScript>();
        } // GetComponents()

        override public float GetSnowStrength()
        {
            if (_weather == null)
            {
                return base.GetSnowStrength();
            } // if

            return _weather.SnowIntensity;
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (_weather == null)
            {
                return base.GetWetness();
            } // if

            return _weather.RainIntensity;
        } // GetWetness()
    } // class WeatherMakerWeatherSystem()
} // namespace Magique.SoulLink
#endif