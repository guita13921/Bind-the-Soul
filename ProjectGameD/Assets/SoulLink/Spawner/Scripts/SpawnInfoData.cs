using UnityEngine;
using System.Collections.Generic;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class SpawnInfoData
    {
        public SpawnType spawnType = SpawnType.Prefab;
        public Transform prefab;
        public SpawnerHerd herd;

        public string spawnCategory = DEFAULT_SPAWN_CATEGORY_NAME;

        public string variantsName = "";
        public List<Transform> prefabVariants = new List<Transform>();

        public int probability = 70; // in percentage
        public int populationCap = 20;
        public bool ignoreTotalSpawns = false;
        public bool ignorePlayerFOV = false;
        public bool reusable = true;

        public float minScale = 1f;
        public float maxScale = 1f;

        public bool overrideElevation = false;
        public float minElevation = 0f;
        public float maxElevation = 1000f;

        public bool overrideSlopeAngle = false;

        [Range(0,90)]
        public float minSlopeAngle = 0f;
        [Range(0, 90)]
        public float maxSlopeAngle = 45f;

        public bool spawnInAir = false;
        public float minAltitude = 10f;
        public float maxAltitude = 50f;

        public Quaternion spawnRot = Quaternion.identity;

        public List<TextureRuleData> textureRules = new List<TextureRuleData>();
        public List<WeatherRuleData> weatherRules = new List<WeatherRuleData>();
        public List<SeasonRuleData> seasonRules = new List<SeasonRuleData>();
        public List<GlobalSnowRuleData> globalSnowRules = new List<GlobalSnowRuleData>();
        public List<QuestRuleData> questRules = new List<QuestRuleData>();
        public List<ProximityRuleData> proximityRules = new List<ProximityRuleData>();

        public bool disable = false;

        public string spawnGuid;
        public string spawnAreaGuid;
        public string waveSpawnerGuid;
        public string waveItemGuid;
        public uint randomChance; // the random value to use from the RandomValues pool
        public uint herdID; // the herd id to identify the herd this will belong to
        public bool markPersistent = false;
        public bool ignoreRaycast = false; // ignores raycast check for spawn position validation; used for spawn points

        public int textureSelected = 0;
        public int texRuleSelected;
        public int weatherRuleSelected;
        public int seasonRuleSelected;
        public int snowRuleSelected;
        public int questRuleSelected;
        public int proximityRuleSelected;
        public bool texFilterFoldoutState = false;
        public bool weatherFilterFoldoutState = false;
        public bool seasonFilterFoldoutState = false;
        public bool globalFilterSnowFoldoutState = false;
        public bool questFilterFoldoutState = false;
        public bool proximityFilterFoldoutState = false;
        public bool proximitySpawnsFoldoutState = false;
        public bool variantsFoldoutState = false;
        public bool hideSpawn = false;
        public bool showFilters = true;

        public void CopyFrom(SpawnInfoData source, bool copyPrefabs = false, bool copyFilters = false)
        {
            // Copy prefab references if requested
            if (copyPrefabs)
            {
                // Clear all prefab references first
                prefab = null;
                herd = null;
                prefabVariants.Clear();

                spawnType = source.spawnType;

                // Now copy from correct references
                switch (source.spawnType)
                {
                    case SpawnType.Prefab:
                        prefab = source.prefab;
                        break;
                    case SpawnType.Herd:
                        herd = source.herd;
                        break;
                    case SpawnType.Variants:
                        variantsName = source.variantsName;
                        foreach (var variant in source.prefabVariants)
                        {
                            prefabVariants.Add(variant);
                        } // foreach
                        break;
                } // switch
            } // if copyPrefabs

            // Copy filters if requested
            if (copyFilters)
            {
                // Clear all existing filters first
                textureRules.Clear();
                weatherRules.Clear();
                seasonRules.Clear();
                globalSnowRules.Clear();
                questRules.Clear();
                proximityRules.Clear();

                // Now copy filters into this spawn from the source spawn
                foreach (var texRule in source.textureRules)
                {
                    var newRule = new TextureRuleData();
                    newRule.CopyFrom(texRule);
                    textureRules.Add(newRule);
                } // foreach

                foreach (var weatherRule in source.weatherRules)
                {
                    var newRule = new WeatherRuleData();
                    newRule.CopyFrom(weatherRule);
                    weatherRules.Add(newRule);
                } // foreach

                foreach (var seasonRule in source.seasonRules)
                {
                    var newRule = new SeasonRuleData();
                    newRule.CopyFrom(seasonRule);
                    seasonRules.Add(newRule);
                } // foreach

                foreach (var globalSnowRule in source.globalSnowRules)
                {
                    var newRule = new GlobalSnowRuleData();
                    newRule.CopyFrom(globalSnowRule);
                    globalSnowRules.Add(newRule);
                } // foreach

                foreach (var questRule in source.questRules)
                {
                    var newRule = new QuestRuleData();
                    newRule.CopyFrom(questRule);
                    questRules.Add(newRule);
                } // foreach

                foreach (var proximityRule in source.proximityRules)
                {
                    var newRule = new ProximityRuleData();
                    newRule.CopyFrom(proximityRule);
                    proximityRules.Add(newRule);
                } // foreach
            } // if copyFilters

            // Always copy basic spawn info data
            probability = source.probability;
            populationCap = source.populationCap;
            ignoreTotalSpawns = source.ignoreTotalSpawns;
            ignorePlayerFOV = source.ignorePlayerFOV;
            reusable = source.reusable;

            minScale = source.minScale;
            maxScale = source.maxScale;

            overrideElevation = source.overrideElevation;
            minElevation = source.minElevation;
            maxElevation = source.maxElevation;

            overrideSlopeAngle = source.overrideSlopeAngle;
            minSlopeAngle = source.minSlopeAngle;
            maxSlopeAngle = source.maxSlopeAngle;

            spawnInAir = source.spawnInAir;
            minAltitude = source.minAltitude;
            maxAltitude = source.maxAltitude;
    } // CopyFrom()
} // class SpawnInfoData
} // namespace Magique.SoulLink

