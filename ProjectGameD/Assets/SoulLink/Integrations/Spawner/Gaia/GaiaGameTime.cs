#if GAIA_PRO_PRESENT || HDRPTIMEOFDAY

#if !HDPipeline
using Gaia;
#endif

// GaiaGameTime
// An integration with Gaia Pro by Procedural Worlds Time of Day (Built-In and HDRP). 
//
// To use with Dialogue System or Quest Machine by Pixel Crushers, attach a GameTime_Saver component to the same object that this 
// component is attached to.
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

#if HDPipeline
using ProceduralWorlds.HDRPTOD;
#endif

namespace Magique.SoulLink
{
    public class GaiaGameTime : BaseGameTime
    {
#if HDPipeline
        private HDRPTimeOfDay _hdrpTimeOfDay;
#endif

        public override void GetComponents()
        {
#if HDPipeline
            if (_hdrpTimeOfDay == null)
            {
                _hdrpTimeOfDay = FindObjectOfType<HDRPTimeOfDay>();
            }

            if (_hdrpTimeOfDay.m_enableTimeOfDaySystem)
            {
                SetDayLength((1f / _hdrpTimeOfDay.m_timeOfDayMultiplier) * 24f);
            }
            else
            {
                SetDayLength(0);
            }
#else
            if (GaiaAPI.GetTimeOfDayEnabled())
            {
                SetDayLength((1f / GaiaAPI.GetTimeOfDayTimeScale()) * 24f);
            }
            else
            {
                SetDayLength(0);
            }
#endif
        } // GetComponents();

        public override bool GetSoulLinkControlsDate()
        {
            return true;
        } // GetSoulLinkControlsDate()

        public override float GetHour()
        {
#if HDPipeline
            GetComponents();
            _currentHour = (int)_hdrpTimeOfDay.TimeOfDay;
#else
            _currentHour = GaiaAPI.GetTimeOfDayHour();
#endif
            return _currentHour;
        } // GetHour()

        override public void SetHour(float hour)
        {
            _currentHour = (int)hour;

#if HDPipeline
            _hdrpTimeOfDay.TimeOfDay = hour;
#else
            GaiaAPI.SetTimeOfDayHour((int)hour);
#endif
        } // SetHour()

        override public void SetMinute(float minute)
        {
            _currentMinute = (int)minute;

#if HDPipeline
            _hdrpTimeOfDay.TimeOfDay = _currentHour + (minute / 60f);
#else
            GaiaAPI.SetTimeOfDayMinute(minute);
#endif
        } // SetMinute()

        public override float GetMinute()
        {
#if HDPipeline
            GetComponents();
            _currentMinute = (int)((_hdrpTimeOfDay.TimeOfDay - (int)_hdrpTimeOfDay.TimeOfDay) * 60f);
#else
            _currentMinute = (int)GaiaAPI.GetTimeOfDayMinute();
#endif
            return _currentMinute;
        } // GetMinute()

        override public void Pause()
        {
            _paused = true;

#if HDPipeline
            _hdrpTimeOfDay.m_enableTimeOfDaySystem = false;
#else
            GaiaAPI.SetTimeOfDayEnabled(false);
#endif
            SetDayLength(0);
        } // Pause()

        override public void UnPause()
        {
            _paused = false;

#if HDPipeline
            _hdrpTimeOfDay.m_enableTimeOfDaySystem = true;
            SetDayLength((1f / _hdrpTimeOfDay.m_timeOfDayMultiplier) * 24f);
#else
            GaiaAPI.SetTimeOfDayEnabled(true);
            SetDayLength((1f / GaiaAPI.GetTimeOfDayTimeScale()) * 24f);
#endif
        } // Pause()

        override protected void FixedUpdate()
        {
            // Update the day, month, year since Gaia does not do this for itself
            UpdateDates();

            // Calculate the number of days from start of the year and update the season
            CalculateDays();
            UpdateSeason();

            UpdateSecondsAdded();
        } // FixedUpdate()
    } // class GaiaGameTime()
} // namespace Magique.SoulLink
#endif