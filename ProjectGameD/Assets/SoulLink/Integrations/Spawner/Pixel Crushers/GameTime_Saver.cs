#if SOULLINK_USE_DS || SOULLINK_USE_QM
using System;
using PixelCrushers;

// (c)2020-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class GameTime_Saver : Saver
    {
        [Serializable]
        public class GameTimeData
        {
            public float hour;
            public float minute;
            public float dayLengthInMinutes;
            public int day;
            public int month;
            public int year;
            public float secondsAdded;
        }

        protected BaseGameTime _gameTime;
        protected GameTimeData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new GameTimeData();
            _gameTime = GetComponent<BaseGameTime>();
        } // Awake()

        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;
                _gameTime.SetHour(m_data.hour);
                _gameTime.SetMinute(m_data.minute);
                _gameTime.SetDayLength(m_data.dayLengthInMinutes);
                _gameTime.SetDay(m_data.day);
                _gameTime.SetMonth(m_data.month);
                _gameTime.SetYear(m_data.year);
                _gameTime.Refresh();

#if SOULLINK_SPAWNER
                // Tell SoulLinkSpawner to update SpawnArea time elapsed and force syncing time of day. 
                if (SoulLinkSpawner.Instance)
                {
                    SoulLinkSpawner.Instance.UpdateSpawnAreas(_gameTime.GetSecondsAdded());
                    SoulLinkSpawner.Instance.DoSyncTimeOfDay();
                    var slSaver = SoulLinkSpawner.Instance.GetComponent<SoulLinkSpawner_Saver>();
                    if (slSaver != null)
                    {
                        slSaver.OnGameTimeLoaded(_gameTime.GetSecondsAdded() / 60f / 60f);
                    } // if
                } // if
#endif
            }
        } // ApplyData()

        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.hour = _gameTime.GetHour();
            m_data.minute = _gameTime.GetMinute();
            m_data.dayLengthInMinutes = _gameTime.GetDayLength();
            m_data.day = _gameTime.GetDay();
            m_data.month = _gameTime.GetMonth();
            m_data.year = _gameTime.GetYear();
            m_data.secondsAdded = _gameTime.GetSecondsAdded();

            return SaveSystem.Serialize(m_data);
        } // RecordData()

    } // class GameTime_Saver
} // namespace Magique.SoulLink
#endif