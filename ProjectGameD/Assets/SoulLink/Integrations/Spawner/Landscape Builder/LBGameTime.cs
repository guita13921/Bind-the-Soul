#if LANDSCAPE_BUILDER
using LandscapeBuilder;
using UnityEngine;

// LBGameTime
// A preliminary integration with Landscape Builder's LBLighting system by SCSM. This script does not update the day, month, or year when time progresses.
// It only updates the minute and hour. Additional work is required to roll over dates.
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class LBGameTime : BaseGameTime
    {
        private LBLighting _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<LBLighting>();
            if (_sky)
            {
                _dayLengthInMinutes = _sky.dayLength / 60;

                GetHour();
                GetMinute();

                SetDayLength(_dayLengthInMinutes);
            } // if
        } // GetComponents()

        public override bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        public override float GetHour()
        {
            GetComponents();

            _currentHour = (int)((_sky.CurrentTime() / _sky.dayLength) * 24f);
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            GetComponents();

            // dayTimer is the value that time calculations are based on (dayTimer % dayLength is CurrentTime())

            var hourValue = hour + (_currentMinute / 60f);
            var currentTime = (hourValue / 24) * _sky.dayLength;

            _sky.SetFieldValue("dayTimer", currentTime);

            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            GetComponents();

            var hourValue = _currentHour + (minute / 60f);
            var currentTime = (hourValue / 24) * _sky.dayLength;

            _sky.SetFieldValue("dayTimer", currentTime);

            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            GetComponents();

            float timeOfDay = (_sky.CurrentTime() / _sky.dayLength) * 24f;
            _currentMinute = (int)((timeOfDay % 1f) * 60f);

            return _currentMinute;
        } // GetMinute()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _sky.PauseLighting();
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.ResumeLighting();
        } // Pause()

        override protected void FixedUpdate()
        {
            // Update the day, month, year since LB does not do this for itself
            UpdateDates();

            CalculateDays();
            UpdateSeason();

            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class LBGameTime()
} // namespace Magique.SoulLink
#endif