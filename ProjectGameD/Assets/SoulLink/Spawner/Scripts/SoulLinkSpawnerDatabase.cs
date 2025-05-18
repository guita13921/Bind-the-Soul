using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// SoulLinkSpawnerDatabase
// A scriptable object to facilitate storage of all global data used in SoulLinkSpawner
//
// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CreateAssetMenu(fileName = "SoulLink Spawner Database", menuName = "SoulLink/Spawner/Database", order = 1)]
    public class SoulLinkSpawnerDatabase : ScriptableObject
    {
        public string version;
        public string author;
        public string description;

        [Tooltip("Define these global time of day ranges for easy usage in your biome spawn data settings.")]
        public List<TimeOfDayData> timeOfDayData = new List<TimeOfDayData>();

        public List<BiomeSpawnData> biomeSpawnData = new List<BiomeSpawnData>();
        public List<AreaData> areaSpawnData = new List<AreaData>();
        public ProximitySpawnsDictionary proximitySpawns = new ProximitySpawnsDictionary();
        public List<SpawnCategoryData> spawnCategories = new List<SpawnCategoryData>();

        /// <summary>
        /// Initialize database with any required data
        /// </summary>
        public void Initialize()
        {
            // If the deafult category does not exist then add it
            if (spawnCategories.Count == 0 || spawnCategories.Find(e => e.categoryName == DEFAULT_SPAWN_CATEGORY_NAME) == null)
            {
                spawnCategories.Add(new SpawnCategoryData() { categoryName = DEFAULT_SPAWN_CATEGORY_NAME, maxSpawns = DEFAULT_MAX_SPAWNS });
            } // if

            // If the default time of day does not exist then add it
            if (timeOfDayData.Count == 0 || timeOfDayData.Find(e => e.timeOfDayName == DEFAULT_TOD_NAME) == null)
            {
                timeOfDayData.Add(new TimeOfDayData() { timeOfDayName = DEFAULT_TOD_NAME, startTime = 0f, endTime = 23.99f });
            } // if
        } // Initialize()

        public int GetProximitySpawnsCount(string spawnGuid)
        {
            if (string.IsNullOrEmpty(spawnGuid) || !proximitySpawns.ContainsKey(spawnGuid)) return 0;
            if (proximitySpawns[spawnGuid].proximitySpawns == null) return 0;

            return proximitySpawns[spawnGuid].proximitySpawns.Count;
        } // GetProximitySpawnsCount()

        public List<ProximitySpawnData> GetProximitySpawns(string spawnGuid)
        {
            if (string.IsNullOrEmpty(spawnGuid) || !proximitySpawns.ContainsKey(spawnGuid)) return null;

            return proximitySpawns[spawnGuid].proximitySpawns;
        } // GetProximitySpawns()

        public void AddProximitySpawn(string spawnGuid, ProximitySpawnData proximitySpawnData)
        {
            if (string.IsNullOrEmpty(spawnGuid)) return;

            if (!proximitySpawns.ContainsKey(spawnGuid))
            {
                proximitySpawns[spawnGuid] = new ProximitySpawnsList();
            }

            proximitySpawns[spawnGuid].proximitySpawns.Add(proximitySpawnData);
        } // AddProximitySpawn()

        public void RemoveProximitySpawn(string spawnGuid, ProximitySpawnData proximitySpawnData)
        {
            if (string.IsNullOrEmpty(spawnGuid)) return;

            if (proximitySpawns.ContainsKey(spawnGuid))
            {
                proximitySpawns[spawnGuid].proximitySpawns.Remove(proximitySpawnData);

                // If no more items in the list then remove this spawn completely from the proximity database dictionary
                if (proximitySpawns[spawnGuid].proximitySpawns.Count == 0)
                {
                    proximitySpawns.Remove(spawnGuid);
                } // if
            } // if
        } // RemoveProximitySpawn()
    } // class SoulLinkSpawnerDatabase
} // namespace Magique.SoulLink
