using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if MAPMAGIC2
using MapMagic.Core;
using MapMagic.Terrains;
using MapMagic.Products;
#endif

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BaseWeatherSystem : MonoBehaviour, IWeatherSystem
    {
        [SerializeField]
        private bool _customTemperatureCurve = false;

        [SerializeField]
        private float _peakTemperature = 80f;

        [SerializeField]
        private bool _seasonBasedTemperature = true;

        public float FallPeakTemperature
        {
            get { return _fallPeakTemperature; }
            set { _fallPeakTemperature = value; }
        }
        [SerializeField]
        private float _fallPeakTemperature = 60f;

        public float WinterPeakTemperature
        {
            get { return _winterPeakTemperature; }
            set { _winterPeakTemperature = value; }
        }
        [SerializeField]
        private float _winterPeakTemperature = 40f;

        public float SpringPeakTemperature
        {
            get { return _springPeakTemperature; }
            set { _springPeakTemperature = value; }
        }
        [SerializeField]
        private float _springPeakTemperature = 80f;

        public float SummerPeakTemperature
        {
            get { return _summerPeakTemperature; }
            set { _summerPeakTemperature = value; }
        }
        [SerializeField]
        private float _summerPeakTemperature = 100f;

        [SerializeField]
        private AnimationCurve _temperatureCurve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(0.5f, 1f), new Keyframe(0.65f, 1f), new Keyframe(1f, 0.5f));

        [SerializeField]
        private bool _elevationAffectsTemperature = false;

        [SerializeField]
        private AnimationCurve _elevationCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.5f));

        [SerializeField]
        private bool _weatherAffectsTemperature = false;

        [Range(0f, 10f)]
        [SerializeField]
        private float _rainFactor = 5f;

        [Range(0f, 10f)]
        [SerializeField]
        private float _snowFactor = 2f;

        [SerializeField]
        private bool _windAffectsTemperature = false;

        [Range(0f, 5f)]
        [SerializeField]
        private float _windFactor = 5f;

        [SerializeField]
        protected TemperatureScale _temperatureScale = TemperatureScale.Fahrenheit;

        private BaseGameTime _gameTime = null;
        private WindZone _windZone = null;
        private Vector2 _terrainMinMax = Vector2.zero;
        private Transform _playerTransform;

        /// <summary>
        /// Awake
        /// </summary>
        virtual public void Awake()
        {
#if MAPMAGIC2
            if (FindObjectOfType<MapMagicObject>())
            {
                TerrainTile.OnTileApplied += TileApplied;
            }
            else
            {
                _terrainMinMax = Utility.GetTerrainMinMax();
            }
#else
            _terrainMinMax = Utility.GetTerrainMinMax();
#endif
            GetComponents();
        } // Awake()

#if MAPMAGIC2
        void TileApplied(TerrainTile tile, TileData data, StopToken token)
        {
            if (!Application.isPlaying) return;

            _terrainMinMax = Utility.GetTerrainMinMax(tile);
        } // TileApplied()
#endif

        /// <summary>
        /// GetComponents
        /// </summary>
        virtual public void GetComponents()
        {
            _gameTime = FindObjectOfType<BaseGameTime>();
            _windZone = FindObjectOfType<WindZone>();
        } // GetComponents()

        /// <summary>
        /// GetSnowStrength
        /// </summary>
        /// <returns></returns>
        virtual public float GetSnowStrength()
        {
            return 0f;
        } // GetSnowStrength()

        /// <summary>
        /// GetTemperature
        /// </summary>
        /// <param name="useElevation"></param>
        /// <returns></returns>
        virtual public float GetTemperature(bool useElevation = true)
        {
            float currentTemp = 0f;

            if (_customTemperatureCurve && _gameTime != null)
            {
                // Update based on time of day and current weather
                currentTemp = GetPeakTemperature() * _temperatureCurve.Evaluate(_gameTime.GetHour() / 24f);
            } // if

            return GetModifiedTemperature(currentTemp, useElevation);
        } // GetTemperature()

        /// <summary>
        /// Returns the peak temperature; uses season specific temps if activated
        /// </summary>
        /// <returns></returns>
        virtual public float GetPeakTemperature()
        {
            if (!_seasonBasedTemperature || !_gameTime) return _peakTemperature;

            var season = _gameTime.GetSeason();
            switch (season)
            {
                case Season.Fall:
                    return _fallPeakTemperature;
                case Season.Winter:
                    return _winterPeakTemperature;
                case Season.Spring:
                    return _springPeakTemperature;
                case Season.Summer:
                    return _summerPeakTemperature;
            } // switch

            return _peakTemperature;
        } // GetPeakTemperature()

        /// <summary>
        /// Get temperature after modifying it with custom temperature controls (i.e. wind, weather, elevation)
        /// </summary>
        /// <param name="baseTemp"></param>
        /// <param name="useElevation"></param>
        /// <returns></returns>
        virtual public float GetModifiedTemperature(float baseTemp, bool useElevation)
        {
            if (_windAffectsTemperature && _windZone != null)
            {
                baseTemp = baseTemp - (baseTemp * (_windZone.windMain / _windFactor));
            } // if

            if (_weatherAffectsTemperature)
            {
                // Modify by rain and snow amount
                if (GetWetness() > 0f)
                {
                    baseTemp = baseTemp - (baseTemp * (GetWetness() / _rainFactor));
                } // if

                if (GetSnowStrength() > 0f)
                {
                    baseTemp = baseTemp - (baseTemp * (GetSnowStrength() / _snowFactor));
                } // if
            } // if

#if SOULLINK_SPAWNER
            _playerTransform = SoulLinkSpawner.Instance.PlayerTransform;
#elif SOULLINK_AI
            _playerTransform = SoulLinkManager.Instance.Player.transform;
#endif

            return baseTemp;
        } // GetModifiedTemperature()

        /// <summary>
        /// GetElevationModifiedTemperature
        /// </summary>
        /// <param name="currentTemp"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        virtual public float GetElevationModifiedTemperature(float currentTemp, float yPos)
        {
            float t = (yPos - _terrainMinMax.x) / (_terrainMinMax.y - _terrainMinMax.x);
            float modifier = 1f - _elevationCurve.Evaluate(t);
            return currentTemp - (Mathf.Abs(currentTemp) * modifier);
        } // GetElevationModifiedTemperature()

        /// <summary>
        /// GetElevationModifiedTemperature
        /// </summary>
        /// <param name="yPos"></param>
        /// <returns></returns>
        virtual public float GetElevationModifiedTemperature(float yPos)
        {
            float t = (yPos - _terrainMinMax.x) / (_terrainMinMax.y - _terrainMinMax.x);
            float modifier = 1f - _elevationCurve.Evaluate(t);
            float currentTemp = GetTemperature(false);
            return currentTemp - (Mathf.Abs(currentTemp) * modifier);
        } // GetElevationModifiedTemperature()

        /// <summary>
        /// ElevationAffectsTemperature
        /// </summary>
        /// <returns></returns>
        virtual public bool ElevationAffectsTemperature()
        {
            return _elevationAffectsTemperature;
        } // ElevationAffectsTemperature()

        /// <summary>
        /// WeatherAffectsTemperature
        /// </summary>
        /// <returns></returns>
        virtual public bool WeatherAffectsTemperature()
        {
            return _weatherAffectsTemperature;
        } // WeatherAffectsTemperature()

        /// <summary>
        /// WindAffectsTemperature
        /// </summary>
        /// <returns></returns>
        virtual public bool WindAffectsTemperature()
        {
            return _windAffectsTemperature;
        } // WindAffectsTemperature()

        /// <summary>
        /// HasCustomTemperatureCurve
        /// </summary>
        /// <returns></returns>
        virtual public bool HasCustomTemperatureCurve()
        {
            return _customTemperatureCurve;
        } // HasCustomTemperatureCurve()


        /// <summary>
        /// Returns true if the weather system uses season based peak temperatures
        /// </summary>
        /// <returns></returns>
        virtual public bool UsesSeasonBasedTemperature()
        {
            return _seasonBasedTemperature;
        } // UsesSeasonBasedTemperature()

        /// <summary>
        /// GetWetness
        /// </summary>
        /// <returns></returns>
        virtual public float GetWetness()
        {
            return 0f;
        } // GetWetness()

        /// <summary>
        /// GetTemperatureScale
        /// </summary>
        /// <returns></returns>
        virtual public TemperatureScale GetTemperatureScale()
        {
            return _temperatureScale;
        } //GetTemperatureScale()

        /// <summary>
        /// HasTemperatureFeature
        /// </summary>
        /// <returns></returns>
        virtual public bool HasTemperatureFeature()
        {
            return false;
        } // HasTemperatureFeature()

        /// <summary>
        /// UpdateWeatherSystem
        /// </summary>
        virtual protected void UpdateWeatherSystem()
        {
            // inherited classes must implement this as needed
        } // UpdateWeatherSystem()

        /// <summary>
        /// FixedUpdate
        /// </summary>
        virtual protected void FixedUpdate()
        {
            UpdateWeatherSystem();
        } // FixedUpdate()
    } // class baseWeatherSystem
} // namespace Magique.SoulLink