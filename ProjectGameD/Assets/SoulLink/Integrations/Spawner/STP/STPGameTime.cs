#if SURVIVAL_TEMPLATE_PRO

// STPGameTime
// An integration with Survival Template Pro by Polymind Games
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

using PolymindGames.WorldManagement;
using UnityEngine;

namespace Magique.SoulLink
{
    public class STPGameTime : BaseGameTime
    {
        private WorldManager _sky;
        private bool _saveTimeProgressMode;

        override public void GetComponents()
        {
            if (_sky != null) return;

            _sky = FindObjectOfType <WorldManager>();
            if (_sky)
            {
                _currentHour = _sky.GetGameTime().Hours;
                _currentMinute = _sky.GetGameTime().Minutes;
                GetMonth();
                GetYear();

                _dayLengthInMinutes = _sky.GetDayDurationInMinutes();
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

            _currentHour = _sky.GetGameTime().Hours;
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            // Calculate precise number of hours to pass. Make sure minutes always ends up zero so the minutes can be set separately
            if (hour - _currentHour > 0)
            {
                var delta = ((hour - _currentHour) - (_currentMinute / 60f)) / 24f;
                _sky.PassTime(delta, 0.1f);
            }
            else
            {
                var delta = ((24f - _currentHour + hour) - (_currentMinute / 60f)) / 24f;
                _sky.PassTime(delta, 0.1f);
            }
        } // SetHour()

        override public void SetMinute(float minute)
        {
            // Calculate precise number of minutes to pass and then update using PassTime
            _sky.PassTime((minute / 60f) / 24f, 0.1f);
        } // SetMinute()

        public override float GetMinute()
        {
            GetComponents();

            _currentMinute = _sky.GetGameTime().Minutes;
            return _currentMinute;
        } // GetMinute()

        override public void Pause()
        {
            _paused = true;
            _saveTimeProgressMode = _sky.TimeProgressionEnabled;

            _sky.TimeProgressionEnabled = false;
        } // Pause()

        public override void UnPause()
        {
            _paused = false;
            _sky.TimeProgressionEnabled = _saveTimeProgressMode;
        } // UnPause()

        override protected void FixedUpdate()
        {
            // Update the day, month, year since Jupiter does not do this for itself
            UpdateDates();
            UpdateSecondsAdded();
            CalculateDays();
            UpdateSeason();
        } // FixedUpdate()
    } // class STPGameTime
} // namespace Magique.SoulLink
#endif