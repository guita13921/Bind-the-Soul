#if SURVIVAL_ENGINE || FARMING_ENGINE

#if SURVIVAL_ENGINE
using SurvivalEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

using System.Collections;
using UnityEngine;

// SurvivalEngineGameTime
// An integration with Survival Engine and Farming Engine by Indie Marc. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SurvivalEngineGameTime : BaseGameTime
    {
        private TheGame _sky;
        private PlayerData _playerData;
        private bool _isInitialized = false;
        private float _saveSpeedMult = 0f;

        override public void GetComponents()
        {
            if (_sky != null) return;

            // Run initialize as a co-routine because PlayerData doesn't exist imediately
            StartCoroutine(Initialize());
        } // GetComponents()

        IEnumerator Initialize()
        {
            while (_playerData == null)
            {
                _playerData = PlayerData.Get();
                yield return new WaitForEndOfFrame();
            } // while

            _sky = FindObjectOfType<TheGame>();
            if (_sky)
            {
                _currentHour = (int)_playerData.day_time;
                _currentMinute = (int)((_playerData.day_time - _currentHour) * 60f);

                _dayLengthInMinutes = _sky.GetGameTimeSpeed() / 10f;
                SetDayLength(_dayLengthInMinutes);
            } // if

            _isInitialized = true;
        } // Initialize()

        public override bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        public override float GetHour()
        {
            if (!_isInitialized) return 0f;

            _currentHour = (int)_playerData.day_time;

            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            if (!_isInitialized) return;

            _currentMinute = (int)((_playerData.day_time - _currentHour) * 60f);
            _playerData.day_time = hour + (_currentMinute / 60f);

            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            if (!_isInitialized) return;

            _currentHour = (int)_playerData.day_time;
            _playerData.day_time = _currentHour + (minute / 60f);
            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            if (!_isInitialized) return 0f;

            _currentMinute = (int)((_playerData.day_time - _currentHour) * 60f);

            return _currentMinute;
        } // GetMinute()

        override public void Pause()
        {
            if (!_sky) return;

            _paused = true;

            _saveSpeedMult = _sky.GetFieldValue<float>("speed_multiplier");
            _sky.SetFieldValue("speed_multiplier", 0f);
        } // Pause()

        public override void UnPause()
        {
            if (!_sky) return;

            _paused = false;
            _sky.SetFieldValue("speed_multiplier", _saveSpeedMult);
        } // UnPause()

        override protected void FixedUpdate()
        {
            if (!_isInitialized) return;

            UpdateDates();

            CalculateDays();
            UpdateSeason();

            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class SurvivalEngineGameTime()
} // namespace Magique.SoulLink
#endif
