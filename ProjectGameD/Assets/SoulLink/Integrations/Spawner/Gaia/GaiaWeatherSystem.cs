#if GAIA_PRO_PRESENT
using Gaia;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class GaiaWeatherSystem : BaseWeatherSystem
    {
        override public float GetSnowStrength()
        {
            if (GaiaAPI.IsSnowing())
            {
                ProceduralWorldsGlobalWeather weather = ProceduralWorldsGlobalWeather.Instance;
                return weather.SnowIntensity;
            } // if
            return 0f;
        } // GetSnowStrength()

        override public float GetWetness()
        {
            if (GaiaAPI.IsRaining())
            {
                ProceduralWorldsGlobalWeather weather = ProceduralWorldsGlobalWeather.Instance;
                return weather.RainIntensity;
            } // if
            return 0f;
        } // GetWetness()
    } // class GaiaWeatherSystem
} // namespace Magique.SoulLink
#endif