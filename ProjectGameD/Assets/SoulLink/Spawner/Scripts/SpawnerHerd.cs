using System.Collections.Generic;
using UnityEngine;

// SpawnerHerd
// A scriptable object storing Herd data
//
// (c)2021 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CreateAssetMenu(fileName = "SpawnerHerd", menuName = "SoulLink/Spawner/Herd", order = 2)]
    public class SpawnerHerd : ScriptableObject
    {
        public string herdName;
        public List<SpawnInfoData> herdPrefabs = new List<SpawnInfoData>();
        public int minPopulation;
        public int maxPopulation;
        public float spawnRadius;
        public bool foldoutState = false;
    } // class SpawnerHerd
} // namespace Magique.SoulLink