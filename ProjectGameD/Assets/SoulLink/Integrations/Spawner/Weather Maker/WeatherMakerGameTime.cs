#if WEATHER_MAKER_PRESENT
using DigitalRuby.WeatherMaker;

// WeatherMakerGameTime
// An integration with Weather Maker by Digital Ruby. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTimer_Saver component to the same object that this 
// component is attached to.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WeatherMakerGameTime : BaseGameTime
    {
        private WeatherMakerDayNightCycleManagerScript _sky;
        private float _saveSpeed;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<WeatherMakerDayNightCycleManagerScript>();
            if (_sky)
            {
                _currentHour = _sky.DateTime.Hour;
                _currentMinute = _sky.DateTime.Minute;

                _dayLengthInMinutes = _sky.DayNightProfile.Speed == 0 ? 1440 : (1440 / _sky.DayNightProfile.Speed);
                SetDayLength(_dayLengthInMinutes);
            } // if
        } // GetComponents()

        public override float GetHour()
        {
            if (!_sky) return base.GetHour();

            _currentHour = _sky.DateTime.Hour;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            var year = _sky.DateTime.Year;
            var month = _sky.DateTime.Month;
            var day = _sky.DateTime.Day;
            var minute = _sky.DateTime.Minute;
            var second = _sky.DateTime.Second;

            _sky.DateTime = new System.DateTime(year, month, day, (int)hour, minute, second);
            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            var year = _sky.DateTime.Year;
            var month = _sky.DateTime.Month;
            var day = _sky.DateTime.Day;
            var hour = _sky.DateTime.Hour;
            var second = _sky.DateTime.Second;

            _sky.DateTime = new System.DateTime(year, month, day, hour, (int)minute, second);
            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            if (!_sky) return base.GetMinute();

            _currentMinute = _sky.DateTime.Minute;
            return _currentMinute;
        } // GetMinute()

        override public int GetDay()
        {
            if (!_sky) return 0;

            _currentDay = _sky.Day;
            return _currentDay;
        } // GetDay()

        override public void SetDay(int value)
        {
            if (!_sky) return;

            _currentDay = value;
            _sky.Day = value;
        } // SetDay()

        override public int GetMonth()
        {
            if (!_sky) return 0;

            _currentMonth = _sky.Month;
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            _currentMonth = value;
            _sky.Month = value;
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.Year;
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _currentYear = value;
            _sky.Year = value;
        } // SetYear()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _saveSpeed = _sky.Speed;
            _sky.Speed = 0f;
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.Speed = _saveSpeed;
        } // Pause()

        override protected void FixedUpdate()
        {
            CalculateDays();
            UpdateSeason();

            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class WeatherMakerGameTime
} // namespace Magique.SoulLink
#endif