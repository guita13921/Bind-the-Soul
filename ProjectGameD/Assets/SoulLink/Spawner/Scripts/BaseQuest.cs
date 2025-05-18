using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BaseQuest : MonoBehaviour, ISoulLinkQuest
    {
        [SerializeField]
        protected List<QuestEntryData> _questEntries = new List<QuestEntryData>();

        protected BaseQuestManager _questManager;

        /// <summary>
        /// Awake
        /// </summary>
        virtual public void Awake()
        {
            _questManager = FindObjectOfType<BaseQuestManager>();
            if (_questManager != null)
            {
                _questManager.RegisterQuest(this);
            } // if
        } // Awake()

        /// <summary>
        /// Register this quest with QuestManager when enabled
        /// </summary>
        virtual public void OnEnable()
        {
            if (_questManager != null)
            {
                _questManager.RegisterQuest(this);
            } // if
        } // OnEnable

        /// <summary>
        /// Unregister this quest with QuestManager when disabled
        /// </summary>
        virtual public void OnDisable()
        {
            if (_questManager != null)
            {
                _questManager.UnregisterQuest(this);
            } // if
        } // OnDisable()

        /// <summary>
        /// GetQuestEntries
        /// </summary>
        /// <returns></returns>
        virtual public List<QuestEntryData> GetQuestEntries()
        {
            return _questEntries;
        } // GetQuestEntries()

        /// <summary>
        /// ContainsPrefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        virtual public bool ContainsPrefab(Transform prefab)
        {
            foreach (var entryItem in _questEntries)
            {
                if (entryItem.questEntryItems.Count > 0 && entryItem.questEntryItems.FindAll(x => x.prefab == prefab) != null)
                {
                    return true;
                } // if
            } // foreach 

            return false;
        } // ContainsPrefab()

        /// <summary>
        /// GetQuestEntries
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        virtual public List<QuestEntryData> GetQuestEntries(Transform prefab)
        {
            List<QuestEntryData> items = new List<QuestEntryData>();

            foreach (var entryItem in _questEntries)
            {
                var entryItems = entryItem.questEntryItems.FindAll(x => x.prefab == prefab);
                foreach (var item in entryItems)
                {
                    if (prefab.Equals(item.prefab))
                    {
                        items.Add(entryItem);
                    } // if
                } // foreach
            } // foreach 

            return items;
        } // GetQuestEntries()

        /// <summary>
        /// Activate
        /// </summary>
        virtual public void Activate()
        {
            throw new System.NotImplementedException();
        } // Activate()

        /// <summary>
        /// Deactivate
        /// </summary>
        virtual public void Deactivate()
        {
            throw new System.NotImplementedException();
        } // Deactivate()

        /// <summary>
        /// IsActive
        /// </summary>
        /// <returns></returns>
        virtual public bool IsActive()
        {
            throw new System.NotImplementedException();
        } // IsActive()

        /// <summary>
        /// GetQuestName
        /// </summary>
        /// <returns></returns>
        virtual public string GetQuestName()
        {
            throw new System.NotImplementedException();
        } // GetQuestName()

        /// <summary>
        /// Returns true if all entries have been marked complete. Each entry can either use a global qty or individual item quantities.
        /// </summary>
        /// <returns></returns>
        public bool AllEntriesComplete()
        {
            bool allComplete = true;
            foreach (var entry in _questEntries)
            {
                if (entry.useGlobalQtyRequired)
                {
                    if (!entry.complete)
                    {
                        allComplete = false;
                        break;
                    } // if
                }
                else
                {
                    if (!entry.questEntryItems.TrueForAll(x => x.complete))
                    {
                        allComplete = false;
                        break;
                    } // if
                }
            } // foreach

            return allComplete;
        } // AllEntriesComplete()

        /// <summary>
        /// Returns the gameObject for this quest component
        /// </summary>
        /// <returns></returns>
        public GameObject GetGameObject()
        {
            return gameObject;
        } // GetGameObject()
    } // class BaseQuest
} // namespace Magique.SoulLink