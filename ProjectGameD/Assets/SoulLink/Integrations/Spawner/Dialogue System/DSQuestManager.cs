#if SOULLINK_USE_DS

using PixelCrushers.DialogueSystem;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class DSQuestManager : BaseQuestManager
    {
        /// <summary>
        /// UpdateQuest
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="prefab"></param>
        override public void UpdateQuest(ISoulLinkQuest quest, Transform prefab)
        {
            var items = quest.GetQuestEntries(prefab);
            foreach (var item in items)
            {
                int total = DialogueLua.GetVariable(item.questVariableName).asInt + 1;
                DialogueLua.SetVariable(item.questVariableName, total);
                DialogueManager.SendUpdateTracker();

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
                QuestLog.CompleteQuest(quest.GetQuestName());
                DialogueManager.ShowAlert(QuestLog.GetQuestDescription(quest.GetQuestName()), 8f);
            } // if
        } // UpdateQuest()

        /// <summary>
        /// ActivateQuest
        /// </summary>
        /// <param name="questName"></param>
        override public void ActivateQuest(string questName)
        {
            QuestLog.SetQuestState(questName, QuestState.Active);
        } // ActivateQuest()

        /// <summary>
        /// IsQuestActive
        /// </summary>
        /// <param name="questName"></param>
        /// <returns></returns>
        override public bool IsQuestActive(string questName)
        {
            return QuestLog.GetQuestState(questName) == QuestState.Active;
        } // IsQuestActive()
    } // class DSQuestManager
} // namespace Magique.SoulLink

#endif