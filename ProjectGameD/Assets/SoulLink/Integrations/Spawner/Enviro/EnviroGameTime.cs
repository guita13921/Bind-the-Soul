#if ENVIRO_LW || ENVIRO_HD

// EnviroGameTime
// An integration with Enviro Sky by Hendrik Haupt. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

using System.Collections;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

namespace Magique.SoulLink
{
    public class EnviroGameTime : BaseGameTime
    {
        private EnviroSkyMgr _sky;
        private EnviroCore _enviroCore;
        private EnviroTime.TimeProgressMode _saveTimeProgressMode;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType <EnviroSkyMgr>();
            if (_sky)
            {
                _currentHour = _sky.Time.Hours;
                _currentMinute = _sky.Time.Minutes;
                GetMonth();
                GetYear();

                _dayLengthInMinutes = _sky.Time.cycleLengthInMinutes;
                SetDayLength(_dayLengthInMinutes);
            } // if

            _enviroCore = FindObjectOfType<EnviroCore>();

            GetDay();
            _days = (uint)_sky.GetCurrentDay();
        } // GetComponents()

        public override float GetHour()
        {
            GetComponents();

            _currentHour = _sky.Time.Hours;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            StartCoroutine(ChangeHour(hour));
        } // SetHour()

        IEnumerator ChangeHour(float hour)
        {
            _sky.Time.ProgressTime = EnviroTime.TimeProgressMode.None;

            yield return new WaitForEndOfFrame();

            _sky.Time.Hours = _currentHour = (int)hour;

            yield return new WaitForEndOfFrame();

            _sky.Time.ProgressTime = EnviroTime.TimeProgressMode.Simulated;
        } // ChangeHour()

        override public void SetMinute(float minute)
        {
            StartCoroutine(ChangeMinute(minute));
        } // SetMinute()

        IEnumerator ChangeMinute(float minute)
        {
            _sky.Time.ProgressTime = EnviroTime.TimeProgressMode.None;

            yield return new WaitForEndOfFrame();

            _sky.Time.Minutes = _currentMinute = (int)minute;

            yield return new WaitForEndOfFrame();

            _sky.Time.ProgressTime = EnviroTime.TimeProgressMode.Simulated;
        } // ChangeMinute()

        public override float GetMinute()
        {
            GetComponents();

            _currentMinute = _sky.Time.Minutes;
            return _currentMinute;
        } // GetMinute()

        override public int GetDay()
        {
            if (!_enviroCore) return 0;

            _days = (uint)_enviroCore.currentDay;
            ConvertDaysToDay();
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_enviroCore) return;

            ConvertDayToDays(value);
            _enviroCore.currentDay = _currentDay;
        } // SetDay()

        public override int GetMonth()
        {
            if (!_sky) return 0;

            _currentMonth = _sky.GetCurrentMonth();
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_enviroCore) return;

            _currentMonth = value;
            _enviroCore.dateTime.SetFieldValue("Month", value);
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.GetCurrentYear();
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_enviroCore) return;

            _enviroCore.currentYear = _currentYear = value;
        } // SetYear()

        public override Season GetSeason()
        {
            if (!_enviroCore) return Season.Winter;

            switch (_enviroCore.Seasons.currentSeasons)
            {
                case EnviroSeasons.Seasons.Spring:
                    return Season.Spring;
                case EnviroSeasons.Seasons.Summer:
                    return Season.Summer;
                case EnviroSeasons.Seasons.Autumn:
                    return Season.Fall;
                case EnviroSeasons.Seasons.Winter:
                    return Season.Winter;
            } // switch

            return Season.Winter;
        } // GetSeason()

        public override void SetSeason(Season value)
        {
            if (!_enviroCore) return;

            switch (value)
            {
                case Season.Spring:
                    _enviroCore.Seasons.currentSeasons = EnviroSeasons.Seasons.Spring;
                    break;
                case Season.Summer:
                    _enviroCore.Seasons.currentSeasons = EnviroSeasons.Seasons.Summer;
                    break;
                case Season.Fall:
                    _enviroCore.Seasons.currentSeasons = EnviroSeasons.Seasons.Autumn;
                    break;
                case Season.Winter:
                    _enviroCore.Seasons.currentSeasons = EnviroSeasons.Seasons.Winter;
                    break;
            } // switch
        } // SetSeason()

        override public void Pause()
        {
            if (_enviroCore == null) return;

            _paused = true;
            _saveTimeProgressMode = _enviroCore.GameTime.ProgressTime;
            _sky.SetTimeProgress(EnviroTime.TimeProgressMode.None);
        } // Pause()

        public override void UnPause()
        {
            _paused = false;
            _sky.SetTimeProgress(_saveTimeProgressMode);
        } // UnPause()

        override protected void FixedUpdate()
        {
            UpdateSecondsAdded();

            if (_sky == null) return;

            _days = (uint)_sky.GetCurrentDay();
        } // FixedUpdate()
    } // class EnviroGameTime
} // namespace Magique.SoulLink
#endif