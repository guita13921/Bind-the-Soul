using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class QuestRuleData
    {
        public string questName;
        public FilterRule questRule;

        public void CopyFrom(QuestRuleData source)
        {
            questName = source.questName;
            questRule = source.questRule;
        } // CopyFrom()
    } // class QuestRuleData
} // namespace Magique.SoulLink

