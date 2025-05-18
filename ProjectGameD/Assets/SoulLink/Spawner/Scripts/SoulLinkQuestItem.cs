using System.Collections;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Quest Item")]
    public class SoulLinkQuestItem : MonoBehaviour, ISpawnEvents
    {
        [SerializeField]
        private Transform _originalPrefab;

        static BaseQuestManager _questManager;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            if (_questManager == null)
            {
                _questManager = FindObjectOfType<BaseQuestManager>();
            } // if
        } // Awake()

        /// <summary>
        /// OnSpawned handler
        /// </summary>
        public void OnSpawned()
        {
            StartCoroutine(AfterSpawned());
        } // OnSpawned()

        /// <summary>
        /// Assign original prefab from spawnable one frame after spawning
        /// </summary>
        /// <returns></returns>
        IEnumerator AfterSpawned()
        {
            yield return null;

            if (_originalPrefab == null)
            {
                var spawnable = gameObject.GetComponent<ISpawnable>();
                if (spawnable != null)
                {
                    _originalPrefab = spawnable.GetSpawnPrefab();
                } // if
            } // if
        } // AfterSpawned()

        /// <summary>
        ///  Check for quest completion when item is destroyed
        /// </summary>
        private void OnDestroy()
        {
            CheckQuestCompletion();
        } // OnDestroy()

        /// <summary>
        /// Check for quest completion on despawn
        /// </summary>
        public void OnDespawned()
        {
            CheckQuestCompletion();
        } // OnDespawned()

        /// <summary>
        /// CheckQuestCompletion
        /// </summary>
        public void CheckQuestCompletion()
        {
            if (_questManager != null)
            {
                _questManager.OnPrefabDestroy(_originalPrefab);
            } // if
        } // CheckQuestCompletion()
    } // class SoulLinkQuestItem
} // namespace Magique.SoulLink
