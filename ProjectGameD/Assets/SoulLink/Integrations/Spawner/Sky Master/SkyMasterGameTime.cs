#if SOULLINK_USE_SKYMASTER

// SkyMasterGameTime
// An integration with Sky Master Ultimate by ArtNGame.
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

using Artngame.SKYMASTER;
using UnityEngine;

namespace Magique.SoulLink
{
    public class SkyMasterGameTime : BaseGameTime
    {
        private SkyMasterManager _sky;
        private float _saveSunSpeed = 0f;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<SkyMasterManager>();
            if (_sky)
            {
                _currentHour = (int)_sky.Current_Time;
                _currentMinute = (int)((_sky.Current_Time - _currentHour) * 60f);

                // Set initial dates because Sky Master doesn't have proper way to set a starting date
                InitStartingDate();

                _dayLengthInMinutes = 1440 / (_sky.SPEED * 60);
                SetDayLength(_dayLengthInMinutes);
            }
        } // GetComponents()

        /// <summary>
        /// GetSoulLinkControlsDate
        /// </summary>
        /// <returns></returns>
        override public bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        /// <summary>
        /// Initialize the starting date for Sky Master based on the current values from the inspector
        /// </summary>
        private void InitStartingDate()
        {
            _sky.Current_Day = (_currentYear * 365) + GetDaysUpToMonth(_currentMonth) + _currentDay;
        } // InitStartingDate()

        public override float GetHour()
        {
            GetComponents();
            if (!_sky) return base.GetHour();

            _currentHour = (int)_sky.Current_Time;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            GetComponents();
            if (_sky)
            {
                _sky.Current_Time = (int)hour + (_currentMinute / 60f);
            } // if

            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            GetComponents();
            if (_sky)
            {
                _sky.Current_Time = _currentHour + (minute / 60f);
            } // if

            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            GetComponents();
            if (!_sky) return base.GetMinute();

            _currentMinute = (int)((_sky.Current_Time - _currentHour) * 60f);
            return _currentMinute;
        } // GetMinute()

        override public int GetDay()
        {
            if (!_sky) return 0;

            _days = (uint)_sky.Current_Day % 365;
            ConvertDaysToDay();
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_sky) return;

            var year = (int)_sky.Current_Day / 365;
            var daysOfYear = (int)(_sky.Current_Day % 365);
            var month = GetMonthFromDays(daysOfYear);

            _currentDay = value;
            _sky.Current_Day = (year * 365) + GetDaysUpToMonth(month) + value;
        } // SetDay()

        public override int GetMonth()
        {
            if (!_sky) return 0;

            // Calulate the month based on days of the year
            var daysOfYear = (int)(_sky.Current_Day % 365);

            _currentMonth = GetMonthFromDays(daysOfYear);
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            var year = (int)_sky.Current_Day / 365;
            var daysOfYear = GetDaysUpToMonth(value);

            _currentMonth = value;
            _sky.Current_Day = (year * 365) + daysOfYear + _currentDay;
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.Current_Day > 365 ? (int)(_sky.Current_Day / 365) : 1;
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _currentYear = value;
            _sky.Current_Day = (value * 365) + GetDaysUpToMonth(_currentMonth) + _currentDay;
        } // SetYear()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _saveSunSpeed = _sky.SPEED;
            _sky.SPEED = 0f;
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.SPEED = _saveSunSpeed;
        } // Pause()

        override protected void FixedUpdate()
        {
            CalculateDays();
            UpdateSeason();
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class SkyMasterGameTime
} // namespace Magique.SoulLink
#endif