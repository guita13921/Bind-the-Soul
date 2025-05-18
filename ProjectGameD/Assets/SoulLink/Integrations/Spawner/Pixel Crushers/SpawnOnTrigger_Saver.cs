#if SOULLINK_USE_DS || SOULLINK_USE_QM && SOULLINK_SPAWNER
using System;
using PixelCrushers;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnOnTrigger_Saver : Saver
    {
        [Serializable]
        public class SpawnOnTriggerData
        {
            public bool isTriggered;
        }

        protected SpawnOnTrigger _spawnOnTrigger;
        protected SpawnOnTriggerData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new SpawnOnTriggerData();
            _spawnOnTrigger = GetComponent<SpawnOnTrigger>();
        } // Awake()

        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;
                _spawnOnTrigger.SetIsTriggered(m_data.isTriggered);
            }
        } // ApplyData()

        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.isTriggered = _spawnOnTrigger.IsTriggered;

            return SaveSystem.Serialize(m_data);
        } // RecordData()

    } // class SpawnOnTrigger_Saver
} // namespace Magique.SoulLink
#endif