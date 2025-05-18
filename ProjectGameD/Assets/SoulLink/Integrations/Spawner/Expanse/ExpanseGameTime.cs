#if SOULLINK_USE_EXPANSE

// ExpanseGameTime
// An integration with Expanse Volumetric Skies by Three M's Creative. 
//
// To use with Dialogue System or Quet Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

using Expanse;
using UnityEngine;

namespace Magique.SoulLink
{
    public class ExpanseGameTime : BaseGameTime
    {
        private DateTimeController _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<DateTimeController>();
            if (_sky)
            {
                _sky.m_timeLocal.hour = _currentHour;
                _sky.m_timeLocal.minute = _currentMinute;
                _sky.m_timeLocal.day = _currentDay;
                _sky.m_timeLocal.month = _currentMonth;
                _sky.m_timeLocal.year = _currentYear;

                _lastHourCheck = _currentHour;
            } // if
        } // GetComponents()

        override public bool GetSoulLinkControlsTime()
        {
            return true;
        } // GetSoulLinkControlsTime()

        override public bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        public override float GetHour()
        {
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            _sky.m_timeLocal.hour = (int)hour;
            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            _sky.m_timeLocal.minute = (int)minute;
            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            return _currentMinute;
        } // GetMinute()

        /// <summary>
        /// Returns the current day of the month
        /// </summary>
        /// <returns></returns>
        override public int GetDay()
        {
            return _currentDay;
        } // GetDay()

        /// <summary>
        /// Sets the current day of the month
        /// </summary>
        /// <param name="value"></param>
        override public void SetDay(int value)
        {
            _currentDay = _sky.m_timeLocal.day = value;
        } // SetDay(

        /// <summary>
        /// Returns the current month
        /// </summary>
        /// <returns></returns>
        override public int GetMonth()
        {
            return _currentMonth;
        } // GetMonth()

        /// <summary>
        /// Sets the current month
        /// </summary>
        /// <param name="value"></param>
        override public void SetMonth(int value)
        {
            _currentMonth = _sky.m_timeLocal.month = value;
        } // SetMonth()

        /// <summary>
        /// Returns the current year
        /// </summary>
        /// <returns></returns>
        override public int GetYear()
        {
            return _currentYear;
        } // GetYear()

        /// <summary>
        /// Sets the current year
        /// </summary>
        /// <param name="value"></param>
        override public void SetYear(int value)
        {
            _currentYear = _sky.m_timeLocal.year = value;
        } // SetYear()

        override public void Pause()
        {
            _paused = true;
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
        } // Pause()

        override protected void FixedUpdate()
        {
            if (_paused) return;

            // Expanse requires the time and dates to be updated manually so we do that here
            UpdateGameTime();

            UpdateSecondsAdded();

            SetHour(_currentHour);
            SetMinute(_currentMinute);

            UpdateDates();
            CalculateDays();
            UpdateSeason();
        } // FixedUpdate()
    } // class ExpanseGameTime
} // namespace Magique.SoulLink
#endif
