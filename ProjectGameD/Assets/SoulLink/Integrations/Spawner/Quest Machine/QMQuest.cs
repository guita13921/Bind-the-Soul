#if SOULLINK_USE_QM

using PixelCrushers.QuestMachine;
using UnityEngine;

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Quest Machine Quest")]
    public class QMQuest : BaseQuest
    {
        public Quest QuestData
        {
            get { return _quest; }
        }
        [SerializeField]
        protected Quest _quest;

        private void Start()
        {
            SyncCounters();
        } // Start()


        /// <summary>
        /// Synchronize the counter values so that user can override what's in the QM database
        /// </summary>
        private void SyncCounters()
        {
            var questJournal = QuestMachine.GetQuestJournal();
            if (questJournal == null)
            {
                Debug.LogError("QMQuest Error: There is no Quest Journal in your scene.");
                return;
            } // if

            var questInstance = questJournal.FindQuest(_quest.id);
            if (questInstance != null)
            {
                // Set the required counter values here as well (will override the Quest Machine value)
                foreach (var entry in _questEntries)
                {
                    var counter = questInstance.GetCounter(entry.questVariableName);
                    if (counter != null)
                    {
                        var syncValue = entry.useGlobalQtyRequired ? entry.globalQtyRequired : GetItemTotals(entry);
                        counter.maxValue = syncValue;

                        QuestNode node = questInstance.GetNode(entry.questEntryName);
                        if (node != null)
                        {
                            CounterQuestCondition condition = 
                                (CounterQuestCondition)node.conditionSet.conditionList.Find(x => x is CounterQuestCondition && x.GetEditorName() == entry.questVariableName);
                            if (condition != null)
                            {
                                condition.requiredCounterValue.literalValue = syncValue;
                            } // if
                        } // if
                    } // if
                } // foreach
            } // if
        } // SyncCounters()

        private int GetItemTotals(QuestEntryData entry)
        {
            var total = 0;
            foreach (var item in entry.questEntryItems)
            {
                total += item.requiredQty;
            } // foreach

            return total;
        } // GetItemTotals()

        /// <summary>
        /// Return the quest name
        /// </summary>
        /// <returns></returns>
        override public string GetQuestName()
        {
            return _quest.name;
        } // GetQuestName()

        /// <summary>
        /// The quest is considered active if the isActive toggle is checked and the quest is active in the DS database
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            if (_questManager == null) return false;

            return _questManager.IsQuestActive(_quest.name);
        } // IsActive()
    } // class QMQuest
} // namespace Magique.SoulLink

#endif