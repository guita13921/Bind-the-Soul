using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using static Magique.SoulLink.SpawnArea;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveSpawner : MonoBehaviour, ISoulLinkGuid
    {
        public const string OnTriggeredMessageName = "OnTriggeredEventHandler";
        protected const string OnStartSpawningMessageName = "OnStartSpawningEventHandler";
        protected const string OnSpawningCompleteMessageName = "OnSpawningCompleteEventHandler";
        protected const string OnRepeatCompleteMessageName = "OnRepeatCompleteEventHandler";
        protected const string OnItemCompleteMessageName = "OnItemCompleteEventHandler";

        const int NUM_RETRIES = 6;

        public string WaveName
        {
            get { return _waveName; }
            set { _waveName = value; }
        }

        [SerializeField]
        private string _waveName;

        private string _objectGuid;

        public string TimeOfDay
        {
            get { return _timeOfDay; }
            set { _timeOfDay = value; }
        }
        [SerializeField]
        private string _timeOfDay;

        public bool ResetWaveOnNextDay
        {
            get { return _resetWaveOnNextDay; }
            set { _resetWaveOnNextDay = value; }
        }
        [SerializeField]
        private bool _resetWaveOnNextDay = false;

        [SerializeField]
        private bool _startPaused = false;

        [SerializeField]
        private float _delaySpawns = 0f;

        [SerializeField]
        private bool _distributeHerdsFromCenter = true;

        public bool RepeatWave
        {
            get { return _repeatWave; }
        }
        [SerializeField]
        private bool _repeatWave = false;

        [SerializeField]
        private ResetRepeatKind _repeatType = ResetRepeatKind.UserDefined;

        public int NumberOfRepeats
        {
            get { return _numberOfRepeats; }
        }
        [SerializeField]
        private int _numberOfRepeats = 0;

        [SerializeField]
        private float _repeatDelay = 0f;

        [Tooltip("Check this if you want your wave spawns to only despawn if they are killed or forced to despawn in code.")]
        [SerializeField]
        private bool _markSpawnsAsPersistent = false;

        public List<WaveItemData> WaveItems
        {
            get { return _waveItems; }
        }

        [SerializeField]
        public List<WaveItemData> _waveItems = new List<WaveItemData>();
        
        [SerializeField]
        private UnityEvent _onWaveStartedEventHandler;

        public UnityEvent OnWaveCompletedEventHandler
        {
            get { return _onWaveCompletedEventHandler; }
            set { _onWaveCompletedEventHandler = value; }
        }
        [SerializeField]
        private UnityEvent _onWaveCompletedEventHandler;

        [SerializeField]
        private UnityEvent _onWaveItemRepeatStartedEventHandler;

        [SerializeField]
        private UnityEvent _onWaveItemRepeatCompletedEventHandler;

        public bool WaveCompleted
        {
            get { return _waveCompleted; }
        }

        public bool IsPaused
        { get { return _isPaused; } }

        private bool _isPaused = false;

        private float _elapsedTime = 0f;

        private bool _is3D = true;
        private Dictionary<WaveItemData, WaveTrigger> _waveTriggers = new Dictionary<WaveItemData, WaveTrigger>();
        private bool _waveCompleted = false; // Only completed when all wave items have completed

        public bool WaveStarted
        { get { return _waveStarted; } }
        private bool _waveStarted = false;
        private bool _hasUnlimitedWaveItems = false;

        public int CurrentRepeat
        {
            get { return _currentRepeat; }
        }
        private int _currentRepeat = 0;

        private readonly object waveCompleteLock = new object();

        public WaveManager WaveManager
        { get { return _waveManager != null ? _waveManager : GetComponentInParent<WaveManager>(); } }
        private WaveManager _waveManager;

        private void Awake()
        {
            _is3D = SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D;

            _waveManager = GetComponentInParent<WaveManager>();

            var poolManager = FindObjectOfType<PoolManager>();
            if (poolManager != null)
            {
                foreach (var waveItem in _waveItems)
                {
                    PrepareWaveItem(waveItem);
                    
                    if (waveItem.spawnInfoData.spawnType == SpawnType.Prefab)
                    {
                        poolManager.AddPoolItem(waveItem.spawnInfoData.prefab, waveItem.spawnInfoData.populationCap, SoulLinkSpawner.Instance.Database);
                    }
                    else if (waveItem.spawnInfoData.spawnType == SpawnType.Herd && waveItem.spawnInfoData.herd != null)
                    {
                        foreach (var spawn in waveItem.spawnInfoData.herd.herdPrefabs)
                        {
                            poolManager.AddPoolItem(spawn.prefab, spawn.populationCap, SoulLinkSpawner.Instance.Database);
                        } // foreach spawn
                    }
                    else if (waveItem.spawnInfoData.spawnType == SpawnType.Variants)
                    {
                        // Add each variant into the pool at the same population cap
                        foreach (var spawn in waveItem.spawnInfoData.prefabVariants)
                        {
                            poolManager.AddPoolItem(spawn, waveItem.spawnInfoData.populationCap, SoulLinkSpawner.Instance.Database, waveItem.spawnInfoData.reusable,
                                waveItem.spawnInfoData.reusable ? false : true);
                        } // foreach
                    }
                } // foreach
            } // if

            _hasUnlimitedWaveItems = (_waveItems.Find(e => e.repeatItem && e.repeatType == ResetRepeatKind.Unlimited) != null);

            CreateWaveItemTriggers();
        } // Awake()

        private void Update()
        {
            if (_isPaused || (_waveCompleted && !_hasUnlimitedWaveItems)) return;

            // Check time of day before allowing wave to fully start
            if (!_waveStarted && !SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(_timeOfDay)) return;

            if (_waveStarted && !SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(_timeOfDay))
            {
                // Complete this wave and stop the wave spawner
                ForceWaveCompletion();
            } // if

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < _delaySpawns) return;

            // Iterate through each wave item and start their wave spawners when appropriate
            foreach (var waveItem in _waveItems)
            {
                // Assign a guid to the wave item if one has not already been generated
                if (string.IsNullOrEmpty(waveItem.guid))
                {
                    waveItem.guid = Guid.NewGuid().ToString();
                } // if

                if (!waveItem.disable)
                {
                    if (!waveItem.started && waveItem.canStart)
                    {
                        if (!_waveStarted && !waveItem.completed)
                        {
                            if (_onWaveStartedEventHandler != null)
                            {
                                _onWaveStartedEventHandler.Invoke();
                            } // if

                            _waveStarted = true;
                        } // if

                        if (waveItem.startType == WaveItemData.StartKind.Auto)
                        {
                            if (waveItem.delayStartWave == 0f)
                            {
                                ActivateWaveTrigger(waveItem);
                            }
                            else
                            {
                                if (_elapsedTime >= waveItem.delayStartWave)
                                {
                                    ActivateWaveTrigger(waveItem);
                                }
                            }
                        }
                        else
                        {
                            ActivateWaveTrigger(waveItem);
                        }
                    } // if
                } // if
            } // foreach
        } // Update()

        /// <summary>
        /// Prepare a wave item before allowing it to start
        /// </summary>
        /// <param name="waveItem"></param>
        void PrepareWaveItem(WaveItemData waveItem)
        {
            // Save original min/max value here so it can be restored on wave repeats as necessary
            waveItem.originalMinMaxSpawns = waveItem.minMaxSpawns;

            waveItem.canStart = (waveItem.startType == WaveItemData.StartKind.Auto);

            // If a spline is assigned then we cache the length of the spline for future use
            if (waveItem.spline != null)
            {
                waveItem.splineLength = waveItem.spline.GetLength();
            } // if
        } // PrepareWaveItem()

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
            SoulLinkSpawner.Instance.RegisterWaveSpawner(this);
            _isPaused = _startPaused;

            SoulLinkSpawner.Instance.OnTimeSync += CheckForTimeOfDayOutOfScope;
        } // OnEnable()

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            SoulLinkSpawner.Instance.OnTimeSync -= CheckForTimeOfDayOutOfScope;
            SoulLinkSpawner.Instance.UnregisterWaveSpawner(this);
        } // OnDisable()

        /// <summary>
        /// Reset all wave items so they can be executed again
        /// </summary>
        public void ResetWaveItems()
        {
            foreach (var waveItem in _waveItems)
            {
                waveItem.started = false;
                waveItem.completed = false;

                PrepareWaveItem(waveItem);
                ResetWaveTrigger(waveItem);
                ResetWaveItemTracker(waveItem);
            } // foreach
        } // ResetWaveItems()

        /// <summary>
        /// Set a number of seconds that have elapsed in order to advance time.
        /// </summary>
        /// <param name="value"></param>
        public void SetSecondsElapsed(float value)
        {
            // TODO: implement this as appropriate for WaveSpawner in order to synchronize saved data
            // currently this is not used, but may need to be implemented depending on user needs
        } // SetSecondsElapsed()

        /// <summary>
        /// Creates all wave item triggers that can be activated when a wave is started
        /// </summary>
        void CreateWaveItemTriggers()
        {
            // For each wave item make a WaveTrigger in the scene and put the item in a dictionary so it can be activated when needed.
            foreach (var waveItem in _waveItems)
            {
                // 1. Create game object in the scene at the local point 0,0,0 underneath the wave spawner or the trigger origin if not null
                GameObject go = new GameObject();
                go.transform.SetParent(waveItem.triggerOrigin != null ? waveItem.triggerOrigin : transform);
                go.transform.localPosition = Vector3.zero;
                go.layer = LayerMask.NameToLayer("Ignore Raycast");

                // 2. Add WaveTrigger component to the object
                var prefix = waveItem.itemName != "" ? waveItem.itemName + "_" : ""; 
                go.name = prefix + "WaveTrigger";
                var waveTrigger = go.AddComponent<WaveTrigger>();

                // 3. Assign the spawner and wave item
                waveTrigger.SetWaveSpawner(this, waveItem);

                // 4. Call CreateTrigger on the WaveTrigger component
                waveTrigger.CreateTrigger();

                // 5. Add trigger to a dictionary matched to the WaveItem
                _waveTriggers[waveItem] = waveTrigger;

                // 6. Deactivate by default
                go.SetActive(false);
            } // foreach
        } // CreateWaveItemTriggers()

        /// <summary>
        /// Send an event message to all receivers on this game object
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="waveItem"></param>
        public void SendEventMessage(string messageName, WaveItemData waveItem)
        {
            BroadcastMessage(messageName, waveItem, SendMessageOptions.DontRequireReceiver);
        } // SendEventMessage()

        /// <summary>
        /// Start the spawning process for a specific wave item
        /// </summary>
        /// <param name="waveItem"></param>
        public void StartSpawning(WaveItemData waveItem)
        {
            SendEventMessage(OnStartSpawningMessageName, waveItem);

            StartCoroutine(GenerateWaveItemSpawns(waveItem));
        } // StartSpawning()

        /// <summary>
        /// Fire a wave trigger by providing the wave item name
        /// </summary>
        /// <param name="waveItemName"></param>
        public void PerformWaveTrigger(string waveItemName)
        {
            var waveItem = _waveItems.Find(e => e.itemName == waveItemName);
            if (waveItem != null)
            {
                _waveTriggers[waveItem].PerformTrigger();
            } // if
        } // PerformWaveTrigger(()

        /// <summary>
        /// Sets the wave item's canStart property so the wave can now start when properly triggered.
        /// </summary>
        /// <param name="waveItemName"></param>
        public void AllowWaveToStart(string waveItemName)
        {
            var waveItem = _waveItems.Find(e => e.itemName == waveItemName);
            if (waveItem != null)
            {
                waveItem.canStart = true;
            } // if
        } // AllowWaveToStart()

        /// <summary>
        /// Activate a wave item's trigger so that the wave can be started when the player has entered the trigger radius
        /// </summary>
        /// <param name="waveItem"></param>
        public void ActivateWaveTrigger(WaveItemData waveItem)
        {
            if (_waveTriggers[waveItem].isActiveAndEnabled) return;
            _waveTriggers[waveItem].gameObject.SetActive(true);
            _waveTriggers[waveItem].EnableCollider();
        } // ActivateWaveTrigger()

        /// <summary>
        /// Deactivate a wave item's trigger so that the wave can be re-started later on repeat
        /// </summary>
        /// <param name="waveItem"></param>
        public void DeactivateWaveTrigger(WaveItemData waveItem)
        {
            _waveTriggers[waveItem].gameObject.SetActive(false);
            _waveTriggers[waveItem].DisableCollider();
        } // DeactivateWaveTrigger()

        /// <summary>
        /// Reset a wave item's trigger so it can be re-fired on a wave repeat
        /// </summary>
        /// <param name="waveItem"></param>
        public void ResetWaveTrigger(WaveItemData waveItem)
        {
            _waveTriggers[waveItem].ResetTrigger();
        } // ResetWaveTrigger()

        /// <summary>
        /// Pause all wave items so they will not do any more spawning or count towards their delay values
        /// </summary>
        public void PauseWave()
        {
            _isPaused = true;

            foreach (var waveItem in _waveItems)
            {
                waveItem.paused = true;
            } // foreach
        } // PauseWave()

        /// <summary>
        /// UnPause all wave items so they will start spawning again and count towards their delay values
        /// </summary>
        public void UnPauseWave()
        {
            foreach (var waveItem in _waveItems)
            {
                waveItem.paused = false;
            } // foreach

            _isPaused = false;
        } // UnPauseWave()

        /// <summary>
        /// Add a new wave item
        /// </summary>
        public void AddWaveItem()
        {
            var newWaveItem = new WaveItemData();
            newWaveItem.guid = Guid.NewGuid().ToString();

            _waveItems.Add(newWaveItem);
        } // AddWaveItem()

        /// <summary>
        /// Remove a wave item
        /// </summary>
        /// <param name="waveItem"></param>
        public void RemoveWaveItem(WaveItemData waveItem)
        {
            _waveItems.Remove(waveItem);
        } // RemoveWaveItem()

        IEnumerator GenerateWaveItemSpawns(WaveItemData waveItem)
        {
            waveItem.started = true;

            int repeatCount = 0;
            Stack<int> herdSpawnQty = new Stack<int>();

            for (int j = 0; j < waveItem.numberOfRepeats + 1; ++j)
            {
                if (j > 0)
                {
                    if (_onWaveItemRepeatStartedEventHandler != null)
                    {
                        _onWaveItemRepeatStartedEventHandler.Invoke();
                    } // if
                } // if

                // Randomize how many spawns to generate; Qty is # of herds if set to spawnType Herd
                var spawnQuota = UnityEngine.Random.Range((int)waveItem.minMaxSpawns.x, (int)waveItem.minMaxSpawns.y + 1);
                if (waveItem.spawnInfoData.spawnType == SpawnType.Prefab || waveItem.spawnInfoData.spawnType == SpawnType.Variants)
                {
                    waveItem.spawnTracker.quota = spawnQuota;
                }
                else // Herd
                {
                    // Randomize a value for each herd that will spawn and add them up for the total quota
                    for (int i = 0; i < spawnQuota; ++i)
                    {
                        int qty = UnityEngine.Random.Range(waveItem.spawnInfoData.herd.minPopulation, waveItem.spawnInfoData.herd.maxPopulation + 1);
                        herdSpawnQty.Push(qty);
                        waveItem.spawnTracker.quota += qty;
                    } // for i
                }

                // Prepare some important spline information
                if (waveItem.spline != null)
                {
                    waveItem.splinePointSeparation = (waveItem.splineLength / waveItem.spawnTracker.quota) / 100f;
                    waveItem.splinePointSeparation -= ((waveItem.splinePointSeparation * waveItem.spawnTracker.quota) - 1f) / waveItem.spawnTracker.quota;
                } // if

                var totalSpawns = 0;
                var totalHerds = 0;

                var spawnInterval = 0;
                if (waveItem.randomizeSpawnInterval && !waveItem.randomizeForEach)
                {
                    spawnInterval = UnityEngine.Random.Range((int)waveItem.spawnInterval, (int)waveItem.spawnIntervalMax + 1);
                }
                else
                {
                    spawnInterval = (int)waveItem.spawnInterval;
                }
                do
                {
                    if (SoulLinkSpawner.Instance.Paused || waveItem.paused)
                    {
                        yield return null;
                        continue;
                    } // if paused

                    uint chance;
                    if (waveItem.spawnInfoData.probability > 0 && SoulLinkSpawner.Instance.GetTotalSpawns("") < SoulLinkSpawner.Instance.MaxSpawns
                        && SoulLinkSpawner.RandomValues.FindValue((uint)waveItem.spawnInfoData.probability, out chance))
                    {
                        Vector3 origin;
                        Vector3 spawnPos = RandomizeSpawnPosition(waveItem, out origin, out waveItem.spawnInfoData.spawnRot);

                        // Check if there are any spawns of this type within a certain range. If so then skip it
                        if (waveItem.clusterRange > 0f && SoulLinkSpawner.Instance.PoolManager.AnySpawnInRange(waveItem.spawnInfoData.prefab, spawnPos, waveItem.clusterRange))
                        {
                            yield return null;
                            continue;
                        } // if

                        if (waveItem.spawnInfoData.spawnType == SpawnType.Herd)
                        {
                            uint herdID = SoulLinkSpawner.Instance.AddHerd(chance);

                            // Spawn a randomized qty
                            int qtyDesired = herdSpawnQty.Pop();
                            int qty = 0;

                            for (int i = 0; i < qtyDesired; ++i)
                            {
                                bool spawnWasGenerated = false;
                                int numTries = 0;
                                while (!spawnWasGenerated && numTries < NUM_RETRIES)
                                {
                                    foreach (var spawn in waveItem.spawnInfoData.herd.herdPrefabs)
                                    {
                                        // Randomize position in radius around spawnPos
                                        Vector3 newPos = Utility.GetRandomPos(0f, waveItem.spawnInfoData.herd.spawnRadius, _distributeHerdsFromCenter ? origin : spawnPos,
                                            SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);
                                        Vector3 herdSpawnPos = new Vector3(newPos.x, SoulLinkSpawner.Instance.GetYPos(newPos), newPos.z);

                                        float minElevation = 0f;
                                        float maxElevation = 1000f;

                                        if (waveItem.spawnInfoData.overrideElevation)
                                        {
                                            minElevation = waveItem.spawnInfoData.minElevation;
                                            maxElevation = waveItem.spawnInfoData.maxElevation;
                                        }
                                        else
                                        {
                                            minElevation = spawn.overrideElevation ? spawn.minElevation : SoulLinkSpawner.Instance.MinElevation;
                                            maxElevation = spawn.overrideElevation ? spawn.maxElevation : SoulLinkSpawner.Instance.MaxElevation;
                                        }

                                        float minSlopeAngle = spawn.overrideSlopeAngle ? spawn.minSlopeAngle : SoulLinkSpawner.Instance.MinSlopeAngle;
                                        float maxSlopeAngle = spawn.overrideSlopeAngle ? spawn.maxSlopeAngle : SoulLinkSpawner.Instance.MaxSlopeAngle;

                                        // Assign the herd's filters to the individual spawns to validate them properly
                                        spawn.textureRules = waveItem.spawnInfoData.textureRules;
                                        spawn.weatherRules = waveItem.spawnInfoData.weatherRules;

                                        spawn.waveItemGuid = waveItem.guid;
                                        spawn.waveSpawnerGuid = _objectGuid;
                                        spawn.markPersistent = _markSpawnsAsPersistent ? true : waveItem.spawnInfoData.markPersistent;
                                        spawn.ignoreRaycast = waveItem.spawnInfoData.ignoreRaycast;

                                        uint herdSpawnChance = (uint)UnityEngine.Random.Range(1, 101);
                                        if (spawn.probability > 0 && herdSpawnChance < (uint)spawn.probability)
                                        {
                                            spawn.randomChance = 0; // these will not return their values to the pool
                                            spawn.herdID = herdID;

                                            // Note: Ignore player POV can override individual spawns only if set to true.
                                            if (SoulLinkSpawner.Instance.ValidateSpawnRules(spawn, herdSpawnPos, _timeOfDay, waveItem.spawnInfoData.ignorePlayerFOV ? true : spawn.ignorePlayerFOV,
                                                minElevation, maxElevation, minSlopeAngle, maxSlopeAngle, true))
                                            {
                                                ++qty;
                                                ++totalSpawns;
                                                spawnWasGenerated = true;
                                            } // if
                                        } // if

                                        if (qty == qtyDesired) break;
                                    } // foreach

                                    if (qty == qtyDesired) break;

                                    ++numTries;
                                    yield return null;
                                } // while !spawnWasGenerated

                                if (qty == qtyDesired) break;
                            } // for i

                            ++totalHerds;
                        }
                        else
                        {
                            if (waveItem.spawnInfoData.spawnType == SpawnType.Variants && waveItem.spawnInfoData.prefabVariants.Count > 0)
                            {
                                waveItem.spawnInfoData.prefab = 
                                    waveItem.spawnInfoData.prefabVariants[UnityEngine.Random.Range(0, waveItem.spawnInfoData.prefabVariants.Count)];
                            } // if

                            float minElevation = waveItem.spawnInfoData.overrideElevation ? waveItem.spawnInfoData.minElevation : SoulLinkSpawner.Instance.MinElevation;
                            float maxElevation = waveItem.spawnInfoData.overrideElevation ? waveItem.spawnInfoData.maxElevation : SoulLinkSpawner.Instance.MaxElevation;
                            float minSlopeAngle = waveItem.spawnInfoData.overrideSlopeAngle ? waveItem.spawnInfoData.minSlopeAngle : SoulLinkSpawner.Instance.MinSlopeAngle;
                            float maxSlopeAngle = waveItem.spawnInfoData.overrideSlopeAngle ? waveItem.spawnInfoData.maxSlopeAngle : SoulLinkSpawner.Instance.MaxSlopeAngle;

                            waveItem.spawnInfoData.waveItemGuid = waveItem.guid;
                            waveItem.spawnInfoData.waveSpawnerGuid = _objectGuid;
                            waveItem.spawnInfoData.markPersistent = _markSpawnsAsPersistent ? true : waveItem.spawnInfoData.markPersistent;

                            if (SoulLinkSpawner.Instance.ValidateSpawnRules(waveItem.spawnInfoData, spawnPos, _timeOfDay, waveItem.spawnInfoData.ignorePlayerFOV, minElevation, 
                                maxElevation, minSlopeAngle, maxSlopeAngle, true))
                            {
                                ++totalSpawns;
                            } // if
                        }
                    } // if probability

                    if (waveItem.randomizeSpawnInterval && waveItem.randomizeForEach)
                    {
                        spawnInterval = UnityEngine.Random.Range((int)waveItem.spawnInterval, (int)waveItem.spawnIntervalMax + 1);
                    } // if

                    yield return new WaitForSeconds(spawnInterval);
                } while ((waveItem.spawnInfoData.spawnType != SpawnType.Herd && totalSpawns < spawnQuota) ||
                         (waveItem.spawnInfoData.spawnType == SpawnType.Herd && totalHerds < spawnQuota));

                if (waveItem.spawnInfoData.spawnType == SpawnType.Herd)
                {
                    if (totalSpawns < waveItem.spawnTracker.quota)
                    {
                        waveItem.spawnTracker.quota = totalSpawns;
                    } // if  
                } // if

                // Call event handler if this item's spawning is complete for the current repeat. 
                SendEventMessage(OnSpawningCompleteMessageName, waveItem);

                if (waveItem.completeType == WaveItemData.CompleteKind.SpawningComplete)
                {
                    waveItem.completed = true;
                } // if

                // TODO: what about spawns that are never even visible for whatever reason? an we discount them from being counted as missed?

                if (waveItem.completeType == WaveItemData.CompleteKind.EliminationKill || waveItem.completeType == WaveItemData.CompleteKind.EliminationKillOrDespawn)
                {
                    while (!IsWaveItemRepeatCompleted(waveItem))
                    {
                        yield return null;
                    } // while
                }

                // Complete this wave item if the total repeats has been completed or it's set to AutoComplete
                waveItem.completed = (waveItem.completeType == WaveItemData.CompleteKind.AutoComplete) || (repeatCount == waveItem.numberOfRepeats);
                if (waveItem.completed)
                {
                    // See if any waves are dependent on this wave completing before they can start
                    CheckForWaveItemStarts(waveItem);
                } // if

                // Call event handler if this item's spawning is complete for the current repeat. Initial wave is not counted.
                if (repeatCount > 0)
                {
                    if (_onWaveItemRepeatCompletedEventHandler != null)
                    {
                        _onWaveItemRepeatCompletedEventHandler.Invoke();
                    } // if

                    SendEventMessage(OnRepeatCompleteMessageName, waveItem);
                } // if

                if (waveItem.completed)
                {
                    // Call event handler if this item is complete for all repeats
                    SendEventMessage(OnItemCompleteMessageName, waveItem);
                } // if

                ++repeatCount;

                if (waveItem.repeatItem && waveItem.repeatChangeType != WaveItemData.RepeatChangeKind.NoChange)
                {
                    // Increase/decrease min/max spawns per settings
                    if (waveItem.repeatChangeType == WaveItemData.RepeatChangeKind.Increase)
                    {
                        waveItem.minMaxSpawns.x += waveItem.repeatMinMaxChange.x;
                        waveItem.minMaxSpawns.y += waveItem.repeatMinMaxChange.y;
                    }
                    else if (waveItem.repeatChangeType == WaveItemData.RepeatChangeKind.Decrease)
                    {
                        waveItem.minMaxSpawns.x -= waveItem.repeatMinMaxChange.x;
                        waveItem.minMaxSpawns.y -= waveItem.repeatMinMaxChange.y;

                        if (waveItem.minMaxSpawns.x < 0) waveItem.minMaxSpawns.x = 0;
                        if (waveItem.minMaxSpawns.y < 0) waveItem.minMaxSpawns.y = 0;
                    }
                } // if

                // Check for entire wave completion here
                CheckForWaveCompletion();

                // Wait until delay time has been reach and then continue
                float elapsedTime = 0f;
                while (elapsedTime < waveItem.repeatDelay)
                {
                    elapsedTime += Time.deltaTime;

                    yield return null;
                } // while

                // Reset wave tracking so the next repeat will be clean
                ResetWaveItemTracker(waveItem);
            } // for i

            // Ready for a complete wave repeat now
            if (_waveCompleted && _repeatWave && (_currentRepeat != _numberOfRepeats || _repeatType == ResetRepeatKind.Unlimited))
            {
                yield return new WaitForSeconds(_repeatDelay);

                ResetWaveItems(); // for next repeat
                _currentRepeat++;

                _elapsedTime = 0f;
                _waveStarted = false;
                _waveCompleted = false;
            } // if
        } // GenerateWaveItemSpawns()

        /// <summary>
        /// Checks if the currently executing wave has gone out of scope and stops spawn generation and resets for next day
        /// </summary>
        void CheckForTimeOfDayOutOfScope()
        {
            if (!_resetWaveOnNextDay) return;

            if (_waveStarted && !SoulLinkSpawner.Instance.IsInCurrentTimeOfDay(_timeOfDay))
            {
                ForceWaveCompletion();
                ResetWave();
            } // if
        } // CheckForTimeOfDayOutOfScope()

        /// <summary>
        /// Resets everything so the wave can be retriggered on consecutive days
        /// </summary>
        public void ResetWave(bool forceAll = false)
        {
            // Do not reset all day spawwners as they will continue on their own without needing a reset
            if (Utility.IsTimeOfDayAllDay(_timeOfDay) && !forceAll) return;

            StopAllCoroutines();
            ResetWaveItems();

            _currentRepeat = 0;
            _elapsedTime = 0f;
            _waveStarted = false;
            _waveCompleted = false;
        } // ResetWave()

        /// <summary>
        /// Check to see if any wave items are waiting for this wave item to complete before starting
        /// </summary>
        /// <param name="waveItem"></param>
        void CheckForWaveItemStarts(WaveItemData waveItem)
        {
            foreach (var item in _waveItems)
            {
                if (item.canStart) continue;
                item.canStart = (!item.canStart && item.startType == WaveItemData.StartKind.WaveItemComplete && item.waveItemDependency == waveItem.itemName);
            } // foreach
        } // CheckForWaveItemStarts()

        /// <summary>
        /// Check all wave items to see if they have been completed. Call event handlers if completed.
        /// </summary>
        void CheckForWaveCompletion()
        {
            if (_waveCompleted) return;

            lock (waveCompleteLock)
            {
                _waveCompleted = true;
                foreach (var waveItem in _waveItems)
                {
                    if (!waveItem.disable && !waveItem.completed && waveItem.completeType != WaveItemData.CompleteKind.AutoComplete &&
                        (!waveItem.repeatItem || waveItem.repeatType != ResetRepeatKind.Unlimited))
                    {
                        _waveCompleted = false;
                        break;
                    } // if
                } // foreach

                if (_waveCompleted)
                {
                    // Call user's custom triggered event handler
                    if (_onWaveCompletedEventHandler != null)
                    {
                        _onWaveCompletedEventHandler.Invoke();
                    } // if

                    // Tell this wave's wave manager that it is completed
                    if (_currentRepeat == _numberOfRepeats || _hasUnlimitedWaveItems)
                    {
                        _waveManager.WaveCompleted();
                    } // if

                    SoulLinkGlobal.DebugLog(gameObject, "Wave Completed");
                } // if
            } // lock
        } // CheckForWaveCompletion()

        /// <summary>
        /// Force wave to fully complete and reset for some future time
        /// </summary>
        public void ForceWaveCompletion()
        {
            if (_waveCompleted) return;

            // Stop wave item generation
            StopAllCoroutines();

            lock (waveCompleteLock)
            {
                ISpawnable[] spawns = GetComponentsInChildren<ISpawnable>();
                foreach (var spawn in spawns)
                {
                    spawn.ForceDespawn();
                } // foreach

                _waveCompleted = true;

                // Call user's custom triggered event handler
                if (_onWaveCompletedEventHandler != null)
                {
                    _onWaveCompletedEventHandler.Invoke();
                } // if

                _waveManager.WaveCompleted();

                SoulLinkGlobal.DebugLog(gameObject, "Wave Forcibly Completed");
            } // lock
        } // ForceWaveCompletion()

        /// <summary>
        /// Reset the spawn tracker so the next repeat cycle will have clean values
        /// </summary>
        /// <param name="waveItem"></param>
        public void ResetWaveItemTracker(WaveItemData waveItem)
        {
            waveItem.spawnTracker.quota = 0;
            waveItem.spawnTracker.totalSpawns = 0;
            waveItem.spawnTracker.spawnsKilled = 0;
            waveItem.spawnTracker.spawnsMissed = 0;

            DeactivateWaveTrigger(waveItem);
        } // ResetWaveItemTracker()

        /// <summary>
        /// Randomize a spawn position based on the spawning method
        /// </summary>
        /// <returns></returns>
        public Vector3 RandomizeSpawnPosition(WaveItemData waveItem, out Vector3 origin, out Quaternion spawnRot)
        {
            origin = Vector3.zero;
            spawnRot = Quaternion.identity;

            if (waveItem.spawningMethod == SpawningMethod.SpawnRadius)
            {
                origin = waveItem.triggerOrigin != null ? waveItem.triggerOrigin.position : transform.position;
                var spawnPos = Utility.GetRandomPos(waveItem.minimumSpawnRange, waveItem.spawnRadius, origin,
                    SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);

                // Sample terrain to get proper y position for spawn
                return new Vector3(spawnPos.x, !_is3D ? spawnPos.y : SoulLinkSpawner.Instance.GetYPos(new Vector3(spawnPos.x, origin.y, spawnPos.z)), spawnPos.z);
            }
            else if (waveItem.spawningMethod == SpawningMethod.SpawnBounds && waveItem.spawnAreaBounds.Count > 0)
            {
                // First randomize which bounds to use for this spawn
                var index = UnityEngine.Random.Range(0, waveItem.spawnAreaBounds.Count);

                // Now randomize a position within the bounds
                var pos = Utility.GetRandomPosInBounds(waveItem.spawnAreaBounds[index].bounds, SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);

                origin = waveItem.spawnAreaBounds[index].bounds.center;
                if (_is3D)
                {
                    // Sample terrain to get proper y position for spawn
                    return new Vector3(pos.x, SoulLinkSpawner.Instance.GetYPos(new Vector3(pos.x, waveItem.spawnAreaBounds[index].min.y, pos.y)), pos.y);
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
            else if (waveItem.spawningMethod == SpawningMethod.SpawnPoints && waveItem.spawnPoints.Count > 0)
            {
                Vector3 spawnPos = transform.position; // fallback if points are null
                bool isValidPoint = waveItem.spawnPoints[waveItem.spawnPointIndex].transform != null;
                if (isValidPoint)
                {
                    if (waveItem.spawnPoints[waveItem.spawnPointIndex].useRadius)
                    {
                        spawnPos = Utility.GetRandomPos(0, waveItem.spawnPoints[waveItem.spawnPointIndex].spawnRadius,
                            waveItem.spawnPoints[waveItem.spawnPointIndex].transform.position,
                            SoulLinkSpawner.Instance.SpawnMode, SoulLinkSpawner.Instance.SpawnAxes);
                    }
                    else
                    {
                        spawnPos = waveItem.spawnPoints[waveItem.spawnPointIndex].transform.position;
                    }
                }
                else
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Error: Spawn Point was null. Spawning at WavSpawner's origin.");
                }

                origin = isValidPoint ? waveItem.spawnPoints[waveItem.spawnPointIndex].transform.position : spawnPos;
                spawnRot = isValidPoint ? waveItem.spawnPoints[waveItem.spawnPointIndex].transform.rotation : spawnRot;

                ++waveItem.spawnPointIndex;
                if (waveItem.spawnPointIndex >= waveItem.spawnPoints.Count)
                {
                    waveItem.spawnPointIndex = 0;
                } // if

                return spawnPos;
            }
            else if (waveItem.spawningMethod == SpawningMethod.Spline && waveItem.spline != null)
            {
                if (waveItem.splinePositioning == SplinePositioning.Start)
                {
                    return waveItem.spline.GetWorldPosition(0); // return position at start of spline
                }
                else if (waveItem.splinePositioning == SplinePositioning.End)
                {
                    return waveItem.spline.GetWorldPosition(1); // return position at end of spline
                }
                else
                {
                    // Use spawnPointIndex to distribute along spline
                    var spawnPos = waveItem.spline.GetWorldPosition(waveItem.spawnPointIndex * waveItem.splinePointSeparation);
                    ++waveItem.spawnPointIndex;
                    if (waveItem.spawnPointIndex >= waveItem.spawnTracker.quota)
                    {
                        waveItem.spawnPointIndex = 0;
                    } // if

                    return spawnPos;
                }
            }

            return Vector3.zero;
        } // RandomizeSpawnPosition()

        /// <summary>
        /// Increments the total active spawns for this wave item
        /// </summary>
        /// <param name="waveItemGuid"></param>
        public void IncSpawnTotal(string waveItemGuid)
        {
            var waveItem = _waveItems.Find(x => x.guid == waveItemGuid);
            if (waveItem != null)
            {
                ++waveItem.spawnTracker.totalSpawns;
            } // if
        } // IncSpawnTotal()

        /// <summary>
        /// Inform this wave spawner that one of it's spawns died
        /// </summary>
        /// <param name="waveItemGuid"></param>
        public void SpawnDied(string waveItemGuid)
        {
            var waveItem = _waveItems.Find(x => x.guid == waveItemGuid);
            if (waveItem != null)
            {
                --waveItem.spawnTracker.totalSpawns;
                ++waveItem.spawnTracker.spawnsKilled;
            } // if
        } // SpawnDied()

        /// <summary>
        /// Inform this wave spawner that one of it's spawns despawned without dying
        /// </summary>
        /// <param name="waveItemGuid"></param>
        public void SpawnDespawned(string waveItemGuid)
        {
            var waveItem = _waveItems.Find(x => x.guid == waveItemGuid);
            if (waveItem != null)
            {
                ++waveItem.spawnTracker.spawnsMissed;
            } // if
        } // SpawnDespawned()

        /// <summary>
        /// Check for completion of this wave item for the current repeat
        /// </summary>
        /// <param name="waveItem"></param>
        public bool IsWaveItemRepeatCompleted(WaveItemData waveItem)
        {
            if (waveItem.completeType == WaveItemData.CompleteKind.EliminationKill)
            {
                return (waveItem.spawnTracker.spawnsKilled == waveItem.spawnTracker.quota);
            }
            else if (waveItem.completeType == WaveItemData.CompleteKind.EliminationKillOrDespawn)
            {
                return ((waveItem.spawnTracker.spawnsKilled + waveItem.spawnTracker.spawnsMissed) == waveItem.spawnTracker.quota);
            }

            return false;
        } // IsWaveItemRepeatCompleted()

#if UNITY_EDITOR
        /// <summary>
        /// OnDrawGizmosSelected
        /// </summary>
        void OnDrawGizmosSelected()
        {
            foreach (var waveItem in _waveItems)
            {
                if (waveItem.disable) continue;

                if (waveItem.spawningMethod == SpawningMethod.SpawnRadius && waveItem.spawnRadius == 0f)
                {
                    Gizmos.color = new Color(1, 1, 1, 0.75F);
                    Gizmos.DrawIcon(transform.position, "SpawnSilhouette.png");
                }
                else if (waveItem.spawningMethod == SpawningMethod.SpawnPoints)
                {
                    foreach (var point in waveItem.spawnPoints)
                    {
                        if (point.transform != null && !point.useRadius)
                        {
                            Gizmos.color = new Color(1, 1, 1, 0.75F);
                            Gizmos.DrawIcon(point.transform.position, "SpawnSilhouette.png", true);
                        }
                    } // foreach
                }
            } // foreach waveItem
        } // OnDrawGizmosSelected()
#endif
    } // class WaveSpawner
} // namespace Magique.SoulLink