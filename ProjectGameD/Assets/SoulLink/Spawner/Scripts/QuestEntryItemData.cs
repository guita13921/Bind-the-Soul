using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class QuestEntryItemData
    {
        public Transform prefab;
        public int requiredQty;
        public int currentQty = 0;
        public bool complete = false;
    } // QuestEntryItemData
} // namespace Magique.SoulLink
