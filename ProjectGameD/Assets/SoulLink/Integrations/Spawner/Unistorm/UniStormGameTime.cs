#if UNISTORM_PRESENT

using UniStorm;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// UnistormGameTime
// An integration with Unistom by Black Horizon Studios. 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2020-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class UniStormGameTime : BaseGameTime
    {
        private UniStormSystem _sky;

        override public void GetComponents()
        {
            _sky = FindObjectOfType<UniStormSystem>();
            if (_sky)
            {
                _dayLengthInMinutes = _sky.DayLength + _sky.NightLength;
                _currentMinute = _sky.Minute;
                _currentHour = _sky.Hour;
            }
        } // GetComponents()

        public override float GetHour()
        {
            if (!_sky) return base.GetHour();

            _currentHour = _sky.Hour;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            base.SetHour(hour);

            if (_sky != null)
            {
                UniStormManager.Instance.SetTime(_currentHour, _sky.Minute);
            }
            _currentHour = (int)hour;
        } // SetHour()

        public override float GetMinute()
        {
            if (!_sky) return base.GetMinute();

            _currentMinute = _sky.Minute;
            return _currentMinute;
        } // GetMinute()

        override public void SetMinute(float minute)
        {
            base.SetMinute(minute);

            if (_sky != null)
            {
                UniStormManager.Instance.SetTime(_sky.Hour, _currentMinute);
            }

            _currentMinute = (int)minute;
        } // SetMinute()

        override public int GetDay()
        {
            if (!_sky) return 0;

            _currentDay = _sky.Day;
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_sky) return;

            _currentDay = value;
            _sky.Day = value;
        } // SetDay()

        public override int GetMonth()
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

        public override Season GetSeason()
        {
            if (!_sky) return Season.Winter;

            switch (_sky.CurrentSeason)
            {
                case UniStormSystem.CurrentSeasonEnum.Spring:
                    return Season.Spring;
                case UniStormSystem.CurrentSeasonEnum.Summer:
                    return Season.Summer;
                case UniStormSystem.CurrentSeasonEnum.Fall:
                    return Season.Fall;
                case UniStormSystem.CurrentSeasonEnum.Winter:
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
                    _sky.CurrentSeason = UniStormSystem.CurrentSeasonEnum.Spring;
                    break;
                case Season.Summer:
                    _sky.CurrentSeason = UniStormSystem.CurrentSeasonEnum.Summer;
                    break;
                case Season.Fall:
                    _sky.CurrentSeason = UniStormSystem.CurrentSeasonEnum.Fall;
                    break;
                case Season.Winter:
                    _sky.CurrentSeason = UniStormSystem.CurrentSeasonEnum.Winter;
                    break;
            } // switch
        } // SetSeason()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _sky.TimeFlow = UniStormSystem.EnableFeature.Disabled;
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.TimeFlow = UniStormSystem.EnableFeature.Enabled;
        } // Pause()

        override protected void FixedUpdate()
        {
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class UniStormGameTime
} // namespace Magique.SoulLink
#endif
