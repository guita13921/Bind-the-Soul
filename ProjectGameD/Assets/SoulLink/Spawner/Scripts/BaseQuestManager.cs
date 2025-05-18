using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BaseQuestManager : MonoBehaviour, ISoulLinkQuestManager
    {
        public List<ISoulLinkQuest> Quests
        { get { return _quests; } }
        protected List<ISoulLinkQuest> _quests = new List<ISoulLinkQuest>();

        /// <summary>
        /// ActivateQuest
        /// </summary>
        /// <param name="questName"></param>
        virtual public void ActivateQuest(string questName)
        {
            throw new System.NotImplementedException();
        } // ActivateQuest()

        /// <summary>
        /// DeactivateQuest
        /// </summary>
        /// <param name="questName"></param>
        virtual public void DeactivateQuest(string questName)
        {
            throw new System.NotImplementedException();
        } // DeactivateQuest()

        /// <summary>
        /// OnPrefabDestroy
        /// </summary>
        /// <param name="prefab"></param>
        public void OnPrefabDestroy(Transform prefab)
        {
            // Check for this prefab instance in all active quests.
            foreach (var quest in _quests)
            {
                if (quest.IsActive() && quest.ContainsPrefab(prefab))
                {
                    UpdateQuest(quest, prefab);
                } // if
            } // foreach
        } // OnPrefabDestroy()

        /// <summary>
        /// UpdateQuest
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="prefab"></param>
        virtual public void UpdateQuest(ISoulLinkQuest quest, Transform prefab)
        {
            throw new System.NotImplementedException();
        } // UpdateQuest()

        /// <summary>
        /// RegisterQuest
        /// </summary>
        /// <param name="quest"></param>
        public void RegisterQuest(ISoulLinkQuest quest)
        {
            lock (_quests)
            {
                if (!_quests.Contains(quest))
                {
                    _quests.Add(quest);
                } // if
            } // lock
        } // RegisterQuest()

        /// <summary>
        /// UnregisterQuest
        /// </summary>
        /// <param name="quest"></param>
        public void UnregisterQuest(ISoulLinkQuest quest)
        {
            lock (_quests)
            {
                if (_quests.Contains(quest))
                {
                    _quests.Remove(quest);
                } // if
            } // lock
        } // UnregisterQuest()

        /// <summary>
        /// IsQuestActive
        /// </summary>
        /// <param name="questName"></param>
        /// <returns></returns>
        virtual public bool IsQuestActive(string questName)
        {
            throw new System.NotImplementedException();
        } // IsQuestActive()
    } // class BaseQuestManager
} // namespace Magique.SoulLink
