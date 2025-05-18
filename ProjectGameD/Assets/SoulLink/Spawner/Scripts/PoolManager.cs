using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PoolManager : MonoBehaviour, IPoolManager
    {
        // Dictionary for tracking active spawns of each prefab type used in Spawner. 
        protected Dictionary<Transform, List<Transform>> _activeSpawns = new Dictionary<Transform, List<Transform>>();

        /// <summary>
        /// Spawn
        /// </summary>
        /// <param name="itemPrefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parentTransform"></param>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        /// <param name="origScale"></param>
        /// <returns></returns>
        virtual public Transform Spawn(Transform itemPrefab, Vector3 position, Quaternion rotation, Transform parentTransform, float minScale, float maxScale, Vector3 origScale)
        {
            return null;
        } // Spawn()

        /// <summary>
        /// Despawn
        /// </summary>
        /// <param name="itemPrefab"></param>
        /// <returns></returns>
        virtual public bool Despawn(Transform itemPrefab)
        {
            return false;
        } // Despawn()

        /// <summary>
        /// IsInPoolManager
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        virtual public bool IsInPoolManager(Transform prefab)
        {
            return false;
        } // IsInPoolManager()

        /// <summary>
        /// AddPoolItem
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="populationCap"></param>
        /// <param name="database"></param>
        virtual public void AddPoolItem(Transform prefab, int populationCap, SoulLinkSpawnerDatabase database, bool reusable = true, bool growCapacity = false)
        {
        } // AddPoolItem()

        /// <summary>
        /// UpdatePoolItem
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="populationCap"></param>
        /// <param name="reusable"></param>
        virtual public void UpdatePoolItem(Transform prefab, int populationCap, bool reusable = true)
        {
        } // UpdatePoolItem()

        /// <summary>
        /// ClearPoolItems
        /// </summary>
        /// <param name="database"></param>
        /// <param name="allItems"></param>
        virtual public void ClearPoolItems(SoulLinkSpawnerDatabase database, bool allItems = false)
        {
        } // ClearPoolItems()

        /// <summary>
        /// Cleanup
        /// </summary>
        virtual public void Cleanup()
        {
        } // Cleanup()

        /// <summary>
        /// Checks if any spawn in the scene of the specified type is within a specified range.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="origin"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        virtual public bool AnySpawnInRange(Transform prefab, Vector3 origin, float range)
        {
            if (prefab == null || !_activeSpawns.ContainsKey(prefab)) return false;

            foreach (var spawn in _activeSpawns[prefab])
            {
                if (spawn != null)
                {
                    var spawnable = spawn.GetComponent<ISpawnable>();
                    if (spawnable != null && spawnable.GetSpawnPrefab() == prefab)
                    {
                        if (Vector3.Distance(origin, spawn.position) <= range)
                        {
                            return true;
                        } // if
                    } // if
                } // if
            } // foreach

            return false;
        } // AnySpawnInRange()

        /// <summary>
        /// Add a spawn to the active dictionary
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="instance"></param>
        virtual public void AddActiveSpawn(Transform prefab, Transform instance)
        {
            if (!_activeSpawns.ContainsKey(prefab))
            {
                _activeSpawns[prefab] = new List<Transform>();
            } // if

            _activeSpawns[prefab].Add(instance);
        } // AddActiveSpawn()

        /// <summary>
        /// Remove a spawn from the active dictionary
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="instance"></param>
        virtual public void RemoveActiveSpawn(Transform prefab, Transform instance)
        {
            if (!_activeSpawns.ContainsKey(prefab)) return;

            _activeSpawns[prefab].Remove(instance);
        } // RemoveActiveSpawn()

        /// <summary>
        /// Returns a dictionary of all active spawns for each prefab 
        /// </summary>
        /// <returns></returns>
        virtual public Dictionary<Transform, List<Transform>> GetActiveSpawns()
        {
            return _activeSpawns;
        } // GetActiveSpawns()

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="categoryName"></param>
        virtual public void AddCategory(string categoryName)
        {
        } // AddCategory()

        /// <summary>
        /// Remove an existing category and all its object pools
        /// </summary>
        /// <param name="categoryName"></param>
        virtual public bool RemoveCategory(string categoryName)
        {
            return false;
        } // RemoveCategory()

        /// <summary>
        /// Returns whether or not a category exists in PoolManager
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        virtual public bool CategoryExists(string categoryName)
        {
            // inherited classes should implement properly
            return false;
        } // CategoryExists()
    } // class PoolManager
} //namespace Magique.SoulLink

