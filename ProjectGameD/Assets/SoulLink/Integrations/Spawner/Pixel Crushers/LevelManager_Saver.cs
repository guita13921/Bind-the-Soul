#if SOULLINK_USE_DS || SOULLINK_USE_QM
using System;
using PixelCrushers;

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class LevelManager_Saver : Saver
    {
        [Serializable]
        public class LevelManagerData
        {
            public int currentLevelIndex = 0;
        } // class LevelManagerData

        protected LevelManager _levelManager;
        protected LevelManagerData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new LevelManagerData();
            _levelManager = GetComponent<LevelManager>();
        } // Awake()

        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;

                // Restore level manager data
                _levelManager.CurrentLevelIndex = m_data.currentLevelIndex;
            } // if
        } // ApplyData()

        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.currentLevelIndex = _levelManager.CurrentLevelIndex;

            return SaveSystem.Serialize(m_data);
        } // RecordData()
    } // class LevelManager_Saver
} // namespace Magique.SoulLink
#endif