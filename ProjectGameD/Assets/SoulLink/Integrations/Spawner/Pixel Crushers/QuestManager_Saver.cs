#if SOULLINK_USE_DS || SOULLINK_USE_QM
using System;
using System.Collections.Generic;
using PixelCrushers;
using UnityEngine;
using static Magique.SoulLink.Quest_Saver;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class QuestManager_Saver : Saver
    {
        [Serializable]
        public class QuestsData
        {
            public List<BaseQuestData> quests = new List<BaseQuestData>();
        }

        protected QuestsData m_data;

        private BaseQuestManager _questManager;

        public override void Awake()
        {
            base.Awake();

            _questManager = GetComponent<BaseQuestManager>();
            m_data = new QuestsData();
        } // Awake()

        /// <summary>
        /// Setup save data for each quest
        /// </summary>
        public override void Start()
        {
            base.Start();

            for (int i = 0; i <  _questManager.Quests.Count; ++i)
            {
                BaseQuestData questData = new BaseQuestData();
                m_data.quests.Add(questData);
            } // foreach
        } // Start()

        /// <summary>
        /// Record all quest data to save system
        /// </summary>
        /// <returns></returns>
        public override string RecordData()
        {
            for (int i = 0; i < _questManager.Quests.Count; ++i)
            {
                // Save data to persistent storage
                m_data.quests[i].isActive = _questManager.Quests[i].IsActive();
                foreach (var questEntry in _questManager.Quests[i].GetQuestEntries())
                {
                    m_data.quests[i].questEntryComplete.Add(questEntry.complete);

                    m_data.quests[i].itemsCompleteList = new ItemsCompleteList();
                    foreach (var entryItem in questEntry.questEntryItems)
                    {
                        m_data.quests[i].itemsCompleteList.keys.Add(questEntry.questEntryName);
                        m_data.quests[i].itemsCompleteList.values.Add(new EntryItemData() { complete = entryItem.complete, currentQty = entryItem.currentQty });
                    } // foreach
                } // foreach
            }

            return SaveSystem.Serialize(m_data);
        } // RecordData()

        /// <summary>
        /// Restore all saved quest data from save system
        /// </summary>
        /// <param name="s"></param>
        public override void ApplyData(string s)
        {
            // Update from saved data
            if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize(s, m_data);
                if (data == null) return;

                m_data = data;

                for (int h = 0; h < _questManager.Quests.Count; ++h)
                {
                    // complete values for entry and entry items
                    for (int i = 0; i < m_data.quests[h].questEntryComplete.Count; ++i)
                    {
                        int j = 0;
                        _questManager.Quests[h].GetQuestEntries()[i].complete = m_data.quests[h].questEntryComplete[i];
                        foreach (var item in _questManager.Quests[h].GetQuestEntries()[i].questEntryItems)
                        {
                            var index = m_data.quests[h].itemsCompleteList.keys.IndexOf(_questManager.Quests[h].GetQuestEntries()[i].questEntryName);
                            item.complete = m_data.quests[h].itemsCompleteList.values[index].complete;
                            item.currentQty = m_data.quests[h].itemsCompleteList.values[index].currentQty;
                            ++j;
                        } // foreach
                    } // for i
                } // for h
            } // if
        } // ApplyData()
    } // QuestManager_Saver
} // namespace Magique.SoulLink
#endif