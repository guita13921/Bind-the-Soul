using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// SpawnArea
// This component is used to spawn AI around specific areas in the game world.
//
// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Spawn Area")]
    public class SpawnArea : MonoBehaviour, ISoulLinkGuid
    {
        public enum ResetKind
        {
            OutOfRange,         // When the player exits the trigger area
            GameTimeDuration,   // Reset time is in game time hours
            RealtimeDuration,   // Reset time is in seconds
            Elimination         // When all spawns have been defeated/despawned
        }

        public enum ResetRepeatKind
        {
            Unlimited,
            UserDefined
        }

        [System.Serializable]
        public class SpawnPoint
        {
            public Transform transform;
            public bool useRadius = false;
            public float spawnRadius = 2f;
        } // class SpawnPoint

        public int WaveNumber
        {
            get { return _totalResets + 1; }
        }

        public string AreaName
        {
            get { return _areaName; }
            set { _areaName = value; }
        }
        [SerializeField]
        private string _areaName;

        [Tooltip("Check this if you want to define your spawn information here instead of in the SoulLinkSpawner database.")]
        [SerializeField]
        private bool _useLocalSpawnData = false;

        public TimeOfDaySpawnsDictionary TimeOfDaySpawns
        {
            get { return _timeOfDaySpawns; }
        }
        [SerializeField]
        private TimeOfDaySpawnsDictionary _timeOfDaySpawns = new TimeOfDaySpawnsDictionary();

        public bool OverrideTriggerRadius
        { get { return _overrideTriggerRadius; } }

        [TooltipAttribute("Check this if you don't want your area to start spawning at the global spawn radius.")]
        [SerializeField]
        private bool _overrideTriggerRadius = false;

        public float TriggerRadius
        {
            get { return _triggerRadius; }
            set { _triggerRadius = value; }
        }

        [TooltipAttribute("The distance from the area the player must be to trigger the spawns for this area.")]
        [SerializeField]
        private float _triggerRadius = 30f;

        public bool ManualTrigger
        {
            get { return _manualTrigger; }
            set { _manualTrigger = value; }
        }

        [TooltipAttribute("Check this if you want to control spawn trigger via code. This will ignore all collision triggers.")]
        [SerializeField]
        private bool _manualTrigger = false;

        public SpawningMethod SpawnMethod
        {
            get { return _spawningMethod; }
            set { _spawningMethod = value; }
        }

        [TooltipAttribute("Select whether or not spawning will occur withint a spawn radius or withint on ore more defined bounding volumes.")]
        [SerializeField]
        private SpawningMethod _spawningMethod = SpawningMethod.SpawnRadius;

        [Tooltip("Select where to spawn items along the spline.")]
        [SerializeField]
        private SplinePositioning _splinePositioning = SplinePositioning.Distribution;

        public float SpawnRadius
        { get { return _spawnRadius; } }

        [TooltipAttribute("The maximum distance to spawn AI from the area.")]
        [SerializeField]
        private float _spawnRadius = 15f;

        public float MinimumSpawnRange
        { get { return _minimumSpawnRange; } }

        [TooltipAttribute("The minimum distance an AI can spawn from the area.")]
        [SerializeField]
        private float _minimumSpawnRange = 0f;

        [TooltipAttribute("How close spawns of the same type can be clustered. This value is modified by spawn radius and population cap.")]
        [SerializeField]
        private float _clusterRange = 1f;

        [SerializeField]
        private List<Bounds> _spawnBounds = new List<Bounds>();

        public List<SpawnAreaBounds> SpawnAreaBounds
        {
            get { return _spawnAreaBounds; }
        }

        [SerializeField]
        private List<SpawnAreaBounds> _spawnAreaBounds = new List<SpawnAreaBounds>();

        public List<SpawnPoint> SpawnPoints
        {
            get { return _spawnPoints; }
        }

        [SerializeField]
        private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

        public bool ConformRotation
        {
            get { return _conformRotation; }
            set { _conformRotation = value; }
        }
        [Tooltip("Should spawns conform to exact spawn point rotation?")]
        [SerializeField]
        private bool _conformRotation = false;

        public bool ConformPosition
        {
            get { return _conformPosition; }
            set { _conformPosition = value; }
        }
        [Tooltip("Should spawns conform to exact spawn point position?")]
        [SerializeField]
        private bool _conformPosition = false;

        public BaseSpline Spline
        {
            get { return _spline; }
            set { _spline = value; }
        }
        [Tooltip("The reference to the spline object that spawns will be attached to when spawned.")]
        [SerializeField]
        private BaseSpline _spline;

        [Tooltip("Maximum number of spawns to generate.")]
        [SerializeField]
        private int _maxSpawns = 10;

        [Tooltip("This will delay spawns when the area is triggered. Useful if your area is persistent and needs a navmesh generated first.")]
        [SerializeField]
        private float _delaySpawns = 0f;

        public bool SuppressBiomeSpawns
        { 
            get { return _suppressBiomeSpawns; } 
            set 
            { 
                // If the value changed then unregister and re-register this area to take affect
                if (_suppressBiomeSpawns != value)
                {
                    _suppressBiomeSpawns = value;
                    SoulLinkSpawner.Instance.UnregisterSpawnArea(this);
                    SoulLinkSpawner.Instance.RegisterSpawnArea(this);
                } // if
            }
        }
        [Tooltip("Check this if you don't want biome spawns to be generated in this area.")]
        [SerializeField]
        private bool _suppressBiomeSpawns = false;

        public float SuppressionRadius
        {
            get { return _suppressionRadius; }
            set {_suppressionRadius = value; }
        }
        [Tooltip("Set the size of your biome suppression area.")]
        [SerializeField]
        private float _suppressionRadius = 15f;

        public bool DistributeHerdsFromCenter
        {
            get { return _distributeHerdsFromCenter; }
            set { _distributeHerdsFromCenter = value; }
        }
        [SerializeField]
        private bool _distributeHerdsFromCenter = false;

        [Tooltip("This will trigger a nav mesh to be built after all spawns are generated for this area. Must be using Generate NavMesh option.")]
        [SerializeField]
        private bool _buildNavMeshAfterSpawning = false;

        [Tooltip("Check this if you want your area spawns to only despawn if they are killed or forced to despawn in code.")]
        [SerializeField]
        private bool _markSpawnsAsPersistent = false;

        [Tooltip("Set the interval in seconds in which spawns will be generated. Set to 0 to use the Global Spawn Rate.")]
        [SerializeField]
        private float _spawnInterval = 0f;

        public bool ResetArea
        {
            get { return _resetArea; }
            set { _resetArea = value; }
        }
        [Tooltip("Check this option to reset the spawn area under certain conditions.")]
        [SerializeField]
        private bool _resetArea = true;

        public ResetKind ResetType
        {
            get { return _resetType; }
            set { _resetType = value; }
        }
        [Tooltip("Select the method by which the spawn area will reset.")]
        [SerializeField]
        private ResetKind _resetType = ResetKind.OutOfRange;

        public float ResetDuration
        {
            get { return _resetDuration; }
            set { _resetDuration = value; }
        }
        [Tooltip("Set the duration for how long to wait before resetting the spawn area. Units are in game time hours or real time seconds.")]
        [SerializeField]
        private float _resetDuration = 1f;

        [Tooltip("Toggle on if you want the area to automatically trigger when it has been reset.")]
        [SerializeField]
        private bool _autoTriggerOnReset = false;

        [SerializeField]
        private int _minChangeMaxSpawns = 0;

        [SerializeField]
        private int _maxChangeMaxSpawns = 0;

        public ResetRepeatKind ResetRepeatType
        {
            get { return _resetRepeatType; }
            set { _resetRepeatType = value; }
        }
        [Tooltip("Set the reset repeat method. Default is unlimited.")]
        [SerializeField]
        private ResetRepeatKind _resetRepeatType = ResetRepeatKind.Unlimited;

        public int MaxResets
        {
            get { return _maxResets; }
            set {  _maxResets = value; }
        }
        [Tooltip("Set the maximum number of times this area can be reset. A reset is considered a wave.")]
        [SerializeField]
        private int _maxResets = 3;

        [SerializeField]
        private bool _spawnBoss = false;

        [SerializeField]
        private int _spawnBossOnWaveNumber = 3;

        [SerializeField]
        private Transform _bossPrefab;

        [SerializeField]
        private Transform _bossSpawnPoint;

        [SerializeField]
        private UnityEvent _onTriggeredEventHandler;

        [SerializeField]
        private UnityEvent _onStartSpawningEventHandler;

        [SerializeField]
        private UnityEvent _onSpawningCompleteEventHandler;

        [SerializeField]
        private UnityEvent _onWaveCompleteEventHandler;

        [SerializeField]
        private UnityEvent _onSpawnBossEventHandler;

        private FlexCollider _collider;

        public bool IsTriggered
        {  
            get { return _isTriggered; } 
            set { _isTriggered = value; }
        }

        private bool _isTriggered = false;

        public float SecondsToReset
        { get { return _secondsToReset; } }
        private float _secondsToReset = 0f; // the number of seconds remaining until the next reset occurs

        public int TotalResets
        { get { return _totalResets; } }
        private int _totalResets = 0; // the number of times this area has been reset

        public bool IsValid
        { get { return _isValid; } }

        private string _objectGuid;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                EnableDisableArea(value);
            }
        }

        private bool _enabled = true;
        private bool _isValid = false;
        private AreaData _areaData;
        private int _totalSpawns = 0;
        private bool _startDelayComplete = false;
        private int _spawnPointIndex = 0; // the current spawn point to use or point along spline

        private int _spawnsInWave = 0;
        private bool _spawningComplete = false;
        private int _spawnsDead = 0;

        private float _splineLength = 0f;
        private float _splinePointSeparation = 0f;

        private TimeOfDaySpawnsDictionary _spawnsDictionary;

        private bool _is3D = true;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (_spawnRadius < _minimumSpawnRange)
            {
                Debug.LogError("SpawnArea [" + _areaName + "] Error: Your Spawn radius is smaller then your Minimum Spawn range. Nothing will spawn.");
            } // if

            // Make sure the named area actually exists in the SoulLinkSpawnerDatabase else log an error 
            _isValid = true;
            if (!_useLocalSpawnData)
            {
                _isValid = (_areaData != null);
                if (!_isValid)
                {
                    Debug.LogError("Area '" + _areaName + "' does not exist in the SoulLink Spawner Database.");
                } // if
            } // if

            if (_isValid)
            {
                StartCoroutine(DelayStart());
            } // if
        } // Start()

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            _is3D = (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D);

            // Prepare components as needed
            if (!_manualTrigger)
            {
                _collider = gameObject.AddComponent<FlexCollider>();

                if (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D)
                {
                    _collider.CreateCollider();
                }
                else
                {
                    _collider.CreateCollider(false);
                }

                _collider.Radius = _overrideTriggerRadius ? _triggerRadius : SoulLinkSpawner.Instance.SpawnRadius;
                _collider.IsTrigger = true;
                _collider.Enabled = false;
            } // if !_manualTrigger

            var poolManager = FindObjectOfType<PoolManager>();
            if (poolManager != null)
            {
                poolManager.AddPoolItem(_bossPrefab, SoulLinkSpawner.Instance.DefaultPopulationCap, SoulLinkSpawner.Instance.Database);

                if (_useLocalSpawnData)
                {
                    foreach (var tod in _timeOfDaySpawns)
                    {
                        foreach (var spawn in tod.Value.spawnInfoData)
                        {
                            if (spawn.spawnType == SpawnType.Prefab)
                            {
                                poolManager.AddPoolItem(spawn.prefab, spawn.populationCap, SoulLinkSpawner.Instance.Database);
                            }
                            else if (spawn.spawnType == SpawnType.Herd && spawn.herd != null)
                            {
                                foreach (var herdSpawn in spawn.herd.herdPrefabs)
                                {
                                    poolManager.AddPoolItem(herdSpawn.prefab, herdSpawn.populationCap, SoulLinkSpawner.Instance.Database);
                                } // foreach spawn
                            }
                            else if (spawn.spawnType == SpawnType.Variants)
                            {
                                // Add each variant into the pool at the same population cap
                                foreach (var variant in spawn.prefabVariants)
                                {
                                    poolManager.AddPoolItem(variant, spawn.populationCap, SoulLinkSpawner.Instance.Database, spawn.reusable,
                                        spawn.reusable ? false : true);
                                } // foreach
                            }
                        }
                    } // foreach
                } 
                else
                {
                    _areaData = SoulLinkSpawner.Instance.Database.areaSpawnData.Find(e => e.areaName == _areaName);
                }
            } // if

            // If a _spline is assigned then we cache the length of the spline for future use
            if (_spline != null)
            {
                _splineLength = _spline.GetLength();
                _splinePointSeparation = (_splineLength / _maxSpawns) / 100f;
                _splinePointSeparation -= ((_splinePointSeparation * _maxSpawns) - 1f) / _maxSpawns;
            } // if

            _spawnsDictionary = _useLocalSpawnData ? _timeOfDaySpawns : _areaData.timeOfDaySpawns;
        } // Awake()

        /// <summary>
        /// DelayStart
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(SoulLinkSpawner.Instance.DelaySpawnsOnStart);

            _startDelayComplete = true;
        } // DelayStart()

        /// <summary>
        /// FixedUpdate
        /// </summary>
        private void FixedUpdate()
        {
            if (_resetArea && _secondsToReset > 0f)
            {
                _secondsToReset -= Time.fixedDeltaTime;

                if (_secondsToReset < 0f)
                {
                    ResetTrigger();
                    return;
                } // if
            } // if

            if (!_startDelayComplete || _isTriggered) return;

            if (_manualTrigger) return;
        } // FixedUpdate()

        void OnTimeSyncHandler()
        {
            // Check if this area's time of day selections are in current time of day values. If not, disable the trigger collider
            _collider.Enabled = IsValidToSpawn();
        } // OnTimeSyncHandler()

        /// <summary>
        /// Check if the current time of day and weather is valid for this spawn area
        /// </summary>
        /// <returns></returns>
        public bool IsValidToSpawn()
        {
            foreach (var tod in _spawnsDictionary)
            {
                var inTOD = SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(tod.Key);
                if (inTOD)
                {
                    foreach (var spawninfodata in tod.Value.spawnInfoData)
                    {
                        if (SoulLinkSpawner.Instance.IsInCurrentWeatherConditions(spawninfodata.weatherRules, transform.position.y))
                        {
                            return true;
                        } // if
                    } // foreach spawnInfoData
                } // if
            } // foreach

            return false;
        } // IsValidToSpawn()

        /// <summary>
        /// Set the time remaining before the next reset occurs
        /// </summary>
        /// <param name="value"></param>
        public void SetSecondsToReset(float value)
        {
            _secondsToReset = value;
        } // SetResetSeconds()

        /// <summary>
        /// Set the number of times that a reset has already occurred.
        /// </summary>
        /// <param name="value"></param>
        public void SetTotalResets(int value)
        {
            if (_totalResets <= _maxResets || _resetRepeatType == ResetRepeatKind.Unlimited)
            {
                _totalResets = value;
            } // if

            SpawnBoss();
        } // SetTotalResets()

        /// <summary>
        /// Set a number of seconds that have elapsed in order to advance time.
        /// </summary>
        /// <param name="value"></param>
        public void SetSecondsElapsed(float value)
        {
            if (_secondsToReset > 0)
            {
                _secondsToReset -= value;
            } // if
        } // SetSecondsElapsed()

        /// <summary>
        /// OnTriggerEnter
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (_manualTrigger || _isTriggered || !_isValid) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                PerformTrigger();            
            } // if
        } // OnTriggerEnter()

        /// <summary>
        /// OnTriggerEnter2D
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_manualTrigger || _isTriggered || !_isValid) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                PerformTrigger();
            } // if
        } // OnTriggerEnter2D()

        /// <summary>
        /// Trigger spawning
        /// </summary>
        public void PerformTrigger()
        {
            SoulLinkGlobal.DebugLog(gameObject, "Spawn Area PerformTrigger");
            _isTriggered = true;

            if (_resetArea && (_resetType == ResetKind.GameTimeDuration || _resetType == ResetKind.RealtimeDuration))
            {
                // Calculate exact reset time and prepare for the reset
                PrepareReset(_resetDuration);
            } // if

            // Call user's custom triggered event handler
            if (_onTriggeredEventHandler != null)
            {
                _onTriggeredEventHandler.Invoke();
            } // if

            StartCoroutine(GenerateSpawns());
        } // PerformTrigger()

        /// <summary>
        ///  GenerateSpawns
        /// </summary>
        /// <returns></returns>
        IEnumerator GenerateSpawns()
        {
            SoulLinkGlobal.DebugLog(gameObject, "GenerateSpawns");
            yield return new WaitForSeconds(_delaySpawns);

            _spawnPointIndex = 0;

            // Call user's custom on started spawning event handler
            if (_onStartSpawningEventHandler != null)
            {
                _onStartSpawningEventHandler.Invoke();
            } // if

            int iterations = 0;
            while (!SoulLinkSpawner.Instance.Paused && _enabled && TotalSpawned() < _maxSpawns && !SoulLinkSpawner.Instance.DisableAreaSpawns && iterations < (_maxSpawns * 10))
            {
                // Pick a random entry from available time of days
                var tod = SoulLinkSpawner.Instance.RandomCurrentTimeOfDay();

                if (_spawnsDictionary.ContainsKey(tod))
                {
                    if (_spawnsDictionary[tod].spawnInfoData.Count == 0)
                    {
                        continue;
                    } // if

                    // Pick a random entry from available spawns
                    var spawnInfoData = _spawnsDictionary[tod].spawnInfoData[UnityEngine.Random.Range(0, _spawnsDictionary[tod].spawnInfoData.Count)];
                    spawnInfoData.spawnAreaGuid = GetGuid();
                    spawnInfoData.markPersistent = _markSpawnsAsPersistent ? true : spawnInfoData.markPersistent;

                    uint chance;
                    if (spawnInfoData.probability > 0 && SoulLinkSpawner.RandomValues.FindValue((uint)spawnInfoData.probability, out chance) && 
                        (SoulLinkSpawner.Instance.GetTotalSpawns("") < SoulLinkSpawner.Instance.MaxSpawns || spawnInfoData.ignoreTotalSpawns))
                    {
                        Vector3 spawnPos = RandomizeSpawnPosition(out spawnInfoData.spawnRot);

                        // Check if there are any spawns of this type within a certain range. If so then skip it
                        float proximityRange = ((_spawnRadius * _clusterRange) / spawnInfoData.populationCap);
                        if (_clusterRange > 0f && SoulLinkSpawner.Instance.PoolManager.AnySpawnInRange(spawnInfoData.prefab, spawnPos, proximityRange))
                        {
                            yield return null;
                            continue;
                        } // if

                        if (spawnInfoData.spawnType == SpawnType.Herd)
                        {
                            // Spawn a randomized qty
                            int qtyDesired = UnityEngine.Random.Range(spawnInfoData.herd.minPopulation, spawnInfoData.herd.maxPopulation);
                            int qty = 0;

                            for (int i = 0; i < qtyDesired; ++i)
                            {
                                foreach (var spawn in spawnInfoData.herd.herdPrefabs)
                                {
                                    // Randomize position in radius around spawnPos
                                    Vector3 newPos = Utility.GetRandomPos(0f, spawnInfoData.herd.spawnRadius, _distributeHerdsFromCenter ? transform.position : spawnPos,
                                        SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);
                                    Vector3 herdSpawnPos = new Vector3(newPos.x, SoulLinkSpawner.Instance.GetYPos(newPos), newPos.z);

                                    float minElevation = 0f;
                                    float maxElevation = 1000f;

                                    if (spawnInfoData.overrideElevation)
                                    {
                                        minElevation = spawnInfoData.minElevation;
                                        maxElevation = spawnInfoData.maxElevation;
                                    }
                                    else
                                    {
                                        minElevation = spawn.overrideElevation ? spawn.minElevation : SoulLinkSpawner.Instance.MinElevation;
                                        maxElevation = spawn.overrideElevation ? spawn.maxElevation : SoulLinkSpawner.Instance.MaxElevation;
                                    }

                                    float minSlopeAngle = spawn.overrideSlopeAngle ? spawn.minSlopeAngle : SoulLinkSpawner.Instance.MinSlopeAngle;
                                    float maxSlopeAngle = spawn.overrideSlopeAngle ? spawn.maxSlopeAngle : SoulLinkSpawner.Instance.MaxSlopeAngle;

                                    // Assign the herd's filters to the individual spawns to validate them properly
                                    spawn.textureRules = spawnInfoData.textureRules;
                                    spawn.weatherRules = spawnInfoData.weatherRules;

                                    spawn.spawnAreaGuid = GetGuid();
                                    spawn.randomChance = chance;
                                    spawn.herdID = 0; // we don't track these as herd members because they are not using RandomValuePool                                    spawn.markPersistent = spawnInfoData.markPersistent;
                                    spawn.ignoreRaycast = spawnInfoData.ignoreRaycast;

                                    // Note: SpawnArea ignore player POV can override individual spawns only if set to true.
                                    if (SoulLinkSpawner.Instance.ValidateSpawnRules(spawn, herdSpawnPos, tod, spawnInfoData.ignorePlayerFOV ? true : spawn.ignorePlayerFOV,
                                        minElevation, maxElevation, minSlopeAngle, maxSlopeAngle, true))
                                    {
                                        qty++;
                                    } // if

                                    if (qty == qtyDesired) break;
                                } // foreach

                                if (qty == qtyDesired) break;
                            } // for i
                        }
                        else
                        {
                            if (spawnInfoData.spawnType == SpawnType.Variants && spawnInfoData.prefabVariants.Count > 0)
                            {
                                spawnInfoData.prefab = spawnInfoData.prefabVariants[UnityEngine.Random.Range(0, spawnInfoData.prefabVariants.Count)];
                            } // if

                            float minElevation = spawnInfoData.overrideElevation ? spawnInfoData.minElevation : SoulLinkSpawner.Instance.MinElevation;
                            float maxElevation = spawnInfoData.overrideElevation ? spawnInfoData.maxElevation : SoulLinkSpawner.Instance.MaxElevation;
                            float minSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.minSlopeAngle : SoulLinkSpawner.Instance.MinSlopeAngle;
                            float maxSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.maxSlopeAngle : SoulLinkSpawner.Instance.MaxSlopeAngle;

                            spawnInfoData.randomChance = chance;

                            spawnInfoData.ignoreRaycast = (_spawningMethod == SpawningMethod.SpawnPoints && _conformPosition);
                            SoulLinkSpawner.Instance.ValidateSpawnRules(spawnInfoData, spawnPos, tod, spawnInfoData.ignorePlayerFOV, minElevation, maxElevation, minSlopeAngle, maxSlopeAngle, true);
                        }
                    } // if probability
                    else
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "Spawn failed On Probability Check");
                    }

                    ++iterations;
                } // if tod in set

                yield return new WaitForSeconds(_spawnInterval == 0f? (1f / SoulLinkSpawner.Instance.SpawnRate) : _spawnInterval);
            } // while

            SoulLinkGlobal.DebugLog(gameObject, "Spawning complete, Total Spawns = " + (_totalSpawns + _spawnsDead) + ", Max Spawns = " + _maxSpawns + ", iterations = " + iterations);

            // Call user's custom on spawning complete event handler
            if (_onSpawningCompleteEventHandler != null)
            {
                _onSpawningCompleteEventHandler.Invoke();
            } // if

            _spawningComplete = true;

#if SOULLINK_USE_AINAV
            // Build nav mesh after spawning only if Generate Nav Mesh is enabled and only if we have not performed any area resets
            if (_buildNavMeshAfterSpawning && SoulLinkSpawner.Instance.GenerateNavMesh && _totalResets == 0)
            {
                SoulLinkSpawner.Instance.PlaceSpawnNavChecker(transform.position);
            } // if
#endif
        } // GenerateSpawns()

        /// <summary>
        /// Randomize a spawn position based on the spawning method
        /// </summary>
        /// <returns></returns>
        public Vector3 RandomizeSpawnPosition(out Quaternion spawnRot)
        {
            spawnRot = Quaternion.identity;

            if (SpawnMethod == SpawningMethod.SpawnRadius)
            {
                var spawnPos = Utility.GetRandomPos(_minimumSpawnRange, _spawnRadius, transform.position, 
                    SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);

                // Sample terrain to get proper y position for spawn
                return new Vector3(spawnPos.x, !_is3D ? spawnPos.y : SoulLinkSpawner.Instance.GetYPos(new Vector3(spawnPos.x, transform.position.y, spawnPos.z)), spawnPos.z);
            }
            else if (SpawnMethod == SpawningMethod.SpawnBounds && _spawnAreaBounds.Count > 0) 
            {
                // First randomize which bounds to use for this spawn
                var index = UnityEngine.Random.Range(0, _spawnAreaBounds.Count);

                // Now randomize a position within the bounds
                var pos = Utility.GetRandomPosInBounds(_spawnAreaBounds[index].bounds, SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);

                if (_is3D)
                {
                    return new Vector3(pos.x, SoulLinkSpawner.Instance.GetYPos(new Vector3(pos.x, _spawnAreaBounds[index].min.y, pos.y)), pos.y);
                }
                else
                {
                    switch (SoulLinkSpawner.Instance.SpawnAxes)
                    {
                        case SpawnAxes.XY:
                            return new Vector3(pos.x, pos.y, 0f);
                        case SpawnAxes.XZ:
                            return new Vector3(pos.x, 0f, pos.y);
                        case SpawnAxes.YZ:
                            return new Vector3(0f, pos.x, pos.y);
                    } // switch
                }
            }
            else if (SpawnMethod == SpawningMethod.SpawnPoints && _spawnPoints.Count > 0)
            {
                Vector3 spawnPos;
                if (_spawnPoints[_spawnPointIndex].useRadius)
                {
                    spawnPos = Utility.GetRandomPos(0, _spawnPoints[_spawnPointIndex].spawnRadius, _spawnPoints[_spawnPointIndex].transform.position,
                        SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);
                    if (_conformRotation)
                    {
                        spawnRot = _spawnPoints[_spawnPointIndex].transform.rotation;
                    } // if
                }
                else
                {
                    spawnPos = _spawnPoints[_spawnPointIndex].transform.position;
                    if (_conformRotation)
                    {
                        spawnRot = _spawnPoints[_spawnPointIndex].transform.rotation;
                    } // if
                }

                ++_spawnPointIndex;
                if (_spawnPointIndex >= _spawnPoints.Count)
                {
                    _spawnPointIndex = 0;
                } // if

                return spawnPos;
            }
            else if (SpawnMethod == SpawningMethod.Spline && _spline != null)
            {
                if (_splinePositioning == SplinePositioning.Start)
                {
                    return _spline.GetWorldPosition(0); // return position at start of spline
                }
                else if (_splinePositioning == SplinePositioning.End)
                {
                    return _spline.GetWorldPosition(1); // return position at end of spline
                }
                else
                {
                    // Use _spawnPointIndex to distribute along spline
                    var spawnPos = _spline.GetWorldPosition(_spawnPointIndex * _splinePointSeparation);
                    ++_spawnPointIndex;
                    if (_spawnPointIndex >= _maxSpawns)
                    {
                        _spawnPointIndex = 0;
                    } // if

                    return spawnPos;
                }
            }

            return Vector3.zero;
        } // RandomizeSpawnPosition()

        /// <summary>
        /// SpawnMaxReached
        /// </summary>
        /// <returns></returns>
        public bool SpawnMaxReached()
        {
            return ((_totalSpawns + _spawnsDead) == _maxSpawns);
        } // SpawnMaxReached()

        /// <summary>
        /// IncSpawnTotal
        /// </summary>
        public void IncSpawnTotal()
        {
            ++_totalSpawns;
        } // IncSpawnTotal()

        /// <summary>
        /// DecSpawnTotal
        /// </summary>
        public void DecSpawnTotal()
        {
            --_totalSpawns;
        } // DecSpawnTotal()

        /// <summary>
        /// Reset total spawns to zero so the area can be triggered again if the trigger is enabled
        /// </summary>
        public void ResetSpawns()
        {
            _totalSpawns = 0;
        } // ResetSpawns()

        /// <summary>
        /// Reset the area's trigger so that it can re-trigger after conditions are met.
        /// </summary>
        public void ResetTrigger()
        {
            _spawningComplete = false;
            _spawnsDead = 0;
            _totalSpawns = 0;
            ++_totalResets;
            if (_resetRepeatType == ResetRepeatKind.UserDefined && _totalResets > _maxResets) return;

            if (_minChangeMaxSpawns != 0 && _maxChangeMaxSpawns != 0)
            {
                _maxSpawns += UnityEngine.Random.Range(_minChangeMaxSpawns, _maxChangeMaxSpawns);
                if (_maxSpawns < 1) _maxSpawns = 1; // must have at least 1
            } // if

            // If any spawns still remain from the previous trigger, despawn them now
            for (int i = 0; i < transform.childCount; ++i)
            {
                var spawnable = transform.GetChild(i).GetComponent<ISpawnable>();
                if (spawnable != null && spawnable.GetIsPersistent() == false)
                {
                    spawnable.ForceDespawn();
                } // if
            } // for i

            StartCoroutine(DoResetTrigger());
        } // ResetTrigger()

        /// <summary>
        /// DoResetTrigger
        /// </summary>
        /// <returns></returns>
        IEnumerator DoResetTrigger()
        {
            _isTriggered = false;

            if (!_manualTrigger)
            {
                _collider.Enabled = false;

                yield return null;

                _collider.Enabled = true;
            } // if

            if (_autoTriggerOnReset)
            {
                PerformTrigger();
            } // if
        } // DoResetTrigger()

        /// <summary>
        /// Calculate reset time in seconds and fire an Invoke command to perform the reset
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        void PrepareReset(float duration)
        {
            if (_resetType == ResetKind.GameTimeDuration)
            {
                // Convert duration to game time duration based on DayLengthInMinutes
                var ratio = SoulLinkSpawner.Instance.GameTime.DayLengthInMinutes / 1440f;
                _secondsToReset = (duration * 60f * 60f) * ratio;
            }
            else
            {
                _secondsToReset = duration;
            }
        } // PrepareReset()

        /// <summary>
        /// Call this method at runtime to change the area to a different named area.
        /// </summary>
        /// <param name="newAreaName"></param>
        public void ChangeArea(string newAreaName)
        {
            StopAllCoroutines();

            ResetTrigger();

            _areaName = newAreaName;
            _areaData = SoulLinkSpawner.Instance.Database.areaSpawnData.Find(e => e.areaName == _areaName);
            _isValid = (_areaData != null);
            if (!_isValid)
            {
                Debug.LogError("Area '" + _areaName + "' does not exist in the SoulLink Spawner Database.");
            } // if

            if (_isValid)
            {
                StartCoroutine(DelayStart());
            } // if
        } // ChangeArea()

        /// <summary>
        /// Calculate the total number of spawns that have spawned. Counts current total plus any that have already died.
        /// </summary>
        /// <returns></returns>
        public int TotalSpawned()
        {
            return _totalSpawns + _spawnsDead;
        } // TotalSpawned()

        /// <summary>
        /// Call this to indicate that a spawn has been generated and successfully added to this area
        /// </summary>
        public void SpawnAdded()
        {
            _spawnsInWave++;
        } // SpawnAdded()

        /// <summary>
        /// Informs the SpawnArea that one of its spawns has died
        /// </summary>
        public void SpawnDied()
        {
            if (_spawnsInWave == 0) return;

            if (!_spawningComplete)
            {
                _spawnsDead++; // temporary counter until spawning is fully complete
                return;
            } // if

            if (_spawnsDead > 0)
            {
                _spawnsInWave -= _spawnsDead;
                _spawnsDead = 0;
            } // if

            --_spawnsInWave;
            if (_spawnsInWave < 0) _spawnsInWave = 0;

            if (!_resetArea && _spawnsInWave == 0)
            {
                // call wave elimination event handler here if defined
                if (_onWaveCompleteEventHandler != null)
                {
                    _onWaveCompleteEventHandler.Invoke();
                } // if
            } // if

            // if reset is on and type is Elimination then start coroutine to check for wave elimination
            if (_resetArea && _spawnsInWave == 0 && _resetType == ResetKind.Elimination)
            {
                // call wave elimination event handler here if defined
                if (_onWaveCompleteEventHandler != null)
                {
                    _onWaveCompleteEventHandler.Invoke();
                } // if

                // Reset the area if max repeats not exceeded
                if (_totalResets < _maxResets || _resetRepeatType == ResetRepeatKind.Unlimited)
                {
                    SpawnBoss();
                    PrepareReset(_resetDuration);
                }
                else
                {
                    SpawnBoss();
                }
            } // if
        } // SpawnDied()

        /// <summary>
        /// Spawn the boss
        /// </summary>
        public void SpawnBoss()
        {
            if (!_spawnBoss || _bossPrefab == null || _spawnBossOnWaveNumber != (_totalResets + 1)) return;

            // Barebones spawn data
            SpawnData spawnData = new SpawnData()
            {
                spawnPrefab = _bossPrefab,
                spawnCategory = DEFAULT_SPAWN_CATEGORY_NAME, // TODO: make this settable
                spawnPos = _bossSpawnPoint != null ? _bossSpawnPoint.transform.position : transform.position,
                spawnRot = _bossSpawnPoint != null ? _bossSpawnPoint.rotation : transform.rotation
            };
            SoulLinkSpawner.Instance.Spawn(spawnData, false, false);

            // call spawn boss event handler here if defined
            if (_onSpawnBossEventHandler != null)
            {
                _onSpawnBossEventHandler.Invoke();
            } // if
        } // SpawnBoss()

        /// <summary>
        /// Generate a unique id for this object
        /// </summary>
        public void GenerateGuid()
        {
            if (string.IsNullOrEmpty(_objectGuid))
            {
                _objectGuid = Guid.NewGuid().ToString();
            } // if
        } // GenerateGuid()

        /// <summary>
        /// Get the object's Guid
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return _objectGuid;
        } //GetGuid()

        /// <summary>
        /// Set the object's Guid directly
        /// </summary>
        /// <param name="value"></param>
        public void SetGuid(string value)
        {
            _objectGuid = value;
        } // SetGuid()

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            SoulLinkSpawner.Instance.RegisterSpawnArea(this);
            SoulLinkSpawner.Instance.OnTimeSync += OnTimeSyncHandler;
        } // OnEnable()

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            SoulLinkSpawner.Instance.OnTimeSync -= OnTimeSyncHandler;
            SoulLinkSpawner.Instance.UnregisterSpawnArea(this);
        } // OnDisable()

        /// <summary>
        /// OnTriggerExit
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (_manualTrigger) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag) && (_resetArea && _resetType == ResetKind.OutOfRange))
            {
                ResetTrigger();
            } // if
        } // OnTriggerExit()
		
        /// <summary>
        /// Enable or Disable the spawn area. Despawn any live spawns and remove any pending placeholders
        /// </summary>
        /// <param name="value"></param>
        void EnableDisableArea(bool value)
        {
            _enabled = value;
            _collider.Enabled = value;

            if (!_enabled)
            {
                int childCount = transform.childCount - 1;
                for (int i = childCount; i > -1; i--)
                {
                    ISpawnable spawnable = transform.GetChild(i).gameObject.GetComponent<ISpawnable>();
                    if (spawnable != null)
                    {
                        SoulLinkSpawner.Instance.Despawn(transform.GetChild(i));
                        continue;
                    } // if

                    SpawnPlaceholder placeholder = transform.GetChild(i).gameObject.GetComponent<SpawnPlaceholder>();
                    if (placeholder != null)
                    {
                        placeholder.enabled = false;
                        SoulLinkSpawner.Instance.ReleasePlaceholder(placeholder);
                    } // if
                } // for i

                _secondsToReset = 0f; // stop the current reset if there is one

                StopAllCoroutines();
            } // if
        } // EnableDisableArea()

#if UNITY_EDITOR
        /// <summary>
        /// OnDrawGizmosSelected
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (SpawnMethod == SpawningMethod.SpawnRadius && SpawnRadius == 0f)
            {
                Gizmos.color = new Color(1, 1, 1, 0.75F);
                Gizmos.DrawIcon(transform.position, "SpawnSilhouette.png");
            }
            else if (SpawnMethod == SpawningMethod.SpawnPoints)
            {
                foreach (var point in SpawnPoints)
                {
                    if (point.transform != null && !point.useRadius)
                    {
                        Gizmos.color = new Color(1, 1, 1, 0.75F);
                        Gizmos.DrawIcon(point.transform.position, "SpawnSilhouette.png", true);
                    }
                } // foreach
            }
        } // OnDrawGizmosSelected()
#endif
    } // class SpawnArea
} // namespace Magique.SoulLink
