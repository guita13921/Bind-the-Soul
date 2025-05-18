using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface IWeatherSystem
    {
        void GetComponents();

        float GetWetness();

        float GetSnowStrength();

        float GetTemperature(bool useElevation = true);

        float GetModifiedTemperature(float baseTemp, bool useElevation);

        float GetElevationModifiedTemperature(float currentTemp, float yPos);

        float GetElevationModifiedTemperature(float yPos);

        bool ElevationAffectsTemperature();
        bool WeatherAffectsTemperature();
        bool WindAffectsTemperature();

        bool HasCustomTemperatureCurve();

        bool UsesSeasonBasedTemperature();

        TemperatureScale GetTemperatureScale();

        bool HasTemperatureFeature();
    } // interface IWeatherSystem
} // namespace Magique.SoulLink
