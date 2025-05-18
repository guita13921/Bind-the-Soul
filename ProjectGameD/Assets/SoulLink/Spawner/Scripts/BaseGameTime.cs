// BaseGameTime
// A base game time component from which all other game time components should derive. This provides the basic functionality required 
// for all game time systems in order to make integrations easier to implement.
// 
// Use this component in indoor scenes to continue to keep track of game time. Use the GameTime_Saver component to store values across
// scenes so that your outdoor scenes can be properly updated when transitioning.
//
// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

namespace Magique.SoulLink
{
    public class BaseGameTime : MonoBehaviour, IGameTime
    {
        protected uint[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public float DayLengthInMinutes
        {
            get { return _dayLengthInMinutes; }
        }
        [SerializeField]
        protected float _dayLengthInMinutes = 24;

        protected float _secondsAdded;
        protected float _seconds = 0f;
        protected Season _season = Season.Winter;
        protected uint _days = 1; // Incremental count of days from 1-365
        protected float _gameDeltaTime = 0f;

        public event Action OnDayChanged = delegate { };
        private int _lastDay = 0;

        // A dictionary to lookup the day in the month from a day in the year
        private Dictionary<int, int> dayInMonth = new Dictionary<int, int>();

        [Range(0, 24)]
        [SerializeField]
        protected int _currentHour = 12;

        [Range(0,60)]
        [SerializeField]
        protected int _currentMinute;

        [SerializeField]
        protected int _currentDay = 1;

        [Range(1,12)]
        [SerializeField]
        protected int _currentMonth = 1;

        [SerializeField]
        protected int _currentYear = 2022;

        protected bool _paused = false;
        protected int _lastHourCheck = 0;
        protected bool _rollDay = true;

        protected const float oneDayInMinutes = 60 * 24;

        /// <summary>
        ///  Start
        /// </summary>
        virtual public void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        } // Start()

        /// <summary>
        ///  Awake
        /// </summary>
        virtual public void Awake()
        {
            GetComponents();

            // Build dictionary of dayInMonth
            int month = 0;
            int day = 0;
            for (int i = 1; i < 366; ++i)
            {
                day++;
                if (day > daysInMonth[month])
                {
                    day = 1;
                    month++;
                } // if

                dayInMonth[i] = day;
            } // for i
        } // Awake()

        /// <summary>
        /// GetSoulLinkControlsTime
        /// </summary>
        /// <returns></returns>
        virtual public bool GetSoulLinkControlsTime()
        {
            return false;
        } // GetSoulLinkControlsTime()

        /// <summary>
        /// GetSoulLinkControlsDate
        /// </summary>
        /// <returns></returns>
        virtual public bool GetSoulLinkControlsDate()
        {
            return false;
        } // GetSoulLinkControlsDate()

        /// <summary>
        /// GetComponents
        /// </summary>
        virtual public void GetComponents()
        {
            // Inherited classes use this to get the third party sky asset
        } // GetComponents()

        /// <summary>
        /// GetHour
        /// </summary>
        /// <returns></returns>
        virtual public float GetHour()
        {
            return _currentHour;
        } // GetHour()

        /// <summary>
        /// GetMinute
        /// </summary>
        /// <returns></returns>
        virtual public float GetMinute()
        {
            return _currentMinute;
        } // GetMinute()

        /// <summary>
        /// Returns the current day of the month
        /// </summary>
        /// <returns></returns>
        virtual public int GetDay()
        {
            return _currentDay;
        } // GetDay()

        /// <summary>
        /// Sets the current day of the month
        /// </summary>
        /// <param name="value"></param>
        virtual public void SetDay(int value)
        {
            _currentDay = value;
        } // SetDay(

        /// <summary>
        /// Returns the current month
        /// </summary>
        /// <returns></returns>
        virtual public int GetMonth()
        {
            return _currentMonth;
        } // GetMonth()

        /// <summary>
        /// Sets the current month
        /// </summary>
        /// <param name="value"></param>
        virtual public void SetMonth(int value)
        {
            _currentMonth = value;
        } // SetMonth()

        /// <summary>
        /// Returns the current year
        /// </summary>
        /// <returns></returns>
        virtual public int GetYear()
        {
            return _currentYear;
        } // GetYear()

        /// <summary>
        /// Sets the current year
        /// </summary>
        /// <param name="value"></param>
        virtual public void SetYear(int value)
        {
            _currentYear = value;
        } // SetYear()

        /// <summary>
        /// SetDayLength
        /// </summary>
        /// <param name="value"></param>
        virtual public void SetDayLength(float value)
        {
            _dayLengthInMinutes = value;
        } // SetDayLength()

        /// <summary>
        /// GetDayLength
        /// </summary>
        /// <returns></returns>
        virtual public float GetDayLength()
        {
            return _dayLengthInMinutes;
        } // GetDayLength()

        /// <summary>
        /// Returns the number of seconds added since time has started accumulating with this component
        /// </summary>
        /// <returns></returns>
        public float GetSecondsAdded()
        {
            return _secondsAdded;
        } // GetSecondsAdded()

        /// <summary>
        /// OnSceneLoaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        virtual public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Inherited classes use this to perform actions when the scene is loaded
        } // OnSceneLoaded()

        /// <summary>
        /// Refresh
        /// </summary>
        virtual public void Refresh()
        {
            // Inherited classes use this to refresh the third party sky system
        } // Refresh()

        /// <summary>
        /// SetHour
        /// </summary>
        /// <param name="hour"></param>
        virtual public void SetHour(float hour)
        {
            _currentHour = (int)hour;
        } // SetHour()

        /// <summary>
        /// SetMinute
        /// </summary>
        /// <param name="minute"></param>
        virtual public void SetMinute(float minute)
        {
            _currentMinute = (int)minute;
        } // SetMinute()

        /// <summary>
        /// Calculate the updated game time based on the day length in minutes
        /// </summary>
        virtual protected void UpdateGameTime()
        {
            if (_paused) return;

            if (_dayLengthInMinutes > 0)
            {
                float timeFactor = oneDayInMinutes / _dayLengthInMinutes;

                _seconds += Time.fixedDeltaTime * timeFactor;

                if (_seconds >= 60f)
                {
                    _currentMinute++;

                    if (_currentMinute == 60)
                    {
                        _currentHour++;
                        if (_currentHour == 24)
                        {
                            _currentHour = 0;
                        }

                        _currentMinute = 0;
                    } // if

                    _seconds = _seconds - 60f;
                } // if
            } // if
        } // UpdateGameTime()

        /// <summary>
        /// Update days, months, and years based on progression of time
        /// </summary>
        virtual protected void UpdateDates()
        {
            if (_paused) return;

            if (_currentHour < _lastHourCheck && _rollDay)
            {
                _rollDay = false;

                _currentDay++;
                _days++;

                if (_currentDay > daysInMonth[_currentMonth - 1])
                {
                    _currentDay = 1;
                    _currentMonth++;

                    if (_currentMonth > 12)
                    {
                        _currentMonth = 1;
                        _currentYear++;
                        _days = 1;
                    } // if
                } // if
            }
            else if (_currentHour > 0)
            {
                // Reset so that next time hour is back to 0 then we can roll over to the next day again
                _rollDay = true;
            }

            _lastHourCheck = _currentHour;
        } // UpdateDates()

        /// <summary>
        /// Calculate the day of the year value from the month and day
        /// </summary>
        public void CalculateDays()
        {
            _days = 0;
            if (_currentMonth > 1)
            {
                for (int i = 1; i < _currentMonth; ++i)
                {
                    _days += daysInMonth[i - 1];
                }
            } // if

            _days += (uint)_currentDay;
        } // CalculateDays()

        /// <summary>
        /// Updates the season based on the day of the year
        /// </summary>
        public void UpdateSeason()
        {
            if (_days >= 79 && _days < 172)
            {
                _season = Season.Spring;
            }
            else if (_days >= 172 && _days < 264)
            {
                _season = Season.Summer;
            }
            else if (_days >= 264 && _days < 353)
            {
                _season = Season.Fall;
            }
            else if (_days >= 353 && _days < 79)
            {
                _season = Season.Winter;
            }
        } // UpdateSeason()

        /// <summary>
        /// Convert the days in year value to the current day in the current month
        /// </summary>
        protected void ConvertDaysToDay()
        {
            if (!dayInMonth.ContainsKey((int)_days)) return;

            _currentDay = dayInMonth[(int)_days];
        } // ConvertDaysToDay()

        /// <summary>
        /// Calculate the day in the current month from the days count for the year
        /// </summary>
        /// <param name="daysOfYear"></param>
        /// <returns></returns>
        protected int GetDayInMonth(int daysOfYear)
        {
            if (!dayInMonth.ContainsKey(daysOfYear)) return 1;

            return dayInMonth[daysOfYear];
        } // GetDayInMonth()


        /// <summary>
        /// Get the current month number from the accumulated days of the year
        /// </summary>
        /// <param name="daysOfYear"></param>
        /// <returns></returns>
        protected int GetMonthFromDays(int daysOfYear)
        {
            uint dayCount = 0;
            int i = 0;
            foreach (var month in daysInMonth)
            {
                dayCount += daysInMonth[i];
                if (daysOfYear <= dayCount) return i + 1;
                ++i;
            } // foreach

            return 1;
        } // GetMonthFromDays()

        /// <summary>
        /// Count number of days up to the indicated month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        protected uint GetDaysUpToMonth(int month)
        {
            if (month == 1) return 0;

            uint dayCount = 0;
            for (int i = 0; i < month - 1; ++i)
            {
                dayCount += daysInMonth[i];
            } // for i

            return dayCount;
        } // GetDaysUpToMonth()

        /// <summary>
        /// Convert the days in year value to the current day in the current month
        /// </summary>
        protected void ConvertDayToDays(int value)
        {
            if (_currentMonth != 1)
            {
                _days = 0;
                for (int i = 0; i < _currentMonth - 1; ++i)
                {
                    _days += daysInMonth[i];
                } // for i
            }
            else
            {
                _days = (uint)value;
            }
        } // ConvertDaysToDay()

        /// <summary>
        /// GetTime
        /// </summary>
        /// <returns></returns>
        virtual public float GetTime()
        {
            return (GetHour() + (GetMinute() / 60f));
        } // GetTime()

        /// <summary>
        /// Pause game time from advancing
        /// </summary>
        virtual public void Pause()
        {
            _paused = true;
        } // Pause()

        /// <summary>
        /// Unpause game time so it can advance
        /// </summary>
        virtual public void UnPause()
        {
            _paused = false;
        } // UnPause()

        virtual public Season GetSeason()
        {
            return _season;
        } // GetSeason()

        virtual public void SetSeason(Season value)
        {
            _season = value;
        } // SetSeason()

        virtual public float GetGameDeltaTime()
        {
            return _gameDeltaTime;
        } // GetGameDeltaTime()

        virtual public void UpdateSecondsAdded()
        {
            // Must calulate the seconds added since starting time system
            if (_dayLengthInMinutes > 0)
            {
                float timeFactor = oneDayInMinutes / _dayLengthInMinutes;

                _gameDeltaTime = Time.fixedDeltaTime * timeFactor;
                _secondsAdded += _gameDeltaTime;
            } // if

            // Check if we are now in a different day. If so then fire off a ChangeDay event
            if (_lastDay != GetDay())
            {
                if (_lastDay != 0 && OnDayChanged != null)
                {
                    OnDayChanged.Invoke();
                } // if

                _lastDay = GetDay();
            } // if
        } // UpdateSecondsAdded()

        /// <summary>
        /// FixedUpdate
        /// </summary>
        virtual protected void FixedUpdate()
        {
            UpdateGameTime();
            UpdateSecondsAdded();
            UpdateDates();
            UpdateSeason();
        } // FixedUpdate()
    } // class BaseGameTime
} // namespace Magique
