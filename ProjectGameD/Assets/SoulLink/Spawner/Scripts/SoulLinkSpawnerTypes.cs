using System;
using System.Collections.Generic;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkSpawnerTypes
    {
        public const string SPAWNER_CATEGORY_PREFIX = "Spawner_";

        public const string DEFAULT_SPAWN_CATEGORY_NAME = "Default";
        public const int DEFAULT_MAX_SPAWNS = 50;

        public const string DEFAULT_TOD_NAME = "Anytime";

        public enum SpawnMode
        {
            SpawnIn3D,
            SpawnIn2D
        }

        public enum SpawnAxes
        {
            XY,
            YZ,
            XZ
        }

        public enum Season
        {
            Spring,
            Summer,
            Fall,
            Winter
        }

        public enum SpawnValidationFailure
        {
            Probability,
            TextureFilter,
            WeatherFilter,
            Elevation,
            SlopeAngle,
            PopulationCap,
            SuppressionArea,
            QuestFilter,
            ProximityFilter,
            SeasonFilter,
            GlobalSnowFilter
        }

        public enum SpawnedBy
        {
            SoulLinkSpawner,
            SoulLinkGlobal
        }

        public enum FilterRule
        {
            Include,
            Exclude
        }

        public enum FilterClause
        {
            Any,
            All
        }

        public enum BlendMode
        {
            Opaque,
            Transparent
        }

        public enum SpawnType
        {
            Prefab,
            Herd,
            Variants
        }

        public enum WeatherCondition
        {
            Temperature, // set this when you want to test for temperature only
            Rain,
            Snow
        }

        public enum TemperatureScale
        {
            Fahrenheit,
            Celsius
        }

        public enum SpawningMethod
        {
            SpawnRadius,
            SpawnBounds,
            SpawnPoints,
            Spline
        }

        public enum SplinePositioning
        {
            Distribution,
            Start,
            End
        }

        public enum SpawningTriggerMethod
        {
            Manual,
            GlobalRadius,
            OverrideRadius,
            Auto
        }

        public const string NAVMESHBUILDER_TAG = "NavMeshBuilder";
        public const string IgnoreBiomesKey = "Ignore Biomes";
    } // class SoulLinkSpawnerTypes

    [System.Serializable]
    public class AudioClipInfo
    {
#if SOULLINK_USE_MASTERAUDIO
            public string clip;
#else
        public AudioClip clip;
#endif
        [Range(0f, 1f)]
        public float volume = 1f;

        public bool is3D = true;
        public float minRange = 3f;
        public float maxRange = 30f;

        public AudioClipInfo()
        {
            volume = 1f;
            is3D = true;
            minRange = 3f;
            maxRange = 30f;
        }
    } // class AudioClipInfo

    [Serializable]
    public class SpawnListData
    {
        public List<SpawnInfoData> spawnInfoData = new List<SpawnInfoData>();
    }

    [Serializable]
    public class SpawnData
    {
        public Transform spawnPrefab;
        public string spawnCategory;
        public Vector3 spawnPos;
        public Quaternion spawnRot = Quaternion.identity;
        public int populationCap;
        public bool ignoreTotalSpawns = false;
        public bool ignorePlayerFOV = false;
        public string timeOfDay;
        public float minScale = 1f;
        public float maxScale = 1f;
        public Vector3 origScale = Vector3.one;
        public List<WeatherRuleData> weatherRules = new List<WeatherRuleData>();
        public List<SeasonRuleData> seasonRules = new List<SeasonRuleData>();
        public List<QuestRuleData> questRules = new List<QuestRuleData>();
        public string spawnGuid;
        public string spawnAreaGuid;
        public string waveSpawnerGuid;
        public string waveItemGuid;
        public bool spawnInAir = false;
        public float minAltitude = 0f;
        public float maxAltitude = 50f;
        public uint randomChance;
        public uint herdID = 0;
        public bool markPersistent = false;
        public bool ignoreRaycast = false;
    } // class SpawnData

    /// <summary>
    /// A simple class to store data on herds that have spawned so that Spawner can properly maintain populations
    /// and manage random numbers from a RandomValuePool
    /// </summary>
    public class HerdData
    {
        public uint herdID;
        public uint randomChance = 0;
        public List<ISpawnable> herdSpawns = new List<ISpawnable>();
    } // class HerdData

    [Serializable]
    public class AnimLayerData
    {
        public int hash;
        // TODO: time?
    }

    [Serializable]
    public class ExtraAIData
    {
        public Quaternion Rotation;
        public List<AnimLayerData> AnimLayerData = new List<AnimLayerData>();
        public int HealthValue;
        public List<string> objectData = new List<string>();
    } // class ExtraAIData

    [Serializable]
    public class SpawnSaveData
    {
        public SpawnData spawnData;
        public ExtraAIData extraAIData;
    } // class SpawnSaveData

    [Serializable]
    public class SpawnSaveDataList
    {
        public List<SpawnSaveData> spawnSaveData = new List<SpawnSaveData>();
    } // class SpawnSaveDataList

    [Serializable]
    public class ProximitySpawnsList
    {
        public List<ProximitySpawnData> proximitySpawns = new List<ProximitySpawnData>();
    } // class ProximitySpawnsList

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        /// <summary>
        /// Cleanuo
        /// </summary>
        public void Cleanup()
        {
            // TODO: need to fix so it won't error when removing from collection
            foreach (var obj in this)
            {
                if (obj.Key.Equals(null))
                {
                    Remove(obj.Key);
                }
            }
        } // Cleanup()

        /// <summary>
        /// Save the dictionary to lists 
        /// </summary>
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            } // foreach
        } // OnBeforeSerialize()

        /// <summary>
        /// OnAfterDeserialize
        /// Load dictionary from lists
        /// </summary>
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
            {
                throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", keys.Count, values.Count));
            } // if

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            } // for i
        } // OnAfterDeserialize()
    } // class SerializableDictionary

    [Serializable]
    public class QuestEntryItemsDictionary : SerializableDictionary<string, QuestEntryItemData> { }

    [Serializable]
    public class SpawnSaveDataDictionary : SerializableDictionary<int, SpawnSaveDataList> { }

    [Serializable]
    public class ProximitySpawnsDictionary : SerializableDictionary<string, ProximitySpawnsList> { }

    [Serializable]
    public class SpawnerDatabaseDictionary : SerializableDictionary<int, SoulLinkSpawnerDatabase> { }

    [Serializable] 
    public class TimeOfDaySpawnsDictionary : SerializableDictionary<string, SpawnListData> { }

    [Serializable]
    public class ObjectPoolDictionary : SerializableDictionary<Transform, ObjectPool> { };

    [Serializable]
    public class PoolCategoryDictionary : SerializableDictionary<string, ObjectPoolDictionary> { };
} // namespace Magique.SoulLink
