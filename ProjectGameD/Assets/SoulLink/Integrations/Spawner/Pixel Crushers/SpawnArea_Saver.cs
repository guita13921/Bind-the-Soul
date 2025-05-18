#if SOULLINK_USE_DS || SOULLINK_USE_QM && SOULLINK_SPAWNER
using System;
using PixelCrushers;
using UnityEngine;

// (c)2020-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnArea_Saver : Saver
    {
        [Serializable]
        public class SpawnAreaData
        {
            public string spawnAreaGuid;
            public bool isTriggered;
            public float secondsToReset;
            public int totalResets;
            public bool enabled;
        }

        protected SpawnArea _spawnArea;
        protected SpawnAreaData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new SpawnAreaData();
            _spawnArea = GetComponent<SpawnArea>();
        } // Awake()

        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;
                _spawnArea.SetGuid(m_data.spawnAreaGuid);
                _spawnArea.IsTriggered = m_data.isTriggered;
                _spawnArea.SetSecondsToReset(m_data.secondsToReset);
                _spawnArea.SetTotalResets(m_data.totalResets);
                _spawnArea.Enabled = m_data.enabled;
            }
        } // ApplyData()

        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.spawnAreaGuid = _spawnArea.GetGuid();
            m_data.isTriggered = _spawnArea.IsTriggered;
            m_data.secondsToReset = _spawnArea.SecondsToReset;
            m_data.totalResets = _spawnArea.TotalResets;
            m_data.enabled = _spawnArea.Enabled;

            return SaveSystem.Serialize(m_data);
        } // RecordData()

    } // class SpawnArea_Saver
} // namespace Magique.SoulLink
#endif