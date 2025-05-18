using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ISoulLinkQuest
// An interface for integrating with third party quest systems
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISoulLinkQuest
    {
        string GetQuestName();

        void Activate();

        void Deactivate();

        bool IsActive();

        List<QuestEntryData> GetQuestEntries();

        List<QuestEntryData> GetQuestEntries(Transform prefab);

        bool ContainsPrefab(Transform prefab);

        bool AllEntriesComplete();

        GameObject GetGameObject();
    } // interface ISoulLinkQuest
} // namespace Magique.SoulLink

