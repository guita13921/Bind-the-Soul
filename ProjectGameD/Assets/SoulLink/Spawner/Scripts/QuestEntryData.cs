using System.Collections.Generic;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class QuestEntryData
    {
        public string questEntryName;
        public string questVariableName;
        public bool useGlobalQtyRequired = false;
        public int globalQtyRequired;
        public List<QuestEntryItemData> questEntryItems = new List<QuestEntryItemData>();
        public bool complete = false;
        public bool itemFoldoutState = true;
    } // class QuestEntryData
} // namespace Magique.SoulLink

