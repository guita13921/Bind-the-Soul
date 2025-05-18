#if JUPITER
using Pinwheel.Jupiter;

// JupiterGameTime
// An integration with Jupiter by Pinwheel Studio. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class JupiterGameTime : BaseGameTime
    {
        private JDayNightCycle _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<JDayNightCycle>();
            if (_sky)
            {
                _currentHour = (int)_sky.StartTime;
                _currentMinute = (int)((_sky.StartTime - _currentHour) * 60f);

                _dayLengthInMinutes = (24f / _sky.TimeIncrement) / 60f;
                SetDayLength(_dayLengthInMinutes);
            } // if
        } // GetComponents()

        public override bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        public override float GetHour()
        {
            if (!_sky) return base.GetHour();

            _currentHour = (int)_sky.Time;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            if (!_sky)
            {
                base.SetHour(hour);
                return;
            } // if

            _currentHour = (int)hour;
            var currentMinute = (int)((_sky.Time - hour) * 60f);
            _sky.Time = hour + (currentMinute / 60f);
        } // SetHour()

        override public void SetMinute(float minute)
        {
            if (!_sky)
            {
                base.SetMinute(minute);
                return;
            } // if

            _currentMinute = (int)minute;
            var currentHour = (int)_sky.Time;
            _sky.Time = currentHour + (minute / 60f);
        } // SetMinute()

        public override float GetMinute()
        {
            if (!_sky) return base.GetMinute();

            _currentMinute = (int)((_sky.Time - (int)_sky.Time) * 60);
            return _currentMinute;
        } // GetMinute()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _sky.AutoTimeIncrement = false;
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.AutoTimeIncrement = true;
        } // Pause()

        override protected void FixedUpdate()
        {
            // Update the day, month, year since Jupiter does not do this for itself
            UpdateDates();
            UpdateSecondsAdded();
            CalculateDays();
            UpdateSeason();
        } // FixedUpdate()
    } // class JupiterGameTime()
} // namespace Magique.SoulLink
#endif