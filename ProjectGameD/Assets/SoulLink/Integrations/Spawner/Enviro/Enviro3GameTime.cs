#if ENVIRO_3

// Enviro3GameTime
// An integration with Enviro 3 Sky by Hendrik Haupt. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

using System.Collections;
using UnityEngine;
using Enviro;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

namespace Magique.SoulLink
{
    public class Enviro3GameTime : BaseGameTime
    {
        private EnviroManager _sky;
        private bool _saveTimeProgressMode;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType <EnviroManager>();
            if (_sky)
            {
                _currentHour = _sky.Time.hours;
                _currentMinute = _sky.Time.minutes;
                GetMonth();
                GetYear();

                _dayLengthInMinutes = _sky.Time.Settings.cycleLengthInMinutes;
                SetDayLength(_dayLengthInMinutes);
            } // if

            GetDay();
            _days = (uint)_sky.Time.days;
        } // GetComponents()

        public override float GetHour()
        {
            GetComponents();

            _currentHour = _sky.Time.hours;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            StartCoroutine(ChangeHour(hour));
        } // SetHour()

        IEnumerator ChangeHour(float hour)
        {
            _sky.Time.Settings.simulate = false;

            yield return new WaitForEndOfFrame();

            _sky.Time.hours = _currentHour = (int)hour;

            yield return new WaitForEndOfFrame();

            _sky.Time.Settings.simulate = true;
        } // ChangeHour()

        override public void SetMinute(float minute)
        {
            StartCoroutine(ChangeMinute(minute));
        } // SetMinute()

        IEnumerator ChangeMinute(float minute)
        {
            _sky.Time.Settings.simulate = false;

            yield return new WaitForEndOfFrame();

            _sky.Time.minutes = _currentMinute = (int)minute;

            yield return new WaitForEndOfFrame();

            _sky.Time.Settings.simulate = true;
        } // ChangeMinute()

        public override float GetMinute()
        {
            GetComponents();

            _currentMinute = _sky.Time.minutes;
            return _currentMinute;
        } // GetMinute()

        override public int GetDay()
        {
            _days = (uint)_sky.Time.days;

            ConvertDaysToDay();
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            ConvertDayToDays(value);
            _sky.Time.days = _currentDay;
        } // SetDay()

        public override int GetMonth()
        {
            if (!_sky) return 0;

            _currentMonth = _sky.Time.months;
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            _currentMonth = value;
            _sky.Time.months = value;
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.Time.years;
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _sky.Time.years = _currentYear = value;
        } // SetYear()

        public override Season GetSeason()
        {
            if (!_sky) return Season.Winter;

            switch (_sky.Environment.Settings.season)
            {
                case EnviroEnvironment.Seasons.Spring:
                    return Season.Spring;
                case EnviroEnvironment.Seasons.Summer:
                    return Season.Summer;
                case EnviroEnvironment.Seasons.Autumn:
                    return Season.Fall;
                case EnviroEnvironment.Seasons.Winter:
                    return Season.Winter;
            } // switch

            return Season.Winter;
        } // GetSeason()

        public override void SetSeason(Season value)
        {
            if (!_sky) return;

            switch (value)
            {
                case Season.Spring:
                    _sky.Environment.ChangeSeason(EnviroEnvironment.Seasons.Spring);
                    break;
                case Season.Summer:
                    _sky.Environment.ChangeSeason(EnviroEnvironment.Seasons.Summer);
                    break;
                case Season.Fall:
                    _sky.Environment.ChangeSeason(EnviroEnvironment.Seasons.Autumn);
                    break;
                case Season.Winter:
                    _sky.Environment.ChangeSeason(EnviroEnvironment.Seasons.Winter);
                    break;
            } // switch
        } // SetSeason()

        override public void Pause()
        {
            if (_sky == null) return;

            _paused = true;
            _saveTimeProgressMode = _sky.Time.Settings.simulate;
        } // Pause()

        public override void UnPause()
        {
            if (_sky == null) return;

            _paused = false;
            _sky.Time.Settings.simulate = _saveTimeProgressMode;
        } // UnPause()

        override protected void FixedUpdate()
        {
            UpdateSecondsAdded();

            if (_sky == null) return;

            _days = (uint)_sky.Time.days;
        } // FixedUpdate()
    } // class Enviro3GameTime
} // namespace Magique.SoulLink
#endif