using UnityEngine;

// SoulLinkGameTime
// A very basic day/night time management solution. Not intended to be used in a real game, but to serve as an
// example. It is recommended that you use a full solution such as the following:
//
// Time of Day by André Straubmeier
// Azure Sky by 7Stars
// Unistorm by Black Horizon Studios
// Enviro by Hendrik Haupt 
// Weather Maker by Digital Ruby
// Landscape Builder by SCSM
// Jupiter by Pinwheel Studio
// Gaia Procedural Sky by Procedural Worlds
// Tenkoku by Tankuki Digital
// Survival Engine by Indie Marc
//
// Integrations for the above assets are provided with SoulLink.
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2020-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/SoulLink Game Time")]
    public class SoulLinkGameTime : BaseGameTime
    {
        [Tooltip("Only assign a light if your scene is outdoors and needs day/night cycle animation")]
        [SerializeField]
        private Light _light;

        [Tooltip("The intensity of the light when the sun is out")]
        [SerializeField]
        private float _sunIntensity = 1.2f;

        [Tooltip("The intensity of the light when the moon is out")]
        [SerializeField]
        private float _moonIntensity = 0.5f;

        [Tooltip("The starting hour")]
        [SerializeField]
        private float _startingHour = 12f;

        private bool _updateLighting = true;
        private float _degreesPerHour;  // calculated in Awake

        /// <summary>
        /// Awake
        /// </summary>
        override public void Awake()
        {
            _degreesPerHour = (180f / 12f);

            _updateLighting = (_light != null);

            if (_updateLighting)
            {
                _light.intensity = IsDaytime() ? _sunIntensity : _moonIntensity;
            }

            SetHour(_startingHour);
        } // Awake()

        /// <summary>
        /// GetSoulLinkControlsTime
        /// </summary>
        /// <returns></returns>
        override public bool GetSoulLinkControlsTime()
        {
            return true;
        } // GetSoulLinkControlsTime()

        /// <summary>
        /// SetHour
        /// </summary>
        /// <param name="hour"></param>
        override public void SetHour(float hour)
        {
            base.SetHour(hour);
            base.SetMinute((_currentHour - _currentHour) * 60f);

            if (_updateLighting)
            {
                UpdateLightRotation();
                UpdateSunMoonIntensity();
            } // if
        } // SetHour()

        /// <summary>
        /// Updates the light's rotation based on current hour
        /// </summary>
        void UpdateLightRotation()
        {
            float rot = 0f;
            float hour = _currentHour + (_currentMinute / 60f);
            if (hour >= 6f && hour <= 18f)
            {
                float hourNorm = hour - 6f;
                rot = 0f + (hourNorm * _degreesPerHour);
            }
            else if (hour > 18f && hour <= 24f)
            {
                float hourNorm = hour - 18f;
                rot = 0f + (hourNorm * _degreesPerHour);
            }
            else if (hour >= 0f && hour < 6f)
            {
                rot = 0f + (hour * _degreesPerHour);
            }

            _light.transform.eulerAngles = new Vector3(rot, 0f, 0f);
        } // UpdateLightRotation()

        /// <summary>
        /// UpdateSunMoonIntensity
        /// </summary>
        void UpdateSunMoonIntensity()
        {
            _light.intensity = IsDaytime() ? _sunIntensity : _moonIntensity;
        } // UpdateSunMoonRotation()

        /// <summary>
        /// Returns true if the current time is daytime, false if nighttime
        /// </summary>
        /// <returns></returns>
        bool IsDaytime()
        {
            return (_currentHour >= 6f && _currentHour <= 18f);
        } // IsDaytime()

        /// <summary>
        /// FixedUpdate
        /// </summary>
        override protected void FixedUpdate()
        {
            UpdateGameTime();
            UpdateDates();
            CalculateDays();
            UpdateSeason();
            UpdateSecondsAdded();

            if (_updateLighting)
            {
                UpdateLightRotation();
                UpdateSunMoonIntensity();
            } // if
        } // FixedUpdate()
    } // class SoulLinkGameTime
} // namepace Magique.SoulLink