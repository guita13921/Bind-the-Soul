using System.Collections.Generic;
using UnityEngine;

// Copyright 2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public interface IPoolManager
    {
        Transform Spawn(Transform itemPrefab, Vector3 position, Quaternion rotation, Transform parentTransform, float minScale, float maxScale, Vector3 origScale);

        bool Despawn(Transform itemPrefab);

        bool IsInPoolManager(Transform prefab);

        void AddCategory(string categoryName);

        bool RemoveCategory(string categoryName);

        bool CategoryExists(string categoryName);

        void AddPoolItem(Transform prefab, int populationCap, SoulLinkSpawnerDatabase database, bool reusable = true, bool growCapacity = false);

        void UpdatePoolItem(Transform prefab, int populationCap, bool reusable = true);

        void ClearPoolItems(SoulLinkSpawnerDatabase database, bool allItems = false);

        void AddActiveSpawn(Transform prefab, Transform instance);

        void RemoveActiveSpawn(Transform prefab, Transform instance);

        Dictionary<Transform, List<Transform>> GetActiveSpawns();

        bool AnySpawnInRange(Transform prefab, Vector3 origin, float range);

        void Cleanup();
    } // interface IPoolManager
} // namespace Magique.SoulLink

