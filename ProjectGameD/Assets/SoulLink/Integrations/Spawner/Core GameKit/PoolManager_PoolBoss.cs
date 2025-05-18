#if SOULLINK_USE_POOLBOSS
using DarkTonic.CoreGameKit;
using UnityEngine;
using UnityEngine.AI;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PoolManager_PoolBoss : PoolManager
    {
        private string POOLBOSS_CATEGORY_PREFIX = "Spawner_";

        override public Transform Spawn(Transform itemPrefab, Vector3 position, Quaternion rotation, Transform parentTransform, float minScale, float maxScale, Vector3 origScale)
        {
            var instance = PoolBoss.Spawn(itemPrefab, position, rotation, parentTransform);
            var scale = (minScale != 1f || maxScale != 1f) ? Random.Range(minScale, maxScale) : 1f;
            instance.localScale = new Vector3(itemPrefab.localScale.x * scale, itemPrefab.localScale.y * scale, itemPrefab.localScale.x * scale);

            var spawnable = instance.GetComponent<ISpawnable>();
            if (spawnable != null)
            {
                spawnable.SetOriginalScale(origScale);
            }
            else
            {
                Debug.LogError("PoolManager_PoolBoss Error: " + instance.name + " prefab does not have an ISpawnable interface.");
            }

            AddActiveSpawn(itemPrefab, instance);

            // Inform the SpawnArea that a spawn was successfully added
            SpawnArea spawnArea = parentTransform != null ? parentTransform.gameObject.GetComponent<SpawnArea>() : null;
            if (spawnArea != null)
            {
                spawnArea.SpawnAdded();
            } // if

            return instance;
        } // Spawn()

        override public bool Despawn(Transform itemPrefab)
        {
            PoolableInfo pinfo = itemPrefab.GetComponent<PoolableInfo>();

            PoolBossItem poolItem = PoolBoss.Instance.poolItems.Find(e => e.gameObject.name == pinfo.poolItemName);
            RemoveActiveSpawn(poolItem.prefabTransform, itemPrefab);

            return PoolBoss.Despawn(itemPrefab);
        } // Despawn()

        override public bool IsInPoolManager(Transform prefab)
        {
            return (PoolBoss.Instance.poolItems.Find(e => e.prefabTransform == prefab) != null);
        } // IsInPoolManager()

        override public void AddPoolItem(Transform prefab, int populationCap, SoulLinkSpawnerDatabase database, bool reusable = true, bool growCapacity = false)
        {
            if (prefab == null) return;

            // Add it to PoolBoss if it dosn't already exist; else update the values as needed
            if (!IsInPoolManager(prefab))
            {
                var categoryName = POOLBOSS_CATEGORY_PREFIX + database.name;
                CreateNewPoolBossItem(prefab, categoryName, populationCap, growCapacity);
                UpdatePoolItem(prefab, populationCap, reusable); // to set the allowRecycle flag
            }
            else
            {
                UpdatePoolItem(prefab, populationCap, reusable);
            }
        } // AddPoolItem()

        void CreateNewPoolBossItem(Transform prefab, string catName, int populationCap, bool growCapacity = false)
        {
            if (PoolBoss.Instance._categories.Find(e => e.CatName == catName) == null)
            {
                var newCategory = new PoolBossCategory();
                newCategory.CatName = catName;
                PoolBoss.Instance._categories.Add(newCategory);
            } // if

            // Automatically determine if the nav mesh agent needs to be enabled or not based on the prefab's current component setting
            var navMeshAgent = prefab.GetComponent<NavMeshAgent>();

            var newItem = new PoolBossItem()
            {
                prefabTransform = prefab,
                instancesToPreload = populationCap,
                allowInstantiateMore = growCapacity,
                itemHardLimit = populationCap,
                isExpanded = true,
                logMessages = false,
                categoryName = catName,
                prefabSource = new PoolBoss.PrefabSource(),
                isTemporary = false,
                enableNavMeshAgentOnSpawn = (navMeshAgent != null) ? !navMeshAgent.enabled : false
            };

            if (string.IsNullOrEmpty(catName))
            {
                newItem.categoryName = PoolBoss.Instance._categories[0].CatName;
            } // if

            PoolBoss.Instance.poolItems.Add(newItem);
        } // CreateNewPoolBossItem()

        override public void UpdatePoolItem(Transform prefab, int populationCap, bool reusable = true)
        {
            var poolItem = PoolBoss.Instance.poolItems.Find(e => e.prefabTransform == prefab);
            if (poolItem != null)
            {
                // Update pool item if the parameters in this spawn item require a change
                if (poolItem.instancesToPreload < populationCap)
                {
                    poolItem.instancesToPreload = populationCap;
                    poolItem.itemHardLimit = populationCap;
                }

                poolItem.allowRecycle = reusable;
            } // if
        } // UpdatePoolItem()

        override public void ClearPoolItems(SoulLinkSpawnerDatabase database, bool allItems = false)
        {
            // For editor use only so just return ifthe app is playing
            if (Application.isPlaying) return;

            var categoryName = POOLBOSS_CATEGORY_PREFIX + database.name;
            for (int i = 0; i < PoolBoss.Instance.poolItems.Count; ++i)
            {
                if (PoolBoss.Instance.poolItems[i].categoryName == categoryName)
                {
                    PoolBoss.Instance.poolItems.RemoveAt(i);
                } // if
            } // for i
        } // ClearPoolItems()

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="categoryName"></param>
        override public void AddCategory(string categoryName)
        {
            // Not needed
        } // AddCategory()

        /// <summary>
        /// Remove an existing category and all its object pools
        /// </summary>
        /// <param name="categoryName"></param>
        override public bool RemoveCategory(string categoryName)
        {
            int removeCount = 0;
            for (int i = 0; i < PoolBoss.Instance.poolItems.Count; ++i)
            {
                if (PoolBoss.Instance.poolItems[i].categoryName == categoryName)
                {
                    PoolBoss.Instance.poolItems.RemoveAt(i);
                    ++removeCount;
                } // if
            } // for i

            return (removeCount > 0);
        } // RemoveCategory()

        /// <summary>
        /// Returns whether or not a category exists in PoolManager
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        override public bool CategoryExists(string categoryName)
        {
            for (int i = 0; i < PoolBoss.Instance.poolItems.Count; ++i)
            {
                if (PoolBoss.Instance.poolItems[i].categoryName == categoryName) return true;
            } // for i

            return false;
        } // CategoryExists()
    } // class PoolManager_PoolBoss
} // namespace Magique.SoulLink
#endif