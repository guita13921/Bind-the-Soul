using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Spawnable")]
    public partial class Spawnable : MonoBehaviour, ISpawnable, ISpawnEvents
    {
        [Tooltip("Reference to the spawn action to perform when spawning/despawning.")]
        [SerializeField]
        private BaseSpawnAction _spawnAction;

        [Tooltip("How long to wait before despawning when out of range or dead.")]
        [SerializeField]
        private float _despawnDelay = 5f;

        [Tooltip("How long to wait between despawn checks.")]
        [SerializeField]
        private float _despawnCheckInterval = 5f;

        [SerializeField]
        private float _despawnRangeBuffer = 15f;

        [Tooltip("Toggle this on if you want to allow despawning even when in combat. Only applies to inherited classes.")]
        [SerializeField]
        protected bool _allowDespawnInCombat = false;

        [Tooltip("Does this spawn require a navmesh?")]
        [SerializeField]
        private bool _requiresNavMesh = true;

        [Tooltip("Will this spawn remain in the scene and not despawn?")]
        [SerializeField]
        private bool _isPersistent = false;

        [Tooltip("Set this value greater than zero to delay enabling the spawn after it has spawned and compelted its spawn action.")]
        [SerializeField]
        private float _delayEnableAgent = 0f;

        [Tooltip("Set this to true if you are receiving nav mesh errors when objects respawn.")]
        [SerializeField]
        private bool _disableAgentOnDespawn = true;

        [SerializeField]
        protected UnityEvent _onAwakeEventHandler;

        [SerializeField]
        protected UnityEvent _onAgentEnabledEventHandler;

        [SerializeField]
        protected UnityEvent _onAgentDisabledEventHandler;

        [SerializeField]
        protected UnityEvent _onSpawnedEventHandler;

        [SerializeField]
        protected UnityEvent _onDespawnedEventHandler;

        protected Dictionary<Material, Material> _materials = new Dictionary<Material, Material>();
        protected List<Renderer> _renderers = new List<Renderer>();

        protected bool _isInitialized = false;
        protected bool _spawned = false;
        protected bool _isDespawning = false;
        protected string _spawnAreaGuid;
        protected string _waveSpawnerGuid;
        protected string _waveItemGuid;
        protected NavMeshAgent _navMeshAgent = null;
        protected bool _isVisible = false;
        protected bool _spawnOutsidePlayerFOV = false;
        protected bool _despawnOutsidePlayerFOV = false;
        protected ExtraAIData _extraAIData = null;
        protected bool _ignoreTotalSpawns = false;
        protected string _spawnCategory = DEFAULT_SPAWN_CATEGORY_NAME;

        public Vector3 OriginalScale
        { 
            get { return _originalScale; } 
            set { _originalScale = value; }
        }

        protected Vector3 _originalScale = Vector3.one;

        public string TimeOfDaySpawned
        {
            get { return _timeOfDaySpawned; }
            set { _timeOfDaySpawned = value; }
        }

        // The time of day that this AI was spawned. When this time of day key is no longer current then the AI can be despawned.
        private string _timeOfDaySpawned;

        // The season rules that this AI was spawned with. When these season rules no longer apply then the AI can be despawned
        private List<SeasonRuleData> _seasonRules = new List<SeasonRuleData>();

        // The weather rules that this AI was spawned with. When these weather rules no longer apply then the AI can be despawned
        private List<WeatherRuleData> _weatherRules = new List<WeatherRuleData>();

        // The quest rules that this AI was spawned with. When these quest rules no longer apply then the AI can be despawned
        private List<QuestRuleData> _questRules = new List<QuestRuleData>();

        public Transform SpawnPrefab
        {
            get { return _spawnPrefab; }
            set { _spawnPrefab = value; }
        }

        protected uint _randomChance;
        protected uint _herdID;
        private Transform _spawnPrefab;
        private SpawnedBy _spawnedBy;
        protected bool _isDead = false;
        static BaseQuestManager _questManager;

        /// <summary>
        /// Start
        /// </summary>
        virtual public void Start()
        {
            if (!_isInitialized && !_spawned)
            {
                if (_spawnAction != null)
                {
                    _spawnAction.FinalSpawnState();
                } // if

                StartCoroutine(DelayEnableAgent());
            } // if
        } // Start()

        /// <summary>
        ///  Awake
        /// </summary>
        virtual public void Awake()
        {
            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            // Call user's custom triggered event handler
            if (_onAwakeEventHandler != null)
            {
                _onAwakeEventHandler.Invoke();
            } // if

            if (_questManager == null)
            {
                _questManager = FindObjectOfType<BaseQuestManager>();
            } // if

            // Assign spawn action event handlers as necessary
            if (_spawnAction != null)
            {
                _spawnAction.GetComponent<BaseSpawnAction>().OnSpawnActionComplete.AddListener(SpawnActionComplete);
                _spawnAction.GetComponent<BaseSpawnAction>().OnDespawnActionComplete.AddListener(DespawnActionComplete);
            } // if
        } // Awake()

        /// <summary>
        /// OnDeath
        /// Assign this handler to your AI's death event.
        /// </summary>
        virtual public void OnDeath()
        {
            _isDead = true;
           
            if (!string.IsNullOrEmpty(_spawnAreaGuid))
            {
                SpawnArea spawnArea = SoulLinkSpawner.Instance.GetSpawnAreaByGuid(_spawnAreaGuid);
                if (spawnArea != null)
                {
                    spawnArea.SpawnDied();
                } // if
            } // if

            if (!string.IsNullOrEmpty(_waveSpawnerGuid))
            {
                WaveSpawner waveSpawner = SoulLinkSpawner.Instance.GetWaveSpawnerByGuid(_waveSpawnerGuid);
                if (waveSpawner != null)
                {
                    waveSpawner.SpawnDied(_waveItemGuid);
                } // if
            } // if

            CheckQuestCompletion();

            StartCoroutine(DelayDespawn());
        } // OnDeath()

        /// <summary>
        /// CheckQuestCompletion
        /// </summary>
        public void CheckQuestCompletion()
        {
            if (_questManager != null)
            {
                _questManager.OnPrefabDestroy(SpawnPrefab);
            }
        } // CheckQuestCompletion()

        /// <summary>
        /// OnSpawned
        /// Automatically called when PoolManager spawns an instance
        /// </summary>
        virtual public void OnSpawned()
        {
            // Call user's custom triggered event handler
            if (_onSpawnedEventHandler != null)
            {
                _onSpawnedEventHandler.Invoke();
            } // if

            _spawned = true;
            _isDead = false;

            // If SpawnManager is generating the nav mesh then we need a Rigidbody on the AI in order to trigger NavMeshLinkActivator components
            if (SoulLinkSpawner.Instance.GenerateNavMesh && GetComponent<Rigidbody>() == null)
            {
                var rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            } // if

            if (_spawnAction != null)
            {
                _spawnAction.PerformSpawnAction();
            }
            else
            {
                SpawnActionComplete();
            }
        } // OnSpawned()

        /// <summary>
        /// OnDespawned
        /// Automatically called when PoolManager despawns an instance
        /// </summary>
        virtual public void OnDespawned()
        {
            // If this is despawning, but is not dead then let the WaveSpawner know so that it can be properly tracked
            WaveSpawner waveSpawner = GetWaveSpawner();
            if (waveSpawner != null && !_isDead)
            {
                waveSpawner.SpawnDespawned(_waveItemGuid);
            } // if

            // Call user's custom triggered event handler
            if (_onDespawnedEventHandler != null)
            {
                _onDespawnedEventHandler.Invoke();
            } // if

            StopCoroutine(CheckForDespawn());
        } // OnDespawned()

        /// <summary>
        /// DelayDespawn
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayDespawn()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(_despawnDelay);

            if (_disableAgentOnDespawn)
            {
                EnableNavMeshAgent(false);
            } // if

            if (_spawnAction != null)
            {
                _spawnAction.PerformDespawnAction();
            }
            else
            {
                DespawnActionComplete(); 
            }
        } // DelayDespawn()

        virtual public void OnSave()
        {
            // inherited classes can implement
        } // OnSave()

        virtual public void OnLoad()
        {
            // inherited classes can implement
        } // OnLoad()

        /// <summary>
        /// SpawnActionComplete
        /// </summary>
        virtual public void SpawnActionComplete()
        {
            if (!_isPersistent)
            {
                StartCoroutine(CheckForDespawn());
            } // if

            StartCoroutine(DelayEnableAgent());
        } // SpawnActionComplete()

        IEnumerator DelayEnableAgent()
        {
            yield return new WaitForSeconds(_delayEnableAgent);

            EnableNavMeshAgent();
        } // DelayEnableAgent()

        virtual public void EnableNavMeshAgent(bool value = true)
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.enabled = value;
            } // if

            if (value)
            {
                // Call user's custom triggered event handler
                if (_onAgentEnabledEventHandler != null)
                {
                    _onAgentEnabledEventHandler.Invoke();
                } // if

                if (_requiresNavMesh && _navMeshAgent != null)
                {
                    StartCoroutine(CheckOffMeshLinks());
                } // if
            }
            else
            { 
                // Call user's custom triggered event handler
                if (_onAgentDisabledEventHandler != null)
                {
                    _onAgentDisabledEventHandler.Invoke();
                } // if

                if (_requiresNavMesh && _navMeshAgent != null)
                {
                    StopCoroutine(CheckOffMeshLinks());
                } // if
            }
        } // EnableNavMeshAgent()

        /// <summary>
        /// DespawnActionComplete
        /// </summary>
        virtual public void DespawnActionComplete()
        {
            if (_spawned)
            {
                if (_spawnedBy == SpawnedBy.SoulLinkGlobal)
                {
                    SoulLinkGlobal.Instance.Despawn(gameObject.transform);
                }
                else
                {
#if SOULLINK_SPAWNER
                    SoulLinkSpawner.Instance.Despawn(transform);
#endif
                }
            }
            else
            {
                Destroy(this.gameObject);
            }

            _timeOfDaySpawned = "";
            _isDespawning = false;
        } // DespawnActionComplete()

        /// <summary>
        /// GetTimeOfDaySpawned
        /// </summary>
        /// <returns></returns>
        public string GetTimeOfDaySpawned()
        {
            return _timeOfDaySpawned;
        } // GetTimeOfDaySpawned()

        /// <summary>
        /// SetTimeOfDaySpawned
        /// </summary>
        /// <param name="timeOfDayName"></param>
        public void SetTimeOfDaySpawned(string timeOfDayName)
        {
            _timeOfDaySpawned = timeOfDayName;
        } // SetTimeOfDaySpawned()

        /// <summary>
        /// Get the qeather rules for this spawnable
        /// </summary>
        /// <returns></returns>
        public List<WeatherRuleData> GetWeatherRules()
        {
            return _weatherRules;
        } // GetWeatherRules()

        /// <summary>
        /// Set the weather rules for this spawnable
        /// </summary>
        /// <param name="weatherRules"></param>
        public void SetWeatherRules(List<WeatherRuleData> weatherRules)
        {
            _weatherRules.Clear();
            _weatherRules.AddRange(weatherRules);
        } // SetWeatherRules()

        /// <summary>
        /// Get the season rules for this spawnable
        /// </summary>
        /// <returns></returns>
        public List<SeasonRuleData> GetSeasonRules()
        {
            return _seasonRules;
        } // GetSeasonRules()

        /// <summary>
        /// Set the season rules for this spawnable
        /// </summary>
        /// <param name="seasonRules"></param>
        public void SetSeasonRules(List<SeasonRuleData> seasonRules)
        {
            _seasonRules.Clear();
            _seasonRules.AddRange(seasonRules);
        } // SetSeasonRules()

        /// <summary>
        /// Get the quest rules for this spawnable
        /// </summary>
        /// <returns></returns>
        public List<QuestRuleData> GetQuestRules()
        {
            return _questRules;
        } // GetQuestRules()

        /// <summary>
        /// Set the Quest Rules for this spawnable
        /// </summary>
        /// <param name="questRules"></param>
        public void SetQuestRules(List<QuestRuleData> questRules)
        {
            _questRules.Clear();
            _questRules.AddRange(questRules);
        } // SetQuestRules()

        /// <summary>
        /// GetSpawnPrefab
        /// </summary>
        /// <returns></returns>
        public Transform GetSpawnPrefab()
        {
            return _spawnPrefab;
        } // GetSpawnPrefab()

        /// <summary>
        /// SetSpawnPrefab
        /// </summary>
        /// <param name="prefab"></param>
        public void SetSpawnPrefab(Transform prefab)
        {
            _spawnPrefab = prefab;
        } // SetSpawnPrefab()

        /// <summary>
        /// Return whether or not this Spawnable is performing a spawn action
        /// </summary>
        /// <returns></returns>
        bool IsPerformingSpawnAction()
        {
            if (_spawnAction == null) return false;
            return (_spawnAction.IsPerforming());
        } // IsPerformingSpawnAction()

        /// <summary>
        /// Checks whether or not an instance in the game world should despawn based on current time of day, weather, etc.
        /// This should only be used for AI that are spawned specifically from the Spawner system
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckForDespawn()
        {
            while (!IsPerformingSpawnAction() && AllowDespawn())
            {
                yield return new WaitForSeconds(_despawnCheckInterval);

                if (!AllowDespawn()) break;

                if (GetDespawnOutsidePlayerFOV() && _isVisible) continue;

                // If an AI is no longer visible then we can despawn it if it is invalid based on Time of day filter or range
                if (Vector3.Distance(SoulLinkSpawner.Instance.PlayerTransform.position, transform.position) > SoulLinkSpawner.Instance.SpawnRadius + _despawnRangeBuffer)
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Despawning due to Out of Range.");
                    StartCoroutine(DelayDespawn());
                    continue;
                } // if

                // If an AI is invalid based on Time of day filter, despawn it
                if (!SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(_timeOfDaySpawned))
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Despawning due to Time of Day.");
                    StartCoroutine(DelayDespawn());
                    continue;
                } // if

                if (!SoulLinkSpawner.Instance.IsWithinSeasonParameters(_seasonRules))
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Despawning due to Season.");
                    StartCoroutine(DelayDespawn());
                    continue;
                } // if

                if (!SoulLinkSpawner.Instance.IsInCurrentWeatherConditions(_weatherRules, transform.position.y))
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Despawning due to Weather.");
                    StartCoroutine(DelayDespawn());
                    continue;
                }

                if (!SoulLinkSpawner.Instance.IsQuestStatusValid(_questRules))
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Despawning due to Quest Status.");
                    StartCoroutine(DelayDespawn());
                    continue;
                }
            } // while
        } // CheckForDespawn()

        /// <summary>
        /// Returns whether or not this spawnable requires a nav mesh in order to spawn
        /// </summary>
        /// <returns></returns>
        public bool GetRequiresNavMesh()
        {
            return _requiresNavMesh;
        } // GetRequiresNavMesh()

        /// <summary>
        /// Set whether or not this spawnable requires a nav mesh
        /// </summary>
        /// <param name="value"></param>
        public void SetRequiresNavMesh(bool value)
        {
            _requiresNavMesh = value;
        } // SetRequiresNavMesh()

        /// <summary>
        /// Returns whether or not this spawn should persist in the scene instead of despawning when conditions allow.
        /// </summary>
        /// <returns></returns>
        public bool GetIsPersistent()
        {
            return _isPersistent;
        } // GetIsPersistent()

        /// <summary>
        /// Set whether or not this spawn should remain persistent in the scene instead of despawning when conditions allow.
        /// </summary>
        /// <param name="value"></param>
        public void SetIsPersistent(bool value)
        {
            _isPersistent = value;
        } // SetIsPersistent()

        /// <summary>
        /// Gets whether or not this spawnable ignores the total spawns counted for the scene
        /// </summary>
        /// <returns></returns>
        public bool GetIgnoreTotalSpawns()
        {
            return _ignoreTotalSpawns;
        } // GetIgnoreTotalSpawns()

        /// <summary>
        /// Set whether or not this spawnable ignores the total spawns counted for the scene
        /// </summary>
        /// <param name="value"></param>
        public void SetIgnoreTotalSpawns(bool value)
        {
            _ignoreTotalSpawns = value;
        } // SetIgnoreTotalSpawns()

        /// <summary>
        /// SetOriginalScale
        /// </summary>
        /// <param name="value"></param>
        public void SetOriginalScale(Vector3 value)
        {
            _originalScale = value;
        } // SetOriginalScale()

        /// <summary>
        /// Return the original scale of the prefab
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOriginalScale()
        {
            return _originalScale;
        } // GetOriginalScale()

        public void SetSpawnArea(string spawnAreaGuid)
        {
            _spawnAreaGuid = spawnAreaGuid;
        } // SetSpawnArea()

        /// <summary>
        /// Get the spawn area by using its Guid
        /// </summary>
        /// <returns></returns>
        public SpawnArea GetSpawnArea()
        {
            return SoulLinkSpawner.Instance.GetSpawnAreaByGuid(_spawnAreaGuid);
        } // GetSpawnArea()

        /// <summary>
        /// Get the spawn area Guid
        /// </summary>
        /// <returns></returns>
        public string GetSpawnAreaGuid()
        {
            return _spawnAreaGuid;
        } // GetSpawnAreaGuid()

        /// <summary>
        /// Get the wave spawner Guid
        /// </summary>
        /// <returns></returns>
        public void SetWaveSpawner(string waveSpawnerGuid, string waveItemGuid)
        {
            _waveSpawnerGuid = waveSpawnerGuid;
            _waveItemGuid = waveItemGuid;
        } // SetWaveSpawner()

        /// <summary>
        /// Get the wave spawner Guid
        /// </summary>
        /// <returns></returns>
        public string GetWaveSpawnerGuid()
        {
            return _waveSpawnerGuid;
        } // GetWaveSpawnerGuid()

        /// <summary>
        /// Get the wave item Guid
        /// </summary>
        /// <returns></returns>
        public string GetWaveItemGuid()
        {
            return _waveItemGuid;
        } // GetWaveItemGuid()

        /// <summary>
        /// Get the wave spawner by using its Guid
        /// </summary>
        /// <returns></returns>
        public WaveSpawner GetWaveSpawner()
        {
            return SoulLinkSpawner.Instance.GetWaveSpawnerByGuid(_waveSpawnerGuid);
        } // GetWaveSpawner()

        /// <summary>
        /// CheckOffMeshLinks
        /// </summary>
        /// <returns></returns>
        protected IEnumerator CheckOffMeshLinks()
        {
            _navMeshAgent.autoTraverseOffMeshLink = false;
            while (true)
            {
                if (_navMeshAgent.isOnOffMeshLink)
                {
                    OffMeshLinkData data = _navMeshAgent.currentOffMeshLinkData;
                    Vector3 endPos = data.endPos + Vector3.up * _navMeshAgent.baseOffset;

                    float elapsed = 0f;
                    float delay = _navMeshAgent.speed > 0f ? (Time.deltaTime / _navMeshAgent.speed) : 0f;
                    while (elapsed < delay)
                    {
                        _navMeshAgent.transform.position = Vector3.MoveTowards(_navMeshAgent.transform.position, endPos, _navMeshAgent.speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();

                        elapsed += Time.deltaTime;
                    } // while

                    _navMeshAgent.CompleteOffMeshLink();
                } // if
                yield return null;
            } // while
        } // CheckOffMeshLinks()

        /// <summary>
        /// SetSpawnedBy
        /// </summary>
        /// <param name="spawnedBy"></param>
        public void SetSpawnedBy(SpawnedBy spawnedBy)
        {
            _spawnedBy = spawnedBy;
        } // SetSpawnedBy()

        /// <summary>
        /// GetSpawnedBy
        /// </summary>
        /// <returns></returns>
        public SpawnedBy GetSpawnedBy()
        {
            return _spawnedBy;
        } // GetSpawnedBy()

        /// <summary>
        /// Return whether or not this spawn is allowed to despawn
        /// </summary>
        /// <returns></returns>
        virtual public bool AllowDespawn()
        {
            return true;
        } // AllowDespawn()

        /// <summary>
        /// This is called when the spawn becomes invisible to the camera
        /// </summary>
        private void OnBecameInvisible()
        {
            _isVisible = false;
        } // OnBecameInvisible()

        /// <summary>
        /// This is called when the spawn becomes visible to the camera
        /// </summary>
        private void OnBecameVisible()
        {
            _isVisible = true;
        } // OnBecameVisible()

        /// <summary>
        /// Set whether or not this unit can spawn only when outside the player's FOV.
        /// </summary>
        /// <param name="value"></param>
        public void SetSpawnOutsidePlayerFOV(bool value)
        {
            _spawnOutsidePlayerFOV = value;
        } // SetSpawnOutsidePlayerFOV()

        /// <summary>
        /// Returns whether or not this unit can spawn only when outside the player's FOV.
        /// </summary>
        /// <returns></returns>
        public bool GetSpawnOutsidePlayerFOV()
        {
            return _spawnOutsidePlayerFOV;
        } // GetSpawnOutsidePlayerFOV()

        /// <summary>
        /// Set whether or not this unit can despawn only when outside the player's FOV.
        /// </summary>
        /// <param name="value"></param>
        public void SetDespawnOutsidePlayerFOV(bool value)
        {
            _despawnOutsidePlayerFOV = value;
        } // SetDespawnOutsidePlayerFOV()

        /// <summary>
        /// Returns whether or not this unit can despawn only when outside the player's FOV.
        /// </summary>
        /// <returns></returns>
        public bool GetDespawnOutsidePlayerFOV()
        {
            return _despawnOutsidePlayerFOV;
        } // GetDespawnOutsidePlayerFOV()

        public float GetDespawnDelay()
        {
            return _despawnDelay;
        } // GetDespawnDelay()

        public void SetDespawnDelay(float value)
        {
            _despawnDelay = value;
        } // SetDespawnDelay()

        public float GetDespawnCheckInterval()
        {
            return _despawnCheckInterval;
        } // GetDespawnCheckInterval()

        public void SetDespawnCheckInterval(float value)
        {
            _despawnCheckInterval = value;
        } // SetDespawnCheckInterval()

        public float GetDespawnRangeBuffer()
        {
            return _despawnRangeBuffer;
        } // GetDespawnRangeBuffer()

        public void SetDespawnRangeBuffer(float value)
        {
            _despawnRangeBuffer = value;
        } // SetDespawnRangeBuffer()

        /// <summary>
        /// Force this spawn to start despawning
        /// </summary>
        public void ForceDespawn()
        {
            if (_isDespawning) return;
            StartCoroutine(DelayDespawn());
        } // ForceDespawn()

        /// <summary>
        /// Import settings from a SpawnableSettingsObject
        /// </summary>
        /// <param name="spawnableSettings"></param>
        public void ImportSettings(SpawnableSettingsObject spawnableSettings)
        {
            Utility.ImportSpawnableSettings(this, spawnableSettings);
        } // ImportSettings()

        /// <summary>
        /// Export settings to a SpawnableSettingsObject
        /// </summary>
        public void ExportSettings()
        {
            Utility.ExportSpawnableSettings(this);
        } // ExportSettings()

        /// <summary>
        /// Set the extra saved ai data so it can be restored at the proper time for each AI system
        /// </summary>
        /// <param name="extraAIData"></param>
        public void SetExtraAIData(ExtraAIData extraAIData)
        {
            _extraAIData = extraAIData;
        } // SetExtraAIData()

        /// <summary>
        /// Stores the random chance value used to spawn this item
        /// </summary>
        /// <param name="value"></param>
        public void SetRandomChance(uint value)
        {
            _randomChance = value;
        } // SetRandomChance()

        /// <summary>
        /// Returns the random chance value that was used to spawn this item
        /// </summary>
        /// <returns></returns>
        public uint GetRandomChance()
        {
            return _randomChance;
        } // GetRandomChance()

        /// Stores the herd ID value to identify which herd this belongs to
        /// </summary>
        /// <param name="value"></param>
        public void SetHerdID(uint value)
        {
            _herdID = value;
        } // SetHerdID()

        /// <summary>
        /// Returns the herd ID that identifies which herd this belongs to
        /// </summary>
        /// <returns></returns>
        public uint GetHerdID()
        {
            return _herdID;
        } // GetHerdID()

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GameObject GetGameObject()
        {
            return gameObject;
        } // GetGameObject()

        /// <summary>
        /// Set the spawn action to use for this spawnable
        /// </summary>
        /// <param name="spawnAction"></param>
        public void SetSpawnAction(BaseSpawnAction spawnAction)
        {
            _spawnAction = spawnAction;
        } // SetSpawnAction()

        /// <summary>
        /// Set the spawn category for this spawnable
        /// </summary>
        /// <param name="categoryName"></param>
        public void SetSpawnCategory(string categoryName)
        {
            _spawnCategory = categoryName;
        } // SetSpawnCategory(

        /// <summary>
        /// Get the spawn category for this spawnable
        /// </summary>
        /// <returns></returns>
        public string GetSpawnCategory()
        {
            return _spawnCategory;
        } // GetSpawnCategory()
    } // class Spawnable
} // namespace Magique.SoulLink
