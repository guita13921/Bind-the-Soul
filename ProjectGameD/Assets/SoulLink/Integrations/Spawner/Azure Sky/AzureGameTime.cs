#if SOULLINK_USE_AZURE
using UnityEngine.AzureSky;

// AzureGameTime
// An integration with Azure Sky by 7Stars.
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2020-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class AzureGameTime : BaseGameTime
    {
        private AzureTimeController _sky;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType<AzureTimeController>();
            if (_sky)
            {
                var tod = _sky.GetTimeOfDay();
                _currentHour = (int)tod.x;
                _currentMinute = (int)tod.y;

                _dayLengthInMinutes = _sky.GetFieldValue<float>("m_dayLength");
                SetDayLength(_dayLengthInMinutes);
            }
        } // GetComponents()

        public override float GetHour()
        {
            GetComponents();
            if (!_sky) return base.GetHour();

            _currentHour = (int)_sky.GetTimeOfDay().x;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            GetComponents();
            if (_sky)
            {
                _sky.SetTimeline(hour);
            } // if

            _currentHour = (int)hour;
        } // SetHour()

        override public void SetMinute(float minute)
        {
            GetComponents();
            if (_sky)
            {
                _sky.SetTimeline(_currentHour + (minute / 60f));
            } // if

            _currentMinute = (int)minute;
        } // SetMinute()

        public override float GetMinute()
        {
            GetComponents();
            if (!_sky) return base.GetMinute();

            _currentMinute = (int)_sky.GetTimeOfDay().y;
            return _currentMinute;
        } // GetMinute()

        override public int GetDay()
        {
            if (!_sky) return 0;

            _currentDay = _sky.GetDay();
            return _currentDay;
        } // GetDay()

        public override void SetDay(int value)
        {
            if (!_sky) return;

            _currentDay = value;
            _sky.SetDay(value);
        } // SetDay()

        public override int GetMonth()
        {
            if (!_sky) return 0;

            _currentMonth = _sky.GetMonth();
            return _currentMonth;
        } // GetMonth()

        public override void SetMonth(int value)
        {
            if (!_sky) return;

            _currentMonth = value;
            _sky.SetMonth(value);
        } // SetMonth()

        public override int GetYear()
        {
            if (!_sky) return 0;

            _currentYear = _sky.GetYear();
            return _currentYear;
        } // GetYear()

        public override void SetYear(int value)
        {
            if (!_sky) return;

            _currentYear = value;
            _sky.SetYear(value);
        } // SetYear()

        override public void Pause()
        {
            _paused = true;
            if (!_sky) return;

            _sky.SetNewDayLength(0f);
        } // Pause()

        override public void UnPause()
        {
            _paused = false;
            if (!_sky) return;

            _sky.SetNewDayLength(_dayLengthInMinutes);
        } // Pause()

        override protected void FixedUpdate()
        {
            CalculateDays();
            UpdateSeason();
            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class AzureGameTime
} // namespace Magique.SoulLink
#endif