#if SOULLINK_USE_QM

using PixelCrushers.QuestMachine;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class QMQuestManager : BaseQuestManager
    {
        /// <summary>
        /// UpdateQuest
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="prefab"></param>
        override public void UpdateQuest(ISoulLinkQuest quest, Transform prefab)
        {
            var questJournal = QuestMachine.GetQuestJournal();
            if (questJournal == null) return;

            // Get quest instance from player's quest journal
            Quest questData = quest.GetFieldValue<Quest>("_quest");
            var questInstance = questJournal.FindQuest(questData.id);

            var items = quest.GetQuestEntries(prefab);
            foreach (var item in items)
            {
                var counter = questInstance.GetCounter(item.questVariableName);
                if (counter == null) continue;

                int total = counter.currentValue + 1;
                counter.SetValue(total);

                item.complete = (total >= item.globalQtyRequired);

                // If not using the global qty feature then we must increment the individual entry's current qty
                if (!item.useGlobalQtyRequired)
                {
                    var entryItems = item.questEntryItems.FindAll(x => x.prefab == prefab);
                    foreach (var entryItem in entryItems)
                    {
                        entryItem.currentQty++;
                        entryItem.complete = (entryItem.currentQty >= entryItem.requiredQty);
                    } // foreach
                } // if
            } // foreach

            if (quest.AllEntriesComplete())
            {
                questInstance.SetState(QuestState.Successful);
            } // if
        } // UpdateQuest()

        /// <summary>
        /// IsQuestActive
        /// </summary>
        /// <param name="questName"></param>
        /// <returns></returns>
        override public bool IsQuestActive(string questName)
        {
            var questJournal = QuestMachine.GetQuestJournal();
            if (questJournal == null) return false;

            foreach (var quest in _quests)
            {
                Quest questData = quest.GetFieldValue<Quest>("_quest");
                if (questData.name != questName) continue;

                var questInstance = questJournal.FindQuest(questData.id);
                if (questInstance != null)
                {
                    if (questInstance.GetState() == QuestState.Active) return true;
                } // if
            } // foreach

            return false;
        } // IsQuestActive()
    } // class QMQuestManager
} // namespace Magique.SoulLink

#endif