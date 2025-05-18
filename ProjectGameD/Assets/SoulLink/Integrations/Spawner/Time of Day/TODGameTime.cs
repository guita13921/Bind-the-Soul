#if SOULLINK_USE_TOD

// TODGameTime
// An integration for Time of Day by André Straubmeier. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2020-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class TODGameTime : BaseGameTime
    {
        private TOD_Time _todTime;
        private TOD_Sky _sky;

        override public void GetComponents()
        {
            _sky = FindObjectOfType<TOD_Sky>();
            if (_sky)
            {
                _currentHour = (int)_sky.Cycle.Hour;
                _currentMinute = (int)(60f * (_sky.Cycle.Hour - (float)((int)_sky.Cycle.Hour)));

                _todTime = _sky.GetComponent<TOD_Time>();
                if (_todTime)
                {
                    _dayLengthInMinutes = _todTime.DayLengthInMinutes;
                } // if
            }
        } // GetComponents()

        override public void SetHour(float hour)
        {
            if (!_sky) return;

            _currentHour = (int)hour;
            _sky.Cycle.Hour = hour;
            _secondsAdded = 0f;
            _seconds = 0f;
        } // SetHour()

        override public float GetHour()
        {
            if (!_sky) return 0f;

            _currentHour = (int)_sky.Cycle.Hour;
            return _currentHour;
        } // GetHour()

        override public void SetMinute(float minute)
        {
            _currentMinute = (int)minute;
        } // SetMinute()

        override public int GetDay()
        {
            if (!_sky) return 0;

            _currentDay = _sky.Cycle.Day;
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_sky) return;

            _currentDay = value;
            _sky.Cycle.Day = value;
        } // SetDay()

        public override int GetMonth()
        {
            if (!_sky) return 0;

            _currentMonth = _sky.Cycle.Month;
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            _currentMonth = value;
            _sky.Cycle.Month = value;
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.Cycle.Year;
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _currentYear = value;
            _sky.Cycle.Year = value;
        } // SetYear()

        override public float GetTime()
        {
            return GetHour(); // includes minutes in fractional part
        } // GetTime()

        override public void Pause()
        {
            if (!_todTime) return;

            _paused = true;
            _todTime.ProgressTime = false;
        } // Pause()

        public override void UnPause()
        {
            if (!_todTime) return;

            _todTime.ProgressTime = true;
            _paused = false;
        } // UnPause()

        override protected void FixedUpdate()
        {
            CalculateDays();
            UpdateSeason();
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class TODGameTime
} // namespace Magique.SoulLink
#endif