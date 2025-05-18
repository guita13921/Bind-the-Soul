using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ISoulLinkQuestManager
// An interface for integrating with third party quest systems
//
// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface ISoulLinkQuestManager
    {
        void RegisterQuest(ISoulLinkQuest quest);

        void UnregisterQuest(ISoulLinkQuest quest);

        void ActivateQuest(string questName);

        void DeactivateQuest(string questName);

        void OnPrefabDestroy(Transform prefab);

        bool IsQuestActive(string questName);
    } // interface ISoulLinkQuestManager
} // namespace Magique.SoulLink

