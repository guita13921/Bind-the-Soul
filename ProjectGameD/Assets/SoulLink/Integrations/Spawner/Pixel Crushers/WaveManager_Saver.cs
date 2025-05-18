#if SOULLINK_USE_DS || SOULLINK_USE_QM
using System;
using PixelCrushers;

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveManager_Saver : Saver
    {
        [Serializable]
        public class WaveManagerData
        {
            public int currentWaveIndex = 0;
        } // class WaveManagerData

        protected WaveManager _waveManager;
        protected WaveManagerData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new WaveManagerData();
            _waveManager = GetComponent<WaveManager>();
        } // Awake()

        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;

                // Restore wave manager data
                _waveManager.CurrentWaveIndex = m_data.currentWaveIndex;
            } // if
        } // ApplyData()

        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.currentWaveIndex = _waveManager.CurrentWaveIndex;

            return SaveSystem.Serialize(m_data);
        } // RecordData()
    } // class WaveManager_Saver
} // namespace Magique.SoulLink
#endif