#if SOULLINK_USE_TENKOKU
using Tenkoku.Core;

// TenkokuGameTime
// An integration with Tenkoku Dynamic Sky System by Tanuki Digital. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class TenkokuGameTime : BaseGameTime
    {
        private TenkokuModule _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<TenkokuModule>();
            if (_sky)
            {
                _currentHour = _sky.currentHour;
                _currentMinute = _sky.currentMinute;

                _dayLengthInMinutes = 1440 / _sky.timeCompression;
                SetDayLength(_dayLengthInMinutes);
            } // if
        } // GetComponents()

        public override float GetHour()
        {
            if (!_sky) return base.GetHour();

            _currentHour = _sky.currentHour;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            _sky.currentHour = (int)hour;
            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            _sky.currentMinute = (int)minute;
            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            if (!_sky) return base.GetMinute();

            _currentMinute = _sky.currentMinute;
            return _currentMinute;
        } // GetMinute()

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

            _currentMonth = _sky.currentMonth;
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            _currentMonth = value;
            _sky.currentMonth = value;
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

            _sky.autoTime = false;
        } // Pause()

        public override void UnPause()
        {
            _paused = false;

            if (!_sky) return;

            _sky.autoTime = true;
        } // UnPause()

        override protected void FixedUpdate()
        {
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class TenkokuGameTime
} // namespace Magique.SoulLink
#endif