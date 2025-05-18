#if COZY_WEATHER

// CozyGameTime
// An integration with COZY: Stylized Weather 2 System by Distant Lands. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

using DistantLands.Cozy;
using UnityEngine;

namespace Magique.SoulLink
{
    public class CozyGameTime : BaseGameTime
    {
        private CozyWeather _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = CozyWeather.instance;
            if (_sky)
            {
                _sky.perennialProfile.pauseTime = false;

                _currentHour = _sky.currentTime.hours;
                _currentMinute = _sky.currentTime.minutes;

                _dayLengthInMinutes = CozyWeather.instance.GetCurrentDayPercentage() * 1440;
                SetDayLength(_dayLengthInMinutes);
            } // if
        } // GetComponents()

        public override float GetHour()
        {
            if (!_sky) return base.GetHour();

            _currentHour = _sky.currentTime.hours;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            SetCozyTime(hour);
            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            SetCozyTime(_currentHour + (minute / 60));
            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            if (!_sky) return base.GetMinute();

            _currentMinute = _sky.currentTime.minutes;
            return _currentMinute;
        } // GetMinute()

        void SetCozyTime(float time)
        {
            CozyWeather.instance.currentTicks = (time / 24f) * (CozyWeather.instance.perennialProfile.ticksPerDay);
        } // SetCozyTime

        override public int GetDay()
        {
            GetComponents();

            if (!_sky) return 0;

            _currentDay = _sky.currentDay;
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_sky) return;

            _sky.currentDay = value;
        } // SetDay()

        public override int GetMonth()
        {
            GetComponents();

            if (!_sky) return 0;

            _currentMonth = Mathf.FloorToInt(CozyWeather.instance.GetCurrentYearPercentage() * 12) + 1;
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            CozyWeather.instance.currentDay = value * (CozyWeather.instance.perennialProfile.daysPerYear / 12);
            _currentMonth = value;
        } // SetMonth()

        public override int GetYear()
        {
            GetComponents();

            if (!_sky) return 0;

            _currentYear = _sky.currentYear;
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _currentYear = value;
            _sky.currentYear = value;
        } // SetYear()

        override public void Pause()
        {
            _paused = true;

            if (!_sky) return;

            _sky.perennialProfile.pauseTime = true;
        } // Pause()

        public override void UnPause()
        {
            _paused = false;

            if (!_sky) return;

            _sky.perennialProfile.pauseTime = false;
        } // UnPause()

        override protected void FixedUpdate()
        {
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class CozyGameTime
} // namespace Magique.SoulLink
#endif