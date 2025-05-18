#if SOULLINK_USE_DS || SOULLINK_USE_QM && SOULLINK_SPAWNER
using System;
using System.Collections.Generic;
using PixelCrushers;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

#if EMERALD_AI_PRESENT
using EmeraldAI;
#endif

#if INVECTOR_AI_TEMPLATE
using Invector;
#endif

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkSpawner_Saver : Saver
    {
            [Tooltip("How long in game time hours to wait before respawning game world instead of restoring saved data.")]
            [SerializeField]
            private float _timeToRespawn = 1f;

            [Tooltip("Toggle on if you always want to restore saved data for persistent spawns regardless of how much time has passed.")]
            [SerializeField]
            private bool _alwaysRestorePersistentSpawns = true;
        
        [Serializable]
        public class SoulLinkSpawnerData
        {
            public SpawnerDatabaseDictionary databases = new SpawnerDatabaseDictionary();
            public SpawnSaveDataDictionary savedSpawns = new SpawnSaveDataDictionary();
        } // class SoulLinkSpawnerData

        protected SoulLinkSpawnerData m_data;
        private PoolManager _poolManager;
        private int _sceneIndex;
        private string _savedData;
        private bool _dataApplied = false;
        private bool _shouldRespawn = false;
        private bool _gameTimeLoaded = false;

        public override void Awake()
        {
            base.Awake();
            m_data = new SoulLinkSpawnerData();

            _poolManager = FindObjectOfType<PoolManager>();

            _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        } // Awake()

        public override void ApplyData(string s)
        {
            // Save off the data and wait until game time data has been restored
            _savedData = s;
            _dataApplied = true;

            if (_gameTimeLoaded)
            {
                SpawnSavedItems();
            } // if
        } // ApplyData()

        public void OnGameTimeLoaded(float timePassed)
        {
            _gameTimeLoaded = true;
            _shouldRespawn = (timePassed >= _timeToRespawn);

            if (_dataApplied)
            {
                SpawnSavedItems();
            } // if
        } //OnGameTimeLoaded()

        void SpawnSavedItems()
        {
            SoulLinkGlobal.DebugLog(gameObject, "Loading active spawn data.");

            // Update from saved data
            if (!string.IsNullOrEmpty(_savedData))
            {
                var data = SaveSystem.Deserialize(_savedData, m_data);
                if (data == null) return;

                m_data = data;

                // Assign the correct database if one has been saved for this scene
                if (m_data.databases.ContainsKey(_sceneIndex))
                {
                    SoulLinkSpawner.Instance.Database = m_data.databases[_sceneIndex];
                } // if

                // Query only for this scene's saved active spawns
                var query =
                    from spawnData in m_data.savedSpawns.Values
                    where m_data.savedSpawns.ContainsKey(_sceneIndex)
                    select spawnData;

                foreach (var spawns in query)
                {
                    for (int i = 0; i < spawns.spawnSaveData.Count(); ++i)
                    {
                        if (!_shouldRespawn || (spawns.spawnSaveData[i].spawnData.markPersistent && _alwaysRestorePersistentSpawns))
                        {
                            var spawn = SoulLinkSpawner.Instance.Spawn(spawns.spawnSaveData[i].spawnData);
                            if (spawn != null)
                            {
                                var spawnable = spawn.GetComponent<ISpawnable>();
                                if (spawnable != null)
                                {
                                    spawnable.SetExtraAIData(spawns.spawnSaveData[i].extraAIData);
                                    spawnable.OnLoad();
                                } // if
                            } // if
                        } // if
                    } // for i
                } // forach
            } // if
        } // SpawnSavedItems()

        /// <summary>
        /// Save data to persistent storage
        /// </summary>
        /// <returns></returns>
        public override string RecordData()
        {
            SoulLinkGlobal.DebugLog(gameObject, "Saving active spawn data.");

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            m_data.savedSpawns.Remove(sceneIndex);

            // Must save the current database since it can change at runtime based on regions
            m_data.databases[_sceneIndex] = SoulLinkSpawner.Instance.Database;

            // Fetch active spawns from PoolManager and serialize their spawn data in a list
            foreach (var spawnType in _poolManager.GetActiveSpawns())
            {
                // Enumerate through each individual spawn in this type
                foreach (var spawn in spawnType.Value)
                {
                    ISpawnable spawnable = spawn.GetComponent<ISpawnable>();
                    if (spawnable == null)
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "No ISpawnable interface for " + spawn.name);
                        continue;
                    }
                    var newSpawnData = new SpawnData()
                    { spawnPrefab = spawnable.GetSpawnPrefab(), spawnPos = spawn.position, spawnAreaGuid = spawnable.GetSpawnAreaGuid(), 
                        timeOfDay = spawnable.GetTimeOfDaySpawned(), minScale = spawn.localScale.x, maxScale = spawn.localScale.x, 
                        origScale = spawnable.GetOriginalScale(), seasonRules = spawnable.GetSeasonRules(), weatherRules = spawnable.GetWeatherRules()
                    };

                    // Check for AI integrations and store their AI data as necessary
                    ExtraAIData newExtraAIData = new ExtraAIData();

                    var animator = spawn.GetComponent<Animator>();
                    if (animator != null)
                    {
                        for (int i = 0; i < animator.layerCount; ++i)
                        {
                            newExtraAIData.AnimLayerData.Add(new AnimLayerData() { hash = animator.GetCurrentAnimatorStateInfo(i).fullPathHash });
                        } // for i
                    } // if

                    newExtraAIData.Rotation = spawn.rotation;

#if EMERALD_AI_PRESENT
                    var aiSystem = spawn.GetComponent<EmeraldAISystem>();
                    if (aiSystem != null)
                    {
                        newExtraAIData.HealthValue = aiSystem.CurrentHealth;
                    } // if
#endif

#if INVECTOR_AI_TEMPLATE
                    var health = spawn.GetComponent<vHealthController>();
                    if (health != null)
                    {
                        newExtraAIData.HealthValue = (int)health.currentHealth;
                    } // if
#endif

#if SOULLINK_USE_GKC
                    var health = spawn.GetComponentInChildren<health>();
                    if (health != null)
                    {
                        newExtraAIData.HealthValue = health.healthAmount;
                    } // if
#endif

                    // Get spawnable specific serialized data for more comprehensive save
                    ISpawnableSerializer serializer = spawn.GetComponent<ISpawnableSerializer>();
                    if (serializer != null)
                    {
                        newExtraAIData.objectData = serializer.SerializeData();
                    } // if

                    // Store SpawnData and ExtraAIData together into SpawnSaveData by scene index
                    if (!m_data.savedSpawns.ContainsKey(sceneIndex))
                    {
                        m_data.savedSpawns[sceneIndex] = new SpawnSaveDataList();
                    }

                    m_data.savedSpawns[sceneIndex].spawnSaveData.Add(new SpawnSaveData() { spawnData = newSpawnData, extraAIData = newExtraAIData } );

                    spawnable.OnSave();
                } // foreach spawn
            } // foreach spawnType

            return SaveSystem.Serialize(m_data);
        } // RecordData()
    } // class SoulLinkSpawner_Saver
} // namespace Magique.SoulLink
#endif