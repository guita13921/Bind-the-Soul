#if SOULLINK_USE_DS || SOULLINK_USE_QM
using System;
using System.Collections.Generic;
using PixelCrushers;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Quest_Saver : Saver
    {
        [Serializable]
        public class EntryItemData
        {
            public bool complete = false;
            public int currentQty = 0;
        } // class EntryItemData

        [Serializable]
        public class EntryItemDataList
        {
            public List<EntryItemData> entryItems = new List<EntryItemData>();
        } // EntryItemDataList

        [Serializable]
        public class ItemsCompleteList
        {
            public List<string> keys = new List<string>();
            public List<EntryItemData> values = new List<EntryItemData>();
        }

        [Serializable]
        public class BaseQuestData
        {
            public bool isActive;
            public List<bool> questEntryComplete = new List<bool>();
            public ItemsCompleteList itemsCompleteList = new ItemsCompleteList();
        } // class BaseQuestData

        protected BaseQuest _quest;
        protected BaseQuestData m_data;

        public override void Awake()
        {
            base.Awake();
            m_data = new BaseQuestData();
            _quest = GetComponent<BaseQuest>();
        } // Awake()

        /// <summary>
        /// Record all quest data to save system
        /// </summary>
        /// <returns></returns>
        public override string RecordData()
        {
            // Save data to persistent storage
            m_data.isActive = _quest.IsActive();
            foreach (var questEntry in _quest.GetQuestEntries())
            {
                m_data.questEntryComplete.Add(questEntry.complete);

                m_data.itemsCompleteList = new ItemsCompleteList();
                foreach (var entryItem in questEntry.questEntryItems)
                {
                    m_data.itemsCompleteList.keys.Add(questEntry.questEntryName);
                    m_data.itemsCompleteList.values.Add(new EntryItemData() { complete = entryItem.complete, currentQty = entryItem.currentQty });
                } // foreach

            } // foreach

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

                // complete values for entry and entry items
                for (int i = 0; i < m_data.questEntryComplete.Count; ++i)
                {
                    int j = 0;
                    _quest.GetQuestEntries()[i].complete = m_data.questEntryComplete[i];
                    foreach (var item in _quest.GetQuestEntries()[i].questEntryItems)
                    {
                        var index = m_data.itemsCompleteList.keys.IndexOf(_quest.GetQuestEntries()[i].questEntryName);
                        item.complete = m_data.itemsCompleteList.values[index].complete;
                        item.currentQty = m_data.itemsCompleteList.values[index].currentQty;
                        ++j;
                    } // foreach
                } // for i
            } // if
        } // ApplyData()
    } // class Quest_Saver
} // namespace Magique.SoulLink
#endif