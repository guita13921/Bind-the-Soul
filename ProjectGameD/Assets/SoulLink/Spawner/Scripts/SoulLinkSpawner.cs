using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if UNITY_2020_3_OR_NEWER && !SURVIVAL_ENGINE && !SURVIVAL_ENGINE_ONLINE && SOULLINK_USE_AINAV
using Unity.AI.Navigation;
using System;
#endif

#if SOULLINK_USE_GLOBALSNOW
using GlobalSnowEffect;
#endif

#if MAPMAGIC2
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Terrains;
#endif

#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkSpawner : MonoBehaviour
    {
        public SoulLinkSpawnerDatabase Database
        {
            get { return _SoulLinkSpawnerDatabase; }
            set { _SoulLinkSpawnerDatabase = value; }
        }

        [SerializeField]
        private SoulLinkSpawnerDatabase _SoulLinkSpawnerDatabase;

        public PoolManager PoolManager { get; set; }

        public SpawnMode SpawnMode
        { get { return _spawnMode; } }

        [Tooltip("Select whether spawner is being used for a 3D or 2D game environment.")]
        [SerializeField]
        private SpawnMode _spawnMode = SpawnMode.SpawnIn3D;

        public SpawnAxes SpawnAxes
        {
            get { return _spawnAxes; }
        }

        [SerializeField]
        private SpawnAxes _spawnAxes = SpawnAxes.XY;

        public float SpawnRadius
        { get { return _spawnRadius; } }

        [TooltipAttribute("The maximum distance to spawn AI from the player.")]
        [SerializeField]
        private float _spawnRadius = 75f;

        public float MinimumSpawnRange
        { get { return _minimumSpawnRange; } }

        [TooltipAttribute("The minimum distance an AI can spawn from the player.")]
        [SerializeField]
        private float _minimumSpawnRange = 15f;

        [TooltipAttribute("How close spawns of the same type can be clustered. This value is modified by spawn radius and population cap.")]
        [SerializeField]
        private float _clusterRange = 4f;

        public float SpawnRate
        { get { return _spawnRate; } }

        [TooltipAttribute("How frequently to spawn AI. Populations will increase more rapidly with larger numbers.")]
        [Range(1, 20)]
        [SerializeField]
        private int _spawnRate = 10;

        public int MaxSpawns
        { get { return _maxSpawns; } }

        // Calculate this value when we start based on the totals from all categories
        private int _maxSpawns = 50;

        [TooltipAttribute("Use spawn circles to distribute spawns more evenly throughout the environment.")]
        [SerializeField]
        private bool _useSpawnCircles = true;

        [TooltipAttribute("The number of concentric circles around the player to generate spawns in.")]
        [Range(10, 20)]
        [SerializeField]
        private int _spawnCircles = 10;

        public int DefaultPopulationCap
        {
            get { return _defaultPopulationCap; }
            set { _defaultPopulationCap = value; }
        }
        [TooltipAttribute("The default population cap to assign to new spawn definitions.")]
        [SerializeField]
        private int _defaultPopulationCap = 10;

        public bool AutoAddSpawnableInterface
        {
            get { return _autoAddSpawnableInterface; }
            set { _autoAddSpawnableInterface = value; }
        }
        [TooltipAttribute("Check this if you want SoulLink Spawner to auto add the best available ISpawnable interface if your object does not have one.")]
        [SerializeField]
        private bool _autoAddSpawnableInterface = true;

        public string DefaultSpawnAction
        {
            get { return _defaultSpawnAction; }
            set { _defaultSpawnAction = value; }
        }
        [TooltipAttribute("The default spawn action to assign to new spawn definitions.")]
        [SerializeField]
        private string _defaultSpawnAction = "";

        public SpawnableSettingsObject SpawnableSettings
        {
            get { return _spawnableSettings; }
            set { _spawnableSettings = value; }
        }
        [TooltipAttribute("Drag a pre-defined SpawnableSettings object into this field if you want auto added Spawnables to use those settings. Otherwise the defaults will be used.")]
        [SerializeField]
        private SpawnableSettingsObject _spawnableSettings;

        [TooltipAttribute("Reference to a custom spawn validator for fine tuning your spawning filters.")]
        [SerializeField]
        private SpawnValidator _spawnValidator;

        public float DelaySpawnsOnStart
        { get { return _delaySpawnsOnStart; } }

        [SerializeField]
        private float _delaySpawnsOnStart = 5f;

        public bool SpawnOutsidePlayerFOV
        { get { return _spawnOutsidePlayerFOV; } }
        [TooltipAttribute("Check this if you only want spawns to appear when they are out of the player's FOV.")]
        [SerializeField]
        private bool _spawnOutsidePlayerFOV = false;

        public bool DespawnOutsidePlayerFOV
        { get { return _despawnOutsidePlayerFOV; } }
        [TooltipAttribute("Check this if you only want spawns to disappear when they are out of the player's FOV.")]
        [SerializeField]
        private bool _despawnOutsidePlayerFOV = false;

        public float MinElevation
        { get { return _minElevation; } }

        [TooltipAttribute("The minimum elevation that this AI is allowed to spawn at.")]
        [SerializeField]
        private float _minElevation = 0f;

        public float MaxElevation
        { get { return _maxElevation; } }

        [TooltipAttribute("The maximum elevation that this AI is allowed to spawn at.")]
        [SerializeField]
        private float _maxElevation = 1000f;

        public float MinSlopeAngle
        { get { return _minSlopeAngle; } }

        [TooltipAttribute("The minimum slope angle that this AI is allowed to spawn on.")]
        [Range(0, 90)]
        [SerializeField]
        private float _minSlopeAngle = 0f;

        public float MaxSlopeAngle
        { get { return _maxSlopeAngle; } }

        [TooltipAttribute("The maximum slope angle that this AI is allowed to spawn on.")]
        [Range(0, 90)]
        [SerializeField]
        private float _maxSlopeAngle = 50f;

#if MAPMAGIC2
        [TooltipAttribute("How much influence must a biome have in order to spawn in a given position?")]
        [SerializeField]
        private float _biomeThreshold = 0.5f;
#endif

        private bool _usePolaris = false;

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

#if SOULLINK_USE_GLOBALSNOW
        private GlobalSnow _globalSnow;
#endif

        public bool AutoDetectPlayer
        {
            get { return _autoDetectPlayer; }
            set { _autoDetectPlayer = value; }
        }

        [TooltipAttribute("Check this if you want SpawnerManager to auto detect the player in your scene.")]
        [SerializeField]
        private bool _autoDetectPlayer = true;

        public string PlayerTag
        {
            get { return _playerTag; }
        }
        [TooltipAttribute("Select the Tag that is assigned to your player so that auto detect can find the player when the scene runs.")]
        [SerializeField]
        private string _playerTag = "Player";

        public Transform PlayerTransform
        {
            get { return _playerTransform; }
            set { _playerTransform = value; }
        }

        [SerializeField]
        private Transform _playerTransform;

        public float TimeSyncFrequency
        {
            get { return _timeSyncFrequency; }
        }
        [TooltipAttribute("How often to synchronize time of day with the time/sky system.")]
        [SerializeField]
        private float _timeSyncFrequency = 5f;

        [TooltipAttribute("How often to synchronize with the weather system.")]
        [SerializeField]
        private float _weatherSyncFrequency = 5f;

        public bool GenerateNavMesh
        { get { return _generateNavMesh; } }

        [TooltipAttribute("Generate local nav mesh at runtime? Check this if you are not baking your navmesh manually.")]
        [SerializeField]
        private bool _generateNavMesh = false;

        [TooltipAttribute("Determines the number of local nav meshes that will be built (e.g. 8 x 8 grid).")]
        [Range(1, 10)]
        [SerializeField]
        private int _cellDivisions = 8;

        [TooltipAttribute("When placing spawns, how large of an area to check for nav meshes to see if they've been built.")]
        [SerializeField]
        private float _navCheckRadius = 32f;

        public float AgentSlope
        { get { return _agentSlope; } }

        [SerializeField]
        private float _agentSlope = 45f;

        public float AgentHeight
        { get { return _agentHeight; } }

        [SerializeField]
        private float _agentHeight = 2f;

        public float AgentStepHeight
        { get { return _agentStepHeight; } }

        [SerializeField]
        private float _agentStepHeight = 0.4f;

        public bool GenerateNavMeshLinks
        { get { return _generateNavMeshLinks; } }

        [TooltipAttribute("Generate nav mesh links to connect nav mesh areas at runtime?")]
        [SerializeField]
        private bool _generateNavMeshLinks = true;

        [TooltipAttribute("How many nav mesh links to generate for each cell.")]
        [SerializeField]
        private int _linksPerCell = 16;

        public List<string> LinkActivatorTags
        { get { return _linkActivatorTags; } }
        [SerializeField]
        private List<string> _linkActivatorTags = new List<string>();

        public LayerMask NavMeshIgnoreLayers
        { get { return _navMeshIgnoreLayers; } }
        [TooltipAttribute("Layers to ignore when checking for walkable surfaces to place AI on.")]
        [SerializeField]
        private LayerMask _navMeshIgnoreLayers;

        public float RaycastDistance
        {
            get { return _raycastDistance; }
            set { _raycastDistance = value; }
        }

        [TooltipAttribute("The distance to check above the terrain for walkable surfaces to place AI on. Should be low for indoor environments.")]
        [SerializeField]
        private float _raycastDistance = 1000f;

        public bool DisableBiomeSpawns
        { get { return _disableBiomeSpawns; } }

        [TooltipAttribute("Check this to disable spawns in Biomes so that only Area spawns are active.")]
        [SerializeField]
        private bool _disableBiomeSpawns = false;

        public bool DisableAreaSpawns
        { get { return _disableAreaSpawns; } }

        [TooltipAttribute("Check this to disable Area spawns so that only Biomes spawns are active.")]
        [SerializeField]
        private bool _disableAreaSpawns = false;

        public bool Paused { get; set; }

        public float CurrentTime { get; private set; } = 0f;

        public string CurrentTimeString
        { get { return Utility.BuildTimeString(CurrentTime); } }

        public string CurrentDateString
        { get { return Utility.BuildDateString(GameTime); } }

        public float CurrentSnowStrength { get; private set; } = 0f;

        public float CurrentWetness { get; private set; } = 0f;

        public float CurrentTemperature { get; private set; } = 75f;

#if MAPMAGIC2
        private MapMagicObject _mapMagicObject;
        private TerrainTile[] _tiles;
        private DirectMatricesHolder[] _matricesHolders;
#endif

        public List<BiomeTerrain> Biomes
        { get { return _biomes; } }

        [SerializeField]
        private List<BiomeTerrain> _biomes = new List<BiomeTerrain>();

        public BaseGameTime GameTime { get; set; } = null;

        public event Action OnTimeSync = delegate { };
        public event Action OnWeatherSync = delegate { };

        private List<string> _currentTOD = new List<string>();
        private List<string> _currentBiomes = new List<string>();
        private Dictionary<Transform, int> _populations = new Dictionary<Transform, int>();

        private BaseWeatherSystem _weather;
        private bool _useMapMagic = false;

        private List<SpawnArea> _suppressionAreas = new List<SpawnArea>();
        private Dictionary<string, SpawnArea> _spawnAreas = new Dictionary<string, SpawnArea>();
        private Dictionary<string, WaveSpawner> _waveSpawners = new Dictionary<string, WaveSpawner>();

        static public SoulLinkSpawner Instance { get; private set; }

        public int TotalSpawns
        { get { return GetTotalSpawns(""); } }

        // Dictionary of total spawns by category name
        private Dictionary<string, int> _totalSpawns = new Dictionary<string, int>();

#if SOULLINK_USE_AINAV
        private AsyncNavMeshBuilder _navAreaPrefab;
        private NavMeshLink _navMeshLinkPrefab;
        private NavMeshLinkActivator _navMeshLinkActivatorPrefab;
        private SpawnNavChecker _spawnNavCheckerPrefab;
        private SpawnNavChecker _spawnNavChecker;
#endif
        private SpawnPlaceholder _spawnPlaceholderPrefab;
        private ObjectPool _navMeshLinkPool;

        private Stack<SpawnPlaceholder> _spawnPlaceholders = new Stack<SpawnPlaceholder>();

        private Terrain _targetTerrain;
        private const int VALIDATION_RETRIES = 3;

        private float _sphereCastRadius = 0.25f;
        private float _sphereCastDistance = 1.25f;

        private BaseQuestManager _questManager;

        // Pool of random values for determining spawns from their probability
        public static RandomValuePool RandomValues = new RandomValuePool();

        static public bool Quitting { get; private set; } = false;

        public bool Initializing { get; private set; } = false;

        private Dictionary<uint, HerdData> _herds = new Dictionary<uint, HerdData>();
        private Stack<uint> _herdIDs = new Stack<uint>(); // reusable herd ids
        private uint _herdID = 0;

        /// <summary>
        /// Add a new herd using the next available herd ID or a previously used ID that is no longer in use
        /// </summary>
        /// <param name="randomChance"></param>
        /// <returns></returns>
        public uint AddHerd(uint randomChance)
        {
            uint herdID;
            if (_herdIDs.Count > 0)
            {
                herdID = _herdIDs.Pop();
            }
            else
            {
                herdID = ++_herdID;
                _herds[herdID] = new HerdData();
            }

            _herds[herdID].randomChance = randomChance;
            _herds[herdID].herdID = herdID;

            return herdID;
        } // AddHerd()

        /// <summary>
        /// Add a spawnable to the specified herd ID
        /// </summary>
        /// <param name="spawnable"></param>
        /// <param name="herdID"></param>
        public void AddToHerd(ISpawnable spawnable, uint herdID)
        {
            if (herdID == 0 || !_herds.ContainsKey(herdID)) return;

            _herds[herdID].herdSpawns.Add(spawnable);
        } // AddToHerd()

        public void RemoveFromHerd(ISpawnable spawnable)
        {
            var herdID = spawnable.GetHerdID();
            if (!_herds.ContainsKey(herdID)) return;

            _herds[herdID].herdSpawns.Remove(spawnable);

            if (_herds[herdID].herdSpawns.Count == 0)
            {
                RandomValues.ReturnValue(_herds[herdID].randomChance);
                _herdIDs.Push(herdID);
            } // if
        } // RemoveFromHerd()

        static void Quit()
        {
            Quitting = true;
        } // Quit()

        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            Quitting = false;
            Application.quitting += Quit;
        } // RunOnStart()

        /// <summary>
        ///  Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            PoolManager = FindObjectOfType<PoolManager>();

            _spawnPlaceholderPrefab = Resources.Load<SpawnPlaceholder>("SpawnPlaceholder");

#if SOULLINK_USE_AINAV
            // Load resource for local nav mesh instances and spawn nav checker
            _navAreaPrefab = Resources.Load<AsyncNavMeshBuilder>("NavArea");
            _navMeshLinkPrefab = Resources.Load<NavMeshLink>("NavMeshLink");
            _navMeshLinkActivatorPrefab = Resources.Load<NavMeshLinkActivator>("NavMeshLinkActivator");
            _spawnNavCheckerPrefab = Resources.Load<SpawnNavChecker>("SpawnNavChecker");

            _navMeshLinkPool = gameObject.AddComponent<ObjectPool>();
            _navMeshLinkPool.Prefab = _navMeshLinkPrefab.transform;
            _navMeshLinkPool.InitialQuantity = 128;
            _navMeshLinkPool.GrowCapacity = true;
            _navMeshLinkPool.HideInstancesInHierarchy = true;
            _navMeshLinkPool.hideFlags = HideFlags.HideInInspector;
            _navMeshLinkPool.CreatePool(PoolManager.transform);

            _spawnNavChecker = Instantiate(_spawnNavCheckerPrefab);
            _spawnNavChecker.GetComponent<SphereCollider>().radius = _navCheckRadius;
            _spawnNavChecker.gameObject.SetActive(false);
#endif
#if MAPMAGIC2
            _matricesHolders = FindObjectsOfType<DirectMatricesHolder>();
            _mapMagicObject = FindObjectOfType<MapMagicObject>();
            _useMapMagic = (_mapMagicObject != null && _matricesHolders.Length > 0);
#endif

#if GRIFFIN_2021
            _terrains = FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif

#if SOULLINK_USE_GLOBALSNOW
            _globalSnow = FindObjectOfType<GlobalSnow>();
#endif

            GameTime = FindObjectOfType<BaseGameTime>();
            if (GameTime == null)
            {
                Debug.LogError("SoulLink Spawner Error: There is no time/sky object in the scene. Time of Day functionality will be disabled.");
            } // if

            _weather = FindObjectOfType<BaseWeatherSystem>();
            if (_weather == null)
            {
                Debug.LogWarning("SoulLink Spawner Warning: There is no weather system object in the scene. Weather filters will be disabled.");
            } // if

            // Insure that _navMeshIgnoreLayers has Ignore Raycast. It is required.
            _navMeshIgnoreLayers |= (1 << LayerMask.NameToLayer("Ignore Raycast"));

            // Initialize biomes as necessary
            foreach (var biome in _biomes)
            {
                biome.Initialize();
            } // forrach

            // Find a quest manager if there is one
            _questManager = FindObjectOfType<BaseQuestManager>();
        } // Awake()

        /// <summary>
        ///  Start
        /// </summary>
        private void Start()
        {
            Initializing = true;

            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            // Setup PoolManager with all prefabs
            SpawnRegion[] regions = Resources.FindObjectsOfTypeAll<SpawnRegion>() as SpawnRegion[];
            foreach (var region in regions)
            {
                UpdatePoolManager(region.Database);
            } // foreach

            if (Database != null)
            {
                UpdatePoolManager(Database);

                // Calculate max spawns for all categories
                _maxSpawns = Database.spawnCategories.Sum(e => e.maxSpawns);
            }
            else
            {
                Debug.LogError("SoulLinkSpawner Error: There is no database assigned.");
            }

            if (_spawnRadius < _minimumSpawnRange)
            {
                Debug.LogError("SoulLinkSpawner Error: Your Spawn radius is smaller then your Minimum Spawn range. Nothing will spawn.");
            } // if

            if (_autoDetectPlayer)
            {
                var obj = GameObject.FindGameObjectWithTag(_playerTag);
                if (obj != null)
                {
                    _playerTransform = obj.transform;
                } // if
            }
            else
            {
                if (_playerTransform != null)
                {
                    _playerTag = _playerTransform.tag;
                } // if
            }

            // Create a pool of spawn placeholder instances
            if (_spawnOutsidePlayerFOV)
            {
                for (int i = 0; i < _maxSpawns; ++i)
                {
                    _spawnPlaceholders.Push(CreatePlaceholder());
                } // for i
            } // if

#if SOULLINK_USE_AINAV
            if (_useMapMagic)
            {
#if MAPMAGIC2
                // Get all terrain tiles that exist right now and prepare them for local nav mesh generation
                if (_generateNavMesh)
                {
                    var tiles = _mapMagicObject.GetComponentsInChildren<TerrainTile>();
                    foreach (var tile in tiles)
                    {
                        AddNavMeshAreasMM2(tile);
                    } // foreach

                    TerrainTile.OnTileApplied += TileApplied;
                } // if
#endif
            }
            else if (_usePolaris)
            {
#if GRIFFIN_2021
                if (_generateNavMesh)
                {
                    foreach (var tile in _terrains)
                    {
                        AddNavMeshAreasPolaris(tile);
                    } // foreach
                } // if
#endif
            }
            else
            {
                if (_generateNavMesh)
                {
                    var terrains = Terrain.activeTerrains;
                    foreach (var tile in terrains)
                    {
                        AddNavMeshAreas(tile);
                    } // foreach
                } // if
            }
#endif

            // Generate the random values required for generating spawns by probability
            RandomValues.GenerateValues((uint)MaxSpawns);

            PreCheckSpawnerSetup();

            // Do an immediate time sync at startup
            DoSyncTimeOfDay();

            StartCoroutine(SyncTimeOfDay());
            StartCoroutine(SyncWeatherSystem());
            StartCoroutine(GenerateSpawns());

            Initializing = false;
        } // Start()

        /// <summary>
        /// Create a new placeholder instance
        /// </summary>
        /// <returns></returns>
        SpawnPlaceholder CreatePlaceholder()
        {
            var placeHolder = Instantiate(_spawnPlaceholderPrefab, transform);
            placeHolder.gameObject.SetActive(false);
            placeHolder.ResetToDefault();
            placeHolder.hideFlags = HideFlags.HideInHierarchy;

            return placeHolder;
        } // CreatePlaceholder()

        /// <summary>
        /// An initial check to determine if anything is going to spawn. Issue a warning if nothing will be spawning.
        /// </summary>
        void PreCheckSpawnerSetup()
        {
            if (Database == null)
            {
                Debug.LogWarning("SoulLinkSpawner has no database assigned. Nothing will spawn.");
                return;
            } // if

            bool activeAreas = false;
            SpawnArea[] spawnAreas = Resources.FindObjectsOfTypeAll(typeof(SpawnArea)) as SpawnArea[];
            foreach (var area in spawnAreas)
            {
                if (area.isActiveAndEnabled)
                {
                    activeAreas = true;
                    break;
                } // if
            } // foreach

            bool activeWaveSpawners = false;
            WaveSpawner[] waveSpawners = Resources.FindObjectsOfTypeAll(typeof(WaveSpawner)) as WaveSpawner[];
            foreach (var spawner in waveSpawners)
            {
                if (spawner.isActiveAndEnabled)
                {
                    activeWaveSpawners = true;
                    break;
                } // if
            } // foreach

            // TODO: also check now for SpawnOnTrigger components as well

            if ((spawnAreas.Length == 0 || activeAreas == false) && (waveSpawners.Length == 0 || activeWaveSpawners == false) && Database.biomeSpawnData.Count == 0)
            {
                Debug.LogWarning("SoulLinkSpawner detected that you have no active Spawners in your scene and no spawns are defined for your Biomes. Nothing will spawn.");
            } // if
        } // PreCheckSpawnerSetup()

        /// <summary>
        ///  SwitchDatabase
        /// </summary>
        /// <param name="database"></param>
        public void SwitchDatabase(SoulLinkSpawnerDatabase database)
        {
            // Stop GenerateSpawns co-routine
            StopCoroutine(GenerateSpawns());

            // Change database reference
            _SoulLinkSpawnerDatabase = database;

            // Restart Generate Spawns co-routine
            StartCoroutine(GenerateSpawns());
        } // SwitchDatabase()

#if SOULLINK_USE_AINAV

#if MAPMAGIC2
        /// <summary>
        /// TileApplied
        /// </summary>
        /// <param name="tile"></param>
        void TileApplied(TerrainTile tile, TileData data, StopToken token)
        {
            if (!Application.isPlaying) return;
            if (_generateNavMesh)
            {
                AddNavMeshAreasMM2(tile);
            } // if
        } // TileApplied()

        /// <summary>
        /// AddNavMeshAreasMM2
        /// </summary>
        /// <param name="tile"></param>
        void AddNavMeshAreasMM2(TerrainTile tile)
        {
            // Must add nav mesh source tag component to all main terrain objects
            var mainTerrain = tile.GetTerrain(false);
            if (mainTerrain != null)
            {
                if (mainTerrain.GetComponent<SoulLinkNavMeshSourceTag>() == null)
                {
                    mainTerrain.gameObject.AddComponent<SoulLinkNavMeshSourceTag>();
                } // if

                CreateNavMeshAreas(tile.transform, mainTerrain.terrainData.size, mainTerrain.terrainData.bounds);
            } // if
        } // AddNavMeshAreasMM2()
#endif

#if GRIFFIN_2021
        void AddNavMeshAreasPolaris(GStylizedTerrain tile)
        {
            // Must add nav mesh source tag component to the terrain object
            if (tile.GetComponent<SoulLinkNavMeshSourceTag>() == null)
            {
                tile.gameObject.AddComponent<SoulLinkNavMeshSourceTag>();
            } // if

            CreateNavMeshAreas(tile.transform, tile.Bounds.size, tile.Bounds);
        } // AddNavMeshAreasPolaris()
#endif

        /// <summary>
        ///  AddNavMeshAreas
        /// </summary>
        /// <param name="tile"></param>
        void AddNavMeshAreas(Terrain tile)
        {
            // Must add nav mesh source tag component to the terrain object
            if (tile.GetComponent<SoulLinkNavMeshSourceTag>() == null)
            {
                tile.gameObject.AddComponent<SoulLinkNavMeshSourceTag>();
            } // if

            CreateNavMeshAreas(tile.transform, tile.terrainData.size, tile.terrainData.bounds);
        } // AddNavMeshAreas()

        /// <summary>
        ///  CreateNavMeshAreas
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="terrainData"></param>
        void CreateNavMeshAreas(Transform tile, Vector3 tileSize, Bounds tileBounds)
        {
            var cellSize = (tileSize.x / _cellDivisions);

            float startX = (cellSize / 2f);
            float startZ = (cellSize / 2f);

            float x, z;
            for (int row = 0; row < _cellDivisions; ++row)
            {
                x = startX + (row * cellSize);
                for (int col = 0; col < _cellDivisions; ++col)
                {
                    z = startZ + (col * cellSize);
                    var navArea = Instantiate(_navAreaPrefab, tile);
                    navArea.gameObject.GetComponent<AsyncNavMeshBuilder>().enabled = false; // starts disabled and enables only when spawns are placed within it
                    navArea.SetSize((int)cellSize, (int)tileSize.y);
                    navArea.transform.localPosition = new Vector3(x, tileSize.y / 2f, z);
                    navArea.gameObject.hideFlags = HideFlags.HideInHierarchy;

                    if (_generateNavMeshLinks)
                    {
                        CreateNavMeshLinks(navArea.transform, tileBounds.center.y, cellSize);
                    } // if
                } // for col
            } // for row
        } // CreateNavMeshAreas()

        /// <summary>
        ///  CreateNavMeshLinks
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tileCenter"></param>
        /// <param name="cellSize"></param>
        void CreateNavMeshLinks(Transform parent, float tileCenter, float cellSize)
        {
            // Create NavMeshLink objects to connect each area.
            float spacing = cellSize / _linksPerCell;
            for (int i = 0; i < _linksPerCell; ++i)
            {
                Vector3 linkPos = new Vector3(parent.transform.position.x - (cellSize / 2f) + (i * spacing), tileCenter, parent.transform.position.z + (cellSize / 2f));
                Vector3 navMeshLinkPos = new Vector3(linkPos.x, GetTerrainYPos(linkPos.x, linkPos.y, linkPos.z), linkPos.z);
                var width = _navMeshLinkPrefab.width / 2f;
                Vector3 vec1Pos = new Vector3(navMeshLinkPos.x - width, navMeshLinkPos.y, navMeshLinkPos.z);
                Vector3 vec2Pos = new Vector3(navMeshLinkPos.x + width, navMeshLinkPos.y, navMeshLinkPos.z);
                Vector3 vec1 = new Vector3(vec1Pos.x, GetTerrainYPos(vec1Pos), vec1Pos.z);
                Vector3 vec2 = new Vector3(vec2Pos.x, GetTerrainYPos(vec2Pos), vec2Pos.z);
                float angleZ = Angle(vec1, vec2, _navMeshLinkPrefab.width);
                if (vec1.y > vec2.y) angleZ *= -1f;

                Vector3 vec3Pos = new Vector3(navMeshLinkPos.x, navMeshLinkPos.y, navMeshLinkPos.z + _navMeshLinkPrefab.startPoint.z);
                Vector3 vec4Pos = new Vector3(navMeshLinkPos.x, navMeshLinkPos.y, navMeshLinkPos.z + _navMeshLinkPrefab.endPoint.z);
                Vector3 vec3 = new Vector3(vec3Pos.x, GetTerrainYPos(vec3Pos), vec3Pos.z);
                Vector3 vec4 = new Vector3(vec4Pos.x, GetTerrainYPos(vec4Pos), vec4Pos.z);
                float angleX = Angle(vec3, vec4, Vector3.Distance(_navMeshLinkPrefab.startPoint, _navMeshLinkPrefab.endPoint));
                if (vec4.y > vec3.y) angleX *= -1f;

                var navMeshLinkActivator = Instantiate(_navMeshLinkActivatorPrefab, parent.transform);
                navMeshLinkActivator.transform.position = navMeshLinkPos;
                navMeshLinkActivator.SetAngles(angleX, angleZ);
            } // for i

            for (int i = 0; i < _linksPerCell; ++i)
            {
                Vector3 linkPos = new Vector3(parent.transform.position.x + (cellSize / 2f), tileCenter, parent.transform.position.z - (cellSize / 2f) + (i * spacing));
                Vector3 navMeshLinkPos = new Vector3(linkPos.x, GetTerrainYPos(linkPos.x, linkPos.y, linkPos.z), linkPos.z);
                var width = _navMeshLinkPrefab.width / 2f;
                Vector3 vec1Pos = new Vector3(navMeshLinkPos.x - width, navMeshLinkPos.y, navMeshLinkPos.z);
                Vector3 vec2Pos = new Vector3(navMeshLinkPos.x + width, navMeshLinkPos.y, navMeshLinkPos.z);
                Vector3 vec1 = new Vector3(vec1Pos.x, GetTerrainYPos(vec1Pos), vec1Pos.z);
                Vector3 vec2 = new Vector3(vec2Pos.x, GetTerrainYPos(vec2Pos), vec2Pos.z);
                float angleZ = Angle(vec1, vec2, _navMeshLinkPrefab.width);
                if (vec1.y > vec2.y) angleZ *= -1f;

                Vector3 vec3Pos = new Vector3(navMeshLinkPos.x, navMeshLinkPos.y, navMeshLinkPos.z + _navMeshLinkPrefab.startPoint.z);
                Vector3 vec4Pos = new Vector3(navMeshLinkPos.x, navMeshLinkPos.y, navMeshLinkPos.z + _navMeshLinkPrefab.endPoint.z);
                Vector3 vec3 = new Vector3(vec3Pos.x, GetTerrainYPos(vec3Pos), vec3Pos.z);
                Vector3 vec4 = new Vector3(vec4Pos.x, GetTerrainYPos(vec4Pos), vec4Pos.z);
                float angleX = Angle(vec3, vec4, Vector3.Distance(_navMeshLinkPrefab.startPoint, _navMeshLinkPrefab.endPoint));
                if (vec4.y > vec3.y) angleX *= -1f;

                var navMeshLinkActivator = Instantiate(_navMeshLinkActivatorPrefab, parent.transform);
                navMeshLinkActivator.transform.position = navMeshLinkPos;
                navMeshLinkActivator.SetAngles(angleX, angleZ);
            } // for i
        } // CreateNavMeshLinks()
#endif

        /// <summary>
        ///  Angle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        float Angle(Vector3 a, Vector3 b, float dist)
        {
            var yDiff = a.y - b.y;
            Vector3 u = new Vector3(0f, 0f, 1f);
            Vector3 v = new Vector3(0f, yDiff, dist);

            return Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(u, v) / (u.magnitude * v.magnitude));
        } // Angle()

        /// <summary>
        /// Registers a SpawnArea so it can be referenced by its Guid and recognized as a suppression area if necessary
        /// </summary>
        /// <param name="spawnArea"></param>
        public void RegisterSpawnArea(SpawnArea spawnArea)
        {
            // Add to registered suppression areas if necessary
            if (spawnArea.SuppressBiomeSpawns)
            {
                if (_suppressionAreas.Contains(spawnArea)) return;

                lock (_suppressionAreas)
                {
                    _suppressionAreas.Add(spawnArea);
                } // lock
            } // if

            // If the area does not have a Guid, generate one now
            if (string.IsNullOrEmpty(spawnArea.GetGuid()))
            {
                spawnArea.GenerateGuid();
            } // if

            // Add to dictionary with Guid as key
            if (_spawnAreas.ContainsKey(spawnArea.GetGuid())) return;

            lock (_spawnAreas)
            {
                _spawnAreas[spawnArea.GetGuid()] = spawnArea;
            } // lock
        } // RegisterSpawnArea()

        /// <summary>
        ///  UnregisterSpawnArea
        /// </summary>
        /// <param name="spawnArea"></param>
        public void UnregisterSpawnArea(SpawnArea spawnArea)
        {
            // Remove from registered suppression areas if necessary
            if (spawnArea.SuppressBiomeSpawns)
            {
                if (!_suppressionAreas.Contains(spawnArea)) return;

                lock (_suppressionAreas)
                {
                    _suppressionAreas.Remove(spawnArea);
                } // lock
            } // if

            // Remove from dictionary with Guid as key
            if (!_spawnAreas.ContainsKey(spawnArea.GetGuid())) return;

            lock (_spawnAreas)
            {
                _spawnAreas.Remove(spawnArea.GetGuid());
            } // lock
        } // UnregisterSpawnArea()

        /// <summary>
        /// Registers a WaveSpawner so it can be referenced by its Guid
        /// </summary>
        /// <param name="waveSpawner"></param>
        public void RegisterWaveSpawner(WaveSpawner waveSpawner)
        {
            // If the spawner does not have a Guid, generate one now
            if (string.IsNullOrEmpty(waveSpawner.GetGuid()))
            {
                waveSpawner.GenerateGuid();
            } // if

            // Add to dictionary with Guid as key
            if (_waveSpawners.ContainsKey(waveSpawner.GetGuid())) return;

            lock (_waveSpawners)
            {
                _waveSpawners[waveSpawner.GetGuid()] = waveSpawner;
            } // lock
        } // RegisterWaveSpawner()

        /// <summary>
        ///  UnregisterWaveSpawner
        /// </summary>
        /// <param name="waveSpawner"></param>
        public void UnregisterWaveSpawner(WaveSpawner waveSpawner)
        {
            // Remove from dictionary with Guid as key
            if (!_waveSpawners.ContainsKey(waveSpawner.GetGuid())) return;

            lock (_waveSpawners)
            {
                _waveSpawners.Remove(waveSpawner.GetGuid());
            } // lock
        } // UnregisterSpawnArea()

        /// <summary>
        /// Update all registered spawn areas with the amount of time elapsed. This is called when game time is updated from another scene.
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void UpdateSpawnAreas(float elapsedTime)
        {
            foreach (var spawnArea in _spawnAreas)
            {
                spawnArea.Value.SetSecondsElapsed(elapsedTime);
            } // foreach

            // Also do the same for WaveSpawners
            foreach (var waveSpawner in _waveSpawners)
            {
                waveSpawner.Value.SetSecondsElapsed(elapsedTime);
            } // foreach
        } // UpdateSpawnAreas()

        /// <summary>
        /// Return the SpawnArea with the specified Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public SpawnArea GetSpawnAreaByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            if (!_spawnAreas.ContainsKey(guid)) return null;

            return _spawnAreas[guid];
        } // GetSpawnAreaByGuid()

        /// <summary>
        /// Return the WaveSpawner with the specified Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public WaveSpawner GetWaveSpawnerByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            if (!_waveSpawners.ContainsKey(guid)) return null;

            return _waveSpawners[guid];
        } // GetWaveSpawnerByGuid()

        /// <summary>
        /// Returns true if a spawn position is within a suppression area.
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        public bool IsInSuppressionArea(Vector3 spawnPos)
        {
            lock (_suppressionAreas)
            {
                foreach (var area in _suppressionAreas)
                {
                    var origin = area.transform.position;
                    var xmin = origin.x - area.SuppressionRadius;
                    var xmax = origin.x + area.SuppressionRadius;
                    var zmin = origin.z - area.SuppressionRadius;
                    var zmax = origin.z + area.SuppressionRadius;

                    if (spawnPos.x >= xmin && spawnPos.x <= xmax &&
                        spawnPos.z >= zmin && spawnPos.z <= zmax)
                    {
                        return true;
                    } // if
                } // foreach
            } // lock

            return false;
        } // IsInSuppressionArea()

        /// <summary>
        /// FetchNavMeshLink
        /// </summary>
        /// <returns></returns>
        public Transform FetchNavMeshLink()
        {
            return _navMeshLinkPool.Fetch();
        } // FetchNavMeshLink()

        /// <summary>
        /// ReleaseNavMeshLink
        /// </summary>
        /// <param name="navMeshLink"></param>
        public void ReleaseNavMeshLink(Transform navMeshLink)
        {
            _navMeshLinkPool.Release(navMeshLink);
        } // ReleaseNavMeshLink()

        /// <summary>
        /// Get the total number of spawns currently in the scene for the specified category
        /// </summary>
        /// <param name="spawnCategory"></param>
        /// <returns></returns>
        public int GetTotalSpawns(string spawnCategory)
        {
            if (string.IsNullOrEmpty(spawnCategory))
            {
                // Return the total of all spawns for all categories
                return _totalSpawns.Sum(e => e.Value);
            } // if

            if (_totalSpawns.ContainsKey(spawnCategory))
            {
                return _totalSpawns[spawnCategory];
            }
            else
            {
                return 0;
            }
        } // GetTotalSpawns()

        /// <summary>
        /// Returns whether or not an object is allowed to spawn for the specified category based on total spawns in existence and max spawns for the category
        /// </summary>
        /// <param name="spawnCategory"></param>
        /// <returns></returns>
        public bool CanSpawn(string spawnCategory)
        {
            var item = Database.spawnCategories.Find(e => e.categoryName == spawnCategory);
            if (item == null) return false;

            if (GetTotalSpawns(spawnCategory) >= item.maxSpawns)
            {
                Debug.Log("Cannot spawn item in category " + spawnCategory + ", total = " + GetTotalSpawns(spawnCategory) + ", max = " + item.maxSpawns);
            }
            return GetTotalSpawns(spawnCategory) < item.maxSpawns;
        } // CanSpawn()

        /// <summary>
        /// Increment the total spawns for the specified category
        /// </summary>
        /// <param name="spawnCategory"></param>
        public void IncTotalSpawns(string spawnCategory)
        {
            if (_totalSpawns.ContainsKey(spawnCategory))
            {
                ++_totalSpawns[spawnCategory];
            }
            else
            {
                _totalSpawns[spawnCategory] = 1;
            }
        } // IncTotalSpawns()

        /// <summary>
        /// Decrement the total spawns for the specified category
        /// </summary>
        /// <param name="spawnCategory"></param>
        public void DecTotalSpawns(string spawnCategory)
        {
            if (_totalSpawns.ContainsKey(spawnCategory))
            {
                --_totalSpawns[spawnCategory];
            }
            else
            {
                // this shouldn't happen
                _totalSpawns[spawnCategory] = 0;
            }
        } // IncTotalSpawns()

        #region Game Time Integration

        /// <summary>
        /// SyncTimeOfDay
        /// Update coroutine to fetch time of day from Game Time and query only the valid spawns for those time periods. Note that several time of day definitions
        /// could be used if their time ranges overlap
        /// </summary>
        /// <returns></returns>
        IEnumerator SyncTimeOfDay()
        {
            // Get the time of day names that are valid for current time of day every so many seconds
            while (true && GameTime != null)
            {
                DoSyncTimeOfDay();

                yield return new WaitForSeconds(_timeSyncFrequency);
            } // while
        } // SyncTimeOfDay()

        /// <summary>
        /// Force SoulLink to sync time of day immediately
        /// </summary>
        public void DoSyncTimeOfDay()
        {
            if (GameTime == null) return;

            CurrentTime = GameTime.GetTime();

            if (_SoulLinkSpawnerDatabase != null)
            {
                // Check currentTime against database time of day ranges and add to a list for querying the biome spawn data later
                var query =
                    from todData in _SoulLinkSpawnerDatabase.timeOfDayData
                    where IsCurrentTimeOfDay(todData)
                    select todData;

                _currentTOD.Clear();
                foreach (var tod in query)
                {
                    _currentTOD.Add(tod.timeOfDayName);
                } // foreach

                if (OnTimeSync != null)
                {
                    OnTimeSync.Invoke();
                } // if
            } // if
        } // DoSyncTimeOfDay()

        /// <summary>
        /// IsCurrentTimeofDay
        /// </summary>
        /// <param name="todData"></param>
        /// <returns></returns>
        public bool IsCurrentTimeOfDay(TimeOfDayData todData)
        {
            // Regular comparison 
            if (todData.startTime < todData.endTime)
            {
                return CurrentTime >= todData.startTime && CurrentTime <= todData.endTime;
            } // if

            // Special case where start time is greater than end time
            return (CurrentTime >= todData.startTime || CurrentTime <= todData.endTime);
        } // IsCurrentTimeOfDay()

        /// <summary>
        /// IsInCurrentTimeOfDay
        /// </summary>
        /// <param name="todKey"></param>
        /// <returns></returns>
        public bool IsInCurrentTimeOfDay(string todKey)
        {
            return (todKey == null) || _currentTOD.Contains(todKey);
        } // IsInCurrentTimeOfDay()

        /// <summary>
        /// Check if the quests are in the correct state to allow spawning
        /// </summary>
        /// <param name="questRules"></param>
        /// <returns></returns>
        public bool IsQuestStatusValid(List<QuestRuleData> questRules)
        {
            if (_questManager != null)
            {
                foreach (var questFilter in questRules)
                {
                    if (_questManager.IsQuestActive(questFilter.questName) && questFilter.questRule != FilterRule.Include ||
                        (!_questManager.IsQuestActive(questFilter.questName) && questFilter.questRule != FilterRule.Exclude))
                    {
                        return false;
                    } // if
                } // foreach
            } // if

            return true;
        } // IsQuestStatusValid()

        /// <summary>
        /// Check if the current season is within the parameters of the provided season rules
        /// </summary>
        /// <returns></returns>
        public bool IsWithinSeasonParameters(List<SeasonRuleData> seasonRules)
        {
            foreach (var seasonRule in seasonRules)
            {
                if (seasonRule.seasonRule == FilterRule.Include)
                {
                    if (GameTime.GetSeason() != seasonRule.season) return false;
                }
                else
                {
                    if (GameTime.GetSeason() == seasonRule.season) return false;
                }
            } // foreach

            return true;
        } // IsWithinSeasonParameters()

        /// <summary>
        /// RandomTimeOfDay
        /// </summary>
        /// <returns></returns>
        public string RandomCurrentTimeOfDay()
        {
            if (_currentTOD.Count == 0) return "";
            return _currentTOD[UnityEngine.Random.Range(0, _currentTOD.Count)];
        } // RandomCurrentTimeOfDay()

#endregion

#region Weather System integration

        /// <summary>
        /// Update coroutine to fetch weather conditions from weather system.
        /// </summary>
        /// <returns></returns>
        IEnumerator SyncWeatherSystem()
        {
            while (true && _weather != null)
            {
                // TODO: Compare new values against previous values in order to call special event handlers based on weather event changes

                // Store weather values locally for use in spawning validation
                CurrentSnowStrength = _weather.GetSnowStrength();
                CurrentWetness = _weather.GetWetness();
                CurrentTemperature = _weather.GetTemperature(false);

                if (OnWeatherSync != null)
                {
                    OnWeatherSync.Invoke();
                } // if

                yield return new WaitForSeconds(_weatherSyncFrequency);
            } // while
        } // SyncWeatherSystem()

        /// <summary>
        /// IsInCurrentWeatherConditions
        /// </summary>
        /// <param name="weatherRules"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        public bool IsInCurrentWeatherConditions(List<WeatherRuleData> weatherRules, float yPos)
        {
            bool isValid = true;
            foreach (var weatherRule in weatherRules)
            {
                switch (weatherRule.weatherCondition)
                {
                    case WeatherCondition.Temperature:
                        float modifiedTemp = _weather.ElevationAffectsTemperature() ? _weather.GetElevationModifiedTemperature(yPos) : CurrentTemperature;
                        isValid = (modifiedTemp >= weatherRule.minTemperature && modifiedTemp <= weatherRule.maxTemperature);
                        break;
                    case WeatherCondition.Rain:
                        if (weatherRule.weatherRule == FilterRule.Include)
                        {
                            isValid = (CurrentWetness >= weatherRule.threshold);
                        }
                        else
                        {
                            isValid = (CurrentWetness < weatherRule.threshold);
                        }
                        break;
                    case WeatherCondition.Snow:
                        if (weatherRule.weatherRule == FilterRule.Include)
                        {
                            isValid = (CurrentSnowStrength >= weatherRule.threshold);
                        }
                        else
                        {
                            isValid = (CurrentSnowStrength < weatherRule.threshold);
                        }
                        break;
                } // switch

                // Don't continue if there are any failures
                if (!isValid) break;
            } // foreach

            return isValid;
        } // IsInCurrentWeatherConditions()
#endregion

        /// <summary>
        /// GenerateSpawns
        /// </summary>
        /// <returns></returns>
        IEnumerator GenerateSpawns()
        {
            yield return new WaitForSeconds(_delaySpawnsOnStart);

            float spawnRange = _spawnRadius - _minimumSpawnRange;
            float incrementalRange = (spawnRange / _spawnCircles);
            int iter = 0; // which iteration are we on

            while (!Paused && true && Database != null)
            {
                // Skip if Biomes spawning is disabled or there is no player transform reference
                if (_disableBiomeSpawns || _playerTransform == null)
                {
                    yield return null;
                    continue;
                } // if

                yield return new WaitForSeconds(1f / _spawnRate);

                var totalSpawns = TotalSpawns;
                if (totalSpawns < _maxSpawns) // Checks against total spawns for all categories
                {
                    // Pick a random spawn position
                    var minRange = _useSpawnCircles ? (_minimumSpawnRange + (iter * incrementalRange)) : _minimumSpawnRange;
                    var maxRange = _useSpawnCircles ? (minRange + incrementalRange) : _spawnRadius;

                    var spawnPos = GetSpawnPos(minRange, maxRange);
                    ++iter;

                    if (iter == _spawnCircles)
                    {
                        iter = 0;
                    } // if

                    // First reduce the possible spawns by querying only biome spawn data within current time of day definitions and
                    // the biomes that meet the threshold requirements. Also checks for whether it has been disabled.
                    var query =
                        from biomeData in Database.biomeSpawnData
                        where _currentBiomes.Contains(biomeData.biomeName) && !biomeData.disabled && biomeData.timeOfDaySpawns.Any(i => _currentTOD.Contains(i.Key))
                        select biomeData;

                    // If no time of day definitions match the current time of day then log it so that the user can debug the issue
                    if (_currentTOD.Count == 0)
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "No Time of Day Definitions qualify for spawns. Nothing will spawn.");
                    } // if

                    bool viableSpawn = false;

                    foreach (var item in query)
                    {
                        // Pick a random entry from available time of days
                        var tod = RandomCurrentTimeOfDay();

                        if (item.timeOfDaySpawns.ContainsKey(tod))
                        {
                            if (item.timeOfDaySpawns[tod].spawnInfoData.Count == 0) continue;

                            // Pick a random entry from available spawns
                            var index = UnityEngine.Random.Range(0, item.timeOfDaySpawns[tod].spawnInfoData.Count);
                            var spawnInfoData = item.timeOfDaySpawns[tod].spawnInfoData[index];
                            spawnInfoData.randomChance = 0;
                            spawnInfoData.herdID = 0;

                            // Set a flag indicating that there was at least one valid spawn that could be chosen from the current query
                            // if by the end of the foreach loop, this flag was not set then log something to inform the user
                            viableSpawn = true;

                            // Check for a custom spawn validator and call its PreValidateSpawn method if it exists. This is used for rejecting a spawn as early as
                            // possible. Returning true will have no affect because further filters/rules can still reject it later.
                            bool canSpawn = true;
                            if (_spawnValidator != null)
                            {
                                canSpawn = _spawnValidator.PreValidateSpawn(spawnInfoData);
                            } // if

                            if (!canSpawn) continue;

                            // Check if there are any spawns of this type within a certain range. If so then skip it
                            float proximityRange = ((_spawnRadius * _clusterRange) / spawnInfoData.populationCap);
                            if (PoolManager.AnySpawnInRange(spawnInfoData.prefab, spawnPos, proximityRange)) continue;

                            uint chance = 0;
                            canSpawn = (spawnInfoData.probability > 0 && RandomValues.FindValue((uint)spawnInfoData.probability, out chance) && 
                                (CanSpawn(spawnInfoData.spawnCategory) || spawnInfoData.ignoreTotalSpawns));
                            if (!canSpawn)
                            {
                                canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.Probability) : canSpawn);
                            } // if

                            if (!canSpawn)
                            {
                                FailSpawn(spawnInfoData, "random chance exceeded probability.");
                                continue;
                            } // if

                            // Check if a herd can spawn at least the minimum qty before allowing it
                            if (spawnInfoData.spawnType == SpawnType.Herd && (totalSpawns + spawnInfoData.herd.minPopulation) > MaxSpawns)
                            {
                                FailSpawn(spawnInfoData, "spawning the herd would exceed max spawns");
                                continue;
                            } // if

                            if (canSpawn)
                            {
                                // TODO: Solve the issue of herds spawning too close to other herds of the same type. Possibly use HerdData to insure
                                //  that the herd object is up to date by re-calculating a sphere volume as its members move about
                                if (spawnInfoData.spawnType == SpawnType.Herd)
                                {
                                    uint herdID = AddHerd(chance);

                                    // Spawn a randomized qty
                                    int qtyDesired = UnityEngine.Random.Range(spawnInfoData.herd.minPopulation, spawnInfoData.herd.maxPopulation);
                                    int qty = 0;

                                    for (int i = 0; i < qtyDesired; ++i)
                                    {
                                        float herdMinElevation = spawnInfoData.overrideElevation ? spawnInfoData.minElevation : MinElevation;
                                        float herdMaxElevation = spawnInfoData.overrideElevation ? spawnInfoData.maxElevation : MaxElevation;
                                        float herdMinSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.minSlopeAngle : MinSlopeAngle;
                                        float herdMaxSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.maxSlopeAngle : MaxSlopeAngle;

                                        foreach (var spawn in spawnInfoData.herd.herdPrefabs)
                                        {
                                            // Randomize position in radius around spawnPos
                                            Vector3 newPos = Utility.GetRandomPos(0f, spawnInfoData.herd.spawnRadius, spawnPos, _spawnMode, _spawnAxes);
                                            Vector3 herdSpawnPos = new Vector3(newPos.x, GetYPos(newPos), newPos.z);

                                            float minElevation = spawn.overrideElevation ? spawn.minElevation : herdMinElevation;
                                            float maxElevation = spawn.overrideElevation ? spawn.maxElevation : herdMaxElevation;
                                            float minSlopeAngle = spawn.overrideSlopeAngle ? spawn.minSlopeAngle : herdMinSlopeAngle;
                                            float maxSlopeAngle = spawn.overrideSlopeAngle ? spawn.maxSlopeAngle : herdMaxSlopeAngle;

                                            // Assign the herd's filters to the individual spawns to validate them properly
                                            spawn.textureRules = spawnInfoData.textureRules;
                                            spawn.weatherRules = spawnInfoData.weatherRules;
                                            spawn.seasonRules = spawnInfoData.seasonRules;
                                            spawn.questRules = spawnInfoData.questRules;
                                            spawn.proximityRules = spawnInfoData.proximityRules;
                                            spawn.spawnAreaGuid = string.Empty;
                                            spawn.waveSpawnerGuid = string.Empty;
                                            spawn.waveItemGuid = string.Empty;

                                            uint herdSpawnChance = (uint)UnityEngine.Random.Range(0, 100);
                                            if (spawn.probability > 0 && herdSpawnChance < (uint)spawn.probability)
                                            {
                                                spawn.randomChance = herdSpawnChance;
                                                spawn.herdID = herdID;

                                                if (ValidateSpawnRules(spawn, herdSpawnPos, tod, spawn.ignorePlayerFOV, minElevation, maxElevation, minSlopeAngle, maxSlopeAngle))
                                                {
                                                    qty++;
                                                } // if
                                            } // if

                                            if (qty == qtyDesired) break;
                                        } // foreach

                                        if (qty == qtyDesired) break;

                                        yield return null;
                                    } // for i

                                    // If we spawned any instances from the herd, generate proximity spawns as necessary
                                    if (qty > 0 && Database != null)
                                    {
                                        GenerateProximitySpawns(Database.GetProximitySpawns(spawnInfoData.spawnGuid), tod, spawnPos);
                                    } // if
                                }
                                else
                                {
                                    if (spawnInfoData.spawnType == SpawnType.Variants && spawnInfoData.prefabVariants.Count > 0)
                                    {
                                        spawnInfoData.prefab = spawnInfoData.prefabVariants[UnityEngine.Random.Range(0, spawnInfoData.prefabVariants.Count)];
                                    } // if

                                    float minElevation = spawnInfoData.overrideElevation ? spawnInfoData.minElevation : MinElevation;
                                    float maxElevation = spawnInfoData.overrideElevation ? spawnInfoData.maxElevation : MaxElevation;
                                    float minSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.minSlopeAngle : MinSlopeAngle;
                                    float maxSlopeAngle = spawnInfoData.overrideSlopeAngle ? spawnInfoData.maxSlopeAngle : MaxSlopeAngle;

                                    spawnInfoData.spawnAreaGuid = string.Empty;
                                    spawnInfoData.waveSpawnerGuid = string.Empty;
                                    spawnInfoData.waveItemGuid = string.Empty;
                                    spawnInfoData.randomChance = chance;
                                    ValidateSpawnRules(spawnInfoData, spawnPos, tod, spawnInfoData.ignorePlayerFOV, minElevation, maxElevation, minSlopeAngle, maxSlopeAngle);
                                }
                            } // if
                        } // if tod in set
                        else
                        {
                            SoulLinkGlobal.DebugLog(gameObject, "No spawns for the selected time of day. Selected Time of Day = " + tod);
                        }
                    } // foreach item

                    if (!viableSpawn)
                    {
                        SoulLinkGlobal.DebugLog(gameObject, "No spawns for the available biomes for the proposed spawn position.");
                    } // if
                } // if
            } // while
        } // GenerateSpawns()

        /// <summary>
        /// Call this method to report spawn failure and return the random chance value to the pool
        /// </summary>
        /// <param name="spawnInfoData"></param>
        /// <param name="failureText"></param>
        public void FailSpawn(SpawnInfoData spawnInfoData, string failureText)
        {
            var spawnName = (spawnInfoData.prefab != null) ? spawnInfoData.prefab.name : "<null>";
            SoulLinkGlobal.DebugLog(gameObject, "Failed to spawn " + spawnName + " " + failureText);
            RandomValues.ReturnValue(spawnInfoData.randomChance);
        } // FailSpawn()

        /// <summary>
        /// Call this method to report spawn failure and return the random chance value to the pool
        /// </summary>
        /// <param name="spawnInfoData"></param>
        /// <param name="failureText"></param>
        public void FailSpawn(SpawnData spawnData, string failureText)
        {
            var spawnName = (spawnData.spawnPrefab != null) ? spawnData.spawnPrefab.name : "<null>";
            SoulLinkGlobal.DebugLog(gameObject, "Failed to spawn " + spawnName + " " + failureText);
            RandomValues.ReturnValue(spawnData.randomChance);
        } // FailSpawn()

        /// <summary>
        /// Perform various checks against spawning rules to ensure that the proposed spawn position is valid.
        /// </summary>
        /// <param name="spawnInfoData"></param>
        /// <param name="spawnPos"></param>
        /// <param name="tod"></param>
        /// <param name="ignorePlayerFOV"></param>
        /// <param name="minElevation"></param>
        /// <param name="maxElevation"></param>
        /// <param name="minSlopeAngle"></param>
        /// <param name="maxSlopeAngle"></param>
        /// <param name="isAreaSpawn"></param>
        /// <returns></returns>
        public bool ValidateSpawnRules(SpawnInfoData spawnInfoData, Vector3 spawnPos, string tod, bool ignorePlayerFOV, float minElevation, float maxElevation,
            float minSlopeAngle, float maxSlopeAngle, bool isAreaSpawn = false)
        {
            if (spawnInfoData.prefab == null || spawnInfoData.disable) return false;

            bool canSpawn = true;

            // Validate against quest filters first for the earliest possible out
            if (_questManager != null)
            {
                foreach (var questFilter in spawnInfoData.questRules)
                {
                    canSpawn = (_questManager.IsQuestActive(questFilter.questName) && questFilter.questRule == FilterRule.Include) ||
                               (!_questManager.IsQuestActive(questFilter.questName) && questFilter.questRule == FilterRule.Exclude);

                    if (!canSpawn)
                    {
                        canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.QuestFilter) : canSpawn);
                    } // if
                } // foreach
            } // if

            if (!canSpawn)
            {
                FailSpawn(spawnInfoData, "quest rule violation.");
                return false;
            }

            // Check against current population for this spawn. Do not spawn if cap has already been reached
            if (_populations.ContainsKey(spawnInfoData.prefab))
            {
                canSpawn = (_populations[spawnInfoData.prefab] < spawnInfoData.populationCap);
            } // if

            if (!canSpawn)
            {
                canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.PopulationCap) : canSpawn);
                if (!canSpawn)
                {
                    FailSpawn(spawnInfoData, "population exceeded.");
                    return canSpawn;
                } // if
            } // if

            // Check against elevation 
            canSpawn = (_spawnMode == SpawnMode.SpawnIn2D) || (spawnPos.y >= minElevation && spawnPos.y <= maxElevation);

            if (!canSpawn)
            {
                canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.Elevation) : canSpawn);
                if (!canSpawn)
                {
                    FailSpawn(spawnInfoData, "elevation out of range.");
                    return canSpawn;
                }
            } // if

            // Check against slope angle
            float slopeAngle = Utility.GetSlopeAngle(spawnPos, _navMeshIgnoreLayers, _sphereCastRadius, _sphereCastDistance);
            canSpawn = (_spawnMode == SpawnMode.SpawnIn2D) || (slopeAngle >= minSlopeAngle && slopeAngle <= maxSlopeAngle);

            if (!canSpawn)
            {
                canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.SlopeAngle) : canSpawn);
                if (!canSpawn)
                {
                    FailSpawn(spawnInfoData, "on slope angle = " + slopeAngle + ", spawnPos = " + spawnPos);
                    return false;
                } // if
            } // if

#if SOULLINK_USE_GLOBALSNOW
            // Check aginst Global Snow filter
            foreach (var globalSnowFilter in spawnInfoData.globalSnowRules)
            {
                float snowAmount = _globalSnow.GetSnowAmountAt(spawnPos);
                canSpawn = (globalSnowFilter.filterRule == FilterRule.Exclude && snowAmount < globalSnowFilter.threshold) || 
                    (globalSnowFilter.filterRule == FilterRule.Include && snowAmount >= globalSnowFilter.threshold);
                if (!canSpawn)
                {
                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.GlobalSnowFilter) : canSpawn);
                    if (!canSpawn)
                    {
                        FailSpawn(spawnInfoData, "Global Snow Amount of " + snowAmount);
                        return false;
                    } // if
                } // if
            } // foreach
#endif

            // Check against proximity of other AI
            foreach (var proximityFilter in spawnInfoData.proximityRules)
            {
                if (proximityFilter.spawnPrefab == null) continue;

                bool spawnInRange = PoolManager.AnySpawnInRange(proximityFilter.spawnPrefab, spawnPos, proximityFilter.range);
                canSpawn = (proximityFilter.proximityRule == FilterRule.Exclude && !spawnInRange) || (proximityFilter.proximityRule == FilterRule.Include && spawnInRange);
                if (!canSpawn)
                {
                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.ProximityFilter) : canSpawn);
                    if (!canSpawn)
                    {
                        FailSpawn(spawnInfoData, "proximity of " + proximityFilter.spawnPrefab.name);
                        return false;
                    } // if
                } // if
            } // foreach

            // Check against areas that suppress biome spawns for spawns not generated from a SpawnArea
            if (!isAreaSpawn)
            {
                canSpawn = !IsInSuppressionArea(spawnPos);

                if (!canSpawn)
                {
                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.SuppressionArea) : canSpawn);
                    if (!canSpawn)
                    {
                        FailSpawn(spawnInfoData, "in suppression area.");
                        return false;
                    } // if
                } // if
            } // if

            // Check against texture rules if not already excluded for other reasons
            if (canSpawn && spawnInfoData.textureRules.Count > 0)
            {
                float[,,] aMap = null;

                if (_usePolaris)
                {
#if GRIFFIN_2021
                    // Set the target terrain
                    var targetTerrain = Utility.PolarisTerrainContaining(_terrains, spawnPos);
                    if (targetTerrain != null)
                    {
                        aMap = Utility.CheckPolarisTextures(targetTerrain, spawnPos);
                    } // if
#endif
                }
                else
                {

                    _targetTerrain = Utility.TerrainContaining(spawnPos, _useMapMagic);
                    if (_targetTerrain != null)
                    {
                        aMap = Utility.CheckTextures(_targetTerrain, spawnPos);
                    } // if
                }

                // Fail spawn
                if (aMap == null)
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Error: Texture filters defined for a spawn, but there is no terrain with texture layers.");
                    return false;
                }

                // Query for just texture rules of type 'include'
                var texQuery =
                    from texData in spawnInfoData.textureRules
                    where spawnInfoData.textureRules.Any(e => e.textureRule == FilterRule.Include)
                    select texData;

                // Verify that the aMap has all the required texture indices
                foreach (var tex in texQuery)
                {
                    // if any one item fails then cannot spawn here
                    if (aMap[0, 0, tex.textureIndex] == 0f)
                    {
                        canSpawn = false;
                        break;
                    } // if
                } // foreach

                for (int i = 0; i < aMap.Length; ++i)
                {
                    var influence = aMap[0, 0, i];

                    // Check against each texture filter to see if it meets the threshhold
                    foreach (var texRule in spawnInfoData.textureRules)
                    {
                        // If there is a texture rule for this texture index then let's verify it meets the requirements
                        if (texRule.textureIndex == i)
                        {
                            if (texRule.textureRule == FilterRule.Include)
                            {
                                canSpawn = (texRule.threshold >= influence);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.TextureFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        FailSpawn(spawnInfoData, "failed on exclude texture = " + texRule.textureIndex + " with threshold of " + texRule.threshold + ", influence = " + influence);
                                        return canSpawn;
                                    } // if
                                }
                            }
                            else // Exclude rule
                            {
                                canSpawn = (influence >= texRule.threshold);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.TextureFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        FailSpawn(spawnInfoData, "failed on include texture = " + texRule.textureIndex + " with threshold of " + texRule.threshold + ", influence = " + influence);
                                        return canSpawn;
                                    } // if
                                }
                            }
                        } // if
                    } // foreach
                } // for i
            } // if (texture rules)

            // Check against season rules
            if (canSpawn && spawnInfoData.seasonRules.Count > 0)
            {
                foreach (var seasonRule in spawnInfoData.seasonRules)
                {
                    if (seasonRule.seasonRule == FilterRule.Include)
                    {
                        canSpawn = (GameTime.GetSeason() == seasonRule.season);
                        if (!canSpawn)
                        {
                            canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.SeasonFilter) : canSpawn);
                            if (!canSpawn)
                            {
                                FailSpawn(spawnInfoData, "failed on include season = " + seasonRule.season.ToString());
                                return canSpawn;
                            } // if
                        } // if
                    }
                    else
                    {
                        canSpawn = (GameTime.GetSeason() != seasonRule.season);
                        if (!canSpawn)
                        {
                            canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.SeasonFilter) : canSpawn);
                            if (!canSpawn)
                            {
                                FailSpawn(spawnInfoData, "failed on exclude season = " + seasonRule.season.ToString());
                                return canSpawn;
                            } // if
                        } // if
                    }
                } // foreach
            } // if

            // Check against weather rules if not already excluded for other reasons
            if (canSpawn && _weather != null && spawnInfoData.weatherRules.Count > 0)
            {
                foreach (var weatherRule in spawnInfoData.weatherRules)
                {
                    switch (weatherRule.weatherCondition)
                    {
                        case WeatherCondition.Temperature:
                            float modifiedTemp = _weather.ElevationAffectsTemperature() ? _weather.GetElevationModifiedTemperature(spawnPos.y) : CurrentTemperature;
                            canSpawn = (modifiedTemp >= weatherRule.minTemperature && modifiedTemp <= weatherRule.maxTemperature);
                            if (!canSpawn)
                            {
                                canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.WeatherFilter) : canSpawn);
                                if (!canSpawn)
                                {
                                    FailSpawn(spawnInfoData, "failed on temperature = " + modifiedTemp + " with min/max temp of " + weatherRule.minTemperature + "/" + weatherRule.maxTemperature);
                                    return canSpawn;
                                } // if
                            } // if
                            break;
                        case WeatherCondition.Rain:
                            if (weatherRule.weatherRule == FilterRule.Include)
                            {
                                canSpawn = (CurrentWetness >= weatherRule.threshold);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.WeatherFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        FailSpawn(spawnInfoData, "failed on include weather condition = " + weatherRule.weatherCondition.ToString() + " with threshold of " + weatherRule.threshold + ", influence = " + CurrentWetness);
                                        return canSpawn;
                                    }
                                }
                            }
                            else
                            {
                                canSpawn = (CurrentWetness < weatherRule.threshold);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.WeatherFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        if (!canSpawn)
                                        {
                                            FailSpawn(spawnInfoData, "failed on exclude weather condition = " + weatherRule.weatherCondition.ToString() + " with threshold of " + weatherRule.threshold + ", influence = " + CurrentWetness);
                                            return canSpawn;
                                        } // if
                                    } // if
                                } // if
                            }
                            break;
                        case WeatherCondition.Snow:
                            if (weatherRule.weatherRule == FilterRule.Include)
                            {
                                canSpawn = (CurrentSnowStrength >= weatherRule.threshold);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.WeatherFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        if (!canSpawn)
                                        {
                                            FailSpawn(spawnInfoData, "failed on include weather condition = " + weatherRule.weatherCondition.ToString() + " with threshold of " + weatherRule.threshold + ", influence = " + CurrentSnowStrength);
                                            return canSpawn;
                                        } // if
                                    } // if
                                } // if
                            }
                            else
                            {
                                canSpawn = (CurrentSnowStrength < weatherRule.threshold);
                                if (!canSpawn)
                                {
                                    canSpawn = (_spawnValidator != null ? _spawnValidator.PostValidateSpawn(spawnInfoData, SpawnValidationFailure.WeatherFilter) : canSpawn);
                                    if (!canSpawn)
                                    {
                                        if (!canSpawn)
                                        {
                                            FailSpawn(spawnInfoData, "failed on exclude weather condition = " + weatherRule.weatherCondition.ToString() + " with threshold of " + weatherRule.threshold + ", influence = " + CurrentSnowStrength);
                                            return canSpawn;
                                        } // if
                                    } // if
                                } // if
                            }
                            break;
                    } // switch
                } // foreach
            } // if

            if (canSpawn)
            {
                return SpawnIt(spawnInfoData, spawnPos, tod, ignorePlayerFOV);
            } // if canSpawn

            return canSpawn;
        } // ValidateSpawnRules()

        /// <summary>
        /// SpawnIt
        /// </summary>
        /// <param name="spawnInfoData"></param>
        /// <param name="spawnPos"></param>
        /// <param name="tod"></param>
        /// <param name="ignorePlayerFOV"></param>
        public bool SpawnIt(SpawnInfoData spawnInfoData, Vector3 spawnPos, string tod, bool ignorePlayerFOV)
        {
            if (_generateNavMesh)
            {
#if SOULLINK_USE_AINAV
                // If the Spawnable component does not need a nav mesh then just go ahead and spawn it right now
                if (spawnInfoData.prefab != null)
                {
                    var spawnable = spawnInfoData.prefab.GetComponentInChildren<ISpawnable>();
                    if (spawnable != null && !spawnable.GetRequiresNavMesh())
                    {
                        return TrySpawn(new SpawnData()
                        {
                            spawnPrefab = spawnInfoData.prefab,
                            spawnCategory = spawnInfoData.spawnCategory,
                            spawnPos = spawnPos,
                            spawnRot = spawnInfoData.spawnRot,
                            populationCap = spawnInfoData.populationCap,
                            ignoreTotalSpawns = spawnInfoData.ignoreTotalSpawns,
                            timeOfDay = tod,
                            ignorePlayerFOV = ignorePlayerFOV,
                            minScale = spawnInfoData.minScale,
                            maxScale = spawnInfoData.maxScale,
                            origScale = spawnInfoData.prefab.localScale,
                            weatherRules = spawnInfoData.weatherRules,
                            seasonRules = spawnInfoData.seasonRules,
                            questRules = spawnInfoData.questRules,
                            spawnGuid = spawnInfoData.spawnGuid,
                            spawnAreaGuid = spawnInfoData.spawnAreaGuid,
                            waveSpawnerGuid = spawnInfoData.waveSpawnerGuid,
                            waveItemGuid = spawnInfoData.waveItemGuid,
                            spawnInAir = spawnInfoData.spawnInAir,
                            maxAltitude = spawnInfoData.maxAltitude,
                            randomChance = spawnInfoData.randomChance,
                            herdID = spawnInfoData.herdID,
                            markPersistent = spawnInfoData.markPersistent,
                            ignoreRaycast = spawnInfoData.ignoreRaycast
                        }) != null; 
                    } // if
                } // if

                // Place SpawnNavChecker in spawn position so it can trigger against any local nav mesh builders
                _spawnNavChecker.gameObject.SetActive(false);
                _spawnNavChecker.transform.position = spawnPos;
                _spawnNavChecker.AddSpawn(new SpawnData()
                { spawnPrefab = spawnInfoData.prefab, spawnCategory = spawnInfoData.spawnCategory, spawnPos = spawnPos, spawnRot = spawnInfoData.spawnRot, populationCap = spawnInfoData.populationCap, ignoreTotalSpawns = spawnInfoData.ignoreTotalSpawns,
                    timeOfDay = tod, ignorePlayerFOV = ignorePlayerFOV, minScale = spawnInfoData.minScale, maxScale = spawnInfoData.maxScale, origScale = spawnInfoData.prefab.localScale,
                    weatherRules = spawnInfoData.weatherRules, seasonRules = spawnInfoData.seasonRules, questRules = spawnInfoData.questRules, spawnAreaGuid = spawnInfoData.spawnAreaGuid,
                    waveSpawnerGuid = spawnInfoData.waveSpawnerGuid, waveItemGuid = spawnInfoData.waveItemGuid,
                    spawnInAir = spawnInfoData.spawnInAir, maxAltitude = spawnInfoData.maxAltitude, spawnGuid = spawnInfoData.spawnGuid, 
                    randomChance = spawnInfoData.randomChance, herdID = spawnInfoData.herdID, markPersistent = spawnInfoData.markPersistent, ignoreRaycast = spawnInfoData.ignoreRaycast
                });
                _spawnNavChecker.gameObject.SetActive(true);
#endif
            }
            else
                {
                    return TrySpawn(new SpawnData()
                { spawnPrefab = spawnInfoData.prefab, spawnCategory = spawnInfoData.spawnCategory, spawnPos = spawnPos, spawnRot = spawnInfoData.spawnRot, populationCap = spawnInfoData.populationCap, ignoreTotalSpawns = spawnInfoData.ignoreTotalSpawns,
                    timeOfDay = tod, ignorePlayerFOV = ignorePlayerFOV, minScale = spawnInfoData.minScale, maxScale = spawnInfoData.maxScale, origScale = spawnInfoData.prefab.localScale,
                    weatherRules = spawnInfoData.weatherRules, seasonRules = spawnInfoData.seasonRules, questRules = spawnInfoData.questRules,
                    spawnAreaGuid = spawnInfoData.spawnAreaGuid, waveSpawnerGuid = spawnInfoData.waveSpawnerGuid, waveItemGuid = spawnInfoData.waveItemGuid,
                    spawnInAir = spawnInfoData.spawnInAir, minAltitude = spawnInfoData.minAltitude, maxAltitude = spawnInfoData.maxAltitude, 
                    spawnGuid = spawnInfoData.spawnGuid, randomChance = spawnInfoData.randomChance, herdID = spawnInfoData.herdID,
                    markPersistent = spawnInfoData.markPersistent, ignoreRaycast = spawnInfoData.ignoreRaycast
                    }) != null;
            }

            return false;
        } // SpawnIt()

#if SOULLINK_USE_AINAV
        /// <summary>
        /// PlaceSpawnNavChecker
        /// </summary>
        /// <param name="spawnPos"></param>
        public void PlaceSpawnNavChecker(Vector3 spawnPos)
        {
            _spawnNavChecker.transform.position = spawnPos;
            _spawnNavChecker.gameObject.SetActive(true);
        } // PlaceSpawnNavChecker()
#endif

        /// <summary>
        /// PopulationReached
        /// </summary>
        /// <param name="spawnData"></param>
        /// <returns></returns>
        public bool PopulationReached(SpawnData spawnData)
        {
            bool result = false;
            if (_populations.ContainsKey(spawnData.spawnPrefab))
            {
                result = (_populations[spawnData.spawnPrefab] == spawnData.populationCap);
            } // if

            return result;
        } // PopulationReached()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public Vector3 GetSpawnPos(float minRange, float maxRange)
        {
            Vector3 pos = Utility.GetRandomPos(minRange, maxRange, _playerTransform.position, _spawnMode, _spawnAxes);

            // Sample terrain to get proper y position for spawn
            Vector3 spawnPos = _spawnMode == SpawnMode.SpawnIn3D ?  new Vector3(pos.x, GetYPos(pos), pos.z) : pos;

            // Determine which biomes at the specific position meet the threshold requirements
            QueryBiomes(spawnPos);

            return spawnPos;
        } // GetSpawnPos()

        /// <summary>
        /// If the spawn must be out of player's FOV then put a placeholder in the spawn position, otherwise call the Spawn method to 
        /// spawn the prefab.
        /// </summary>
        /// <param name="spawnData"></param>
        /// <returns></returns>
        public Transform TrySpawn(SpawnData spawnData)
        {
            if (_spawnOutsidePlayerFOV && !spawnData.ignorePlayerFOV)
            {
                return SpawnPlaceholder(spawnData);
            }
            else
            {
                return Spawn(spawnData);
            }
        } // TrySpawn()

        /// <summary>
        /// SpawnPlaceholder
        /// </summary>
        /// <param name="spawnData"></param>
        /// <returns></returns>
        public Transform SpawnPlaceholder(SpawnData spawnData)
        {
            lock (_spawnPlaceholders)
            {
				// Fetch a spawnplaceholder, set its properties and place it where necessary    
                var placeHolder = (_spawnPlaceholders.Count > 0) ? _spawnPlaceholders.Pop() : CreatePlaceholder();
                if (placeHolder)
				{
					// Count as part of the AI's population
					PopulationAdd(spawnData.spawnPrefab);

					placeHolder.SpawnData = spawnData;
					placeHolder.transform.position = spawnData.spawnPos;
					placeHolder.transform.rotation = spawnData.spawnRot != Quaternion.identity ? spawnData.spawnRot : Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
					
                    SpawnArea spawnArea = GetSpawnAreaByGuid(spawnData.spawnAreaGuid);
                    if (spawnArea != null)
                    {
                        placeHolder.transform.SetParent(spawnArea.transform);
                    } // if
					
					placeHolder.gameObject.SetActive(true);
                    placeHolder.enabled = true;
					return placeHolder.transform;
				} // if
			} // lock
            return null;
        } // SpawnPlaceholder()

        /// <summary>
        /// Release a spawn placeholder instance back to the object pool so it can be re-used.
        /// </summary>
        /// <param name="placeHolder"></param>
        public void ReleasePlaceholder(SpawnPlaceholder placeHolder)
        {
            lock (_spawnPlaceholders)
            {
				if (!placeHolder.Spawned)
				{
					PopulationSubtract(placeHolder.SpawnData.spawnPrefab);
				} // if 

				placeHolder.ResetToDefault();
                placeHolder.transform.SetParent(gameObject.transform);
                placeHolder.enabled = false;
                placeHolder.gameObject.SetActive(false);
                _spawnPlaceholders.Push(placeHolder);
			} // lock
        } // ReleasePlaceholder()

        private Quaternion quatIdentity = Quaternion.identity;

        /// <summary>
        /// Spawn
        /// </summary>
        /// <param name="spawnData"></param>
        /// <param name="manualSpawn"></param>
        /// <returns></returns>
        public Transform Spawn(SpawnData spawnData, bool manualSpawn = false, bool randomRotation = true, bool validatePosition = true)
        {
            if (string.IsNullOrEmpty(spawnData.spawnCategory)) spawnData.spawnCategory = DEFAULT_SPAWN_CATEGORY_NAME;
            if (GetTotalSpawns("") == _maxSpawns && !spawnData.ignoreTotalSpawns) return null;
            if (!CanSpawn(spawnData.spawnCategory)) return null;

            // This may be spawned by a SpawnArea so let's check that here
            SpawnArea spawnArea = GetSpawnAreaByGuid(spawnData.spawnAreaGuid);
            if (spawnArea != null && spawnArea.SpawnMaxReached())
            {
                FailSpawn(spawnData, "spawn area spawn max reached.");
                return null;
            } // if

            // This may be spawned by a WaveSpawner so let's check that here
            WaveSpawner waveSpawner = GetWaveSpawnerByGuid(spawnData.waveSpawnerGuid);

            // Figure out if there should be a parent transform; spawned by spawn area or wave spawner?
            Transform parent = null;
            if (spawnArea != null)
            {
                parent = spawnArea.transform;
            }
            else if (waveSpawner != null)
            {
                parent = waveSpawner.transform;
            }

            // Override random rotation in 2D spawning; TODO: possibly change this for 2D axes based rotation
            if (_spawnMode == SpawnMode.SpawnIn2D) randomRotation = false;

            // Insure spawn position is on valid navmesh. If not then try to pick a new position nearby. Otherwise fail this and do not spawn.
            // Note: Remove from population if using placeholders and not manualSpawn
            bool isValid = !validatePosition;
            var spawnPos = (validatePosition ? ValidateSpawnPos(spawnData, out isValid) : spawnData.spawnPos);
            if (!isValid)
            {
                // Subtract from population if using spawn outside player FOV, not ignoring player FOV, and not a manual spawn
                if (_spawnOutsidePlayerFOV && !spawnData.ignorePlayerFOV && !manualSpawn)
                {
                    PopulationSubtract(spawnData.spawnPrefab);
                } // if

                FailSpawn(spawnData, "invalid navmesh position.");
                return null;
            } // if

            // Override randomRotation flag if the rotation has a value already
            if (spawnData.spawnRot != Quaternion.identity)
            {
                randomRotation = false;
            } // if

            Transform spawn = PoolManager.Spawn(spawnData.spawnPrefab, spawnPos, (randomRotation ? Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up) : spawnData.spawnRot), 
                parent, spawnData.minScale, spawnData.maxScale, spawnData.spawnPrefab.localScale);
            if (spawn == null)
            {
                Debug.LogError("Failed to spawn " + spawnData.spawnPrefab.name + " from PoolManager, totalSpawns = " + GetTotalSpawns("") + ", maxspawns = " + _maxSpawns);
                Debug.LogError("   current population = " + GetPopulation(spawnData.spawnPrefab) + ", population cap = " + spawnData.populationCap);

                FailSpawn(spawnData, "could not fetch instance from PoolManager.");
                return null;
            } // if

            ISpawnable spawnable = spawn.gameObject.GetComponentInChildren<ISpawnable>();
            if (spawnable == null)
            {
                FailSpawn(spawnData, "no ISpawnable interface.");
                return null;
            } // if

            spawnable.SetSpawnedBy(SpawnedBy.SoulLinkSpawner);
            spawnable.SetSpawnPrefab(spawnData.spawnPrefab);
            spawnable.SetTimeOfDaySpawned(spawnData.timeOfDay);
            spawnable.SetSpawnCategory(spawnData.spawnCategory);
            spawnable.SetWeatherRules(spawnData.weatherRules);
            spawnable.SetSeasonRules(spawnData.seasonRules);
            spawnable.SetSpawnArea(spawnData.spawnAreaGuid);
            spawnable.SetWaveSpawner(spawnData.waveSpawnerGuid, spawnData.waveItemGuid);
            spawnable.SetSpawnOutsidePlayerFOV(_spawnOutsidePlayerFOV && !spawnData.ignorePlayerFOV);
            spawnable.SetDespawnOutsidePlayerFOV(_despawnOutsidePlayerFOV && !spawnData.ignorePlayerFOV);
            spawnable.SetRandomChance(spawnData.randomChance);
            spawnable.SetHerdID(spawnData.herdID); 
            if (spawnData.markPersistent)
            {
                spawnable.SetIsPersistent(spawnData.markPersistent);
            } // if
            spawnable.SetIgnoreTotalSpawns(spawnData.ignoreTotalSpawns);

            if (spawnArea != null)
            {
                spawnArea.IncSpawnTotal();
            } // if

            if (waveSpawner != null)
            {
                waveSpawner.IncSpawnTotal(spawnData.waveItemGuid);
            } // if

            // Add to population if not using OnlySpawnOutOfSight
            if (!_spawnOutsidePlayerFOV || spawnData.ignorePlayerFOV)
            {
                PopulationAdd(spawnData.spawnPrefab);
            } // if

            // Counting towards total spawns only when it is fully spawned (i.e. visible)
            if (!spawnData.ignoreTotalSpawns) IncTotalSpawns(spawnData.spawnCategory);// ++TotalSpawns; // TODO: update proper total spawns by category

            if (_populations.ContainsKey(spawnData.spawnPrefab))
            {
                SoulLinkGlobal.DebugLog(gameObject, "   Spawning " + spawn.name + ", population = " + _populations[spawnData.spawnPrefab]);
            }
            else
            {
                SoulLinkGlobal.DebugLog(gameObject, "   Spawning " + spawn.name + ", population not yet known.");
            }

            // Check for proximity spawning
            if (Database != null)
            {
                GenerateProximitySpawns(Database.GetProximitySpawns(spawnData.spawnGuid), spawnData.timeOfDay, spawnPos);
            } // if

            AddToHerd(spawnable, spawnData.herdID);
            return spawn;
        } // Spawn()

        /// <summary>
        /// Generate proximity spawns if they meet all criteria
        /// </summary>
        /// <param name="proximitySpawns"></param>
        public void GenerateProximitySpawns(List<ProximitySpawnData> proximitySpawns, string timeOfDay, Vector3 spawnPos)
        {
            if (proximitySpawns == null) return;

            foreach (var proximitySpawn in proximitySpawns)
            {
                uint chance;
                if (proximitySpawn.spawnInfoData.probability > 0 && RandomValues.FindValue((uint)proximitySpawn.spawnInfoData.probability, out chance) && 
                    (CanSpawn(proximitySpawn.spawnInfoData.spawnCategory) || proximitySpawn.spawnInfoData.ignoreTotalSpawns))
                {
                    // Randomize a position from original spawnPos and using min/max range
                    Vector3 proxSpawnPos = Utility.GetRandomPos(proximitySpawn.minRange, proximitySpawn.maxRange, spawnPos, _spawnMode, _spawnAxes);

                    float minElevation = proximitySpawn.spawnInfoData.overrideElevation ? proximitySpawn.spawnInfoData.minElevation : MinElevation;
                    float maxElevation = proximitySpawn.spawnInfoData.overrideElevation ? proximitySpawn.spawnInfoData.maxElevation : MaxElevation;
                    float minSlopeAngle = proximitySpawn.spawnInfoData.overrideSlopeAngle ? proximitySpawn.spawnInfoData.minSlopeAngle : MinSlopeAngle;
                    float maxSlopeAngle = proximitySpawn.spawnInfoData.overrideSlopeAngle ? proximitySpawn.spawnInfoData.maxSlopeAngle : MaxSlopeAngle;

                    proximitySpawn.spawnInfoData.spawnAreaGuid = string.Empty;
                    proximitySpawn.spawnInfoData.waveSpawnerGuid = string.Empty;
                    proximitySpawn.spawnInfoData.waveItemGuid = string.Empty;
                    proximitySpawn.spawnInfoData.randomChance = chance;
                    ValidateSpawnRules(proximitySpawn.spawnInfoData, proxSpawnPos, timeOfDay, proximitySpawn.spawnInfoData.ignorePlayerFOV, minElevation,
                        maxElevation, minSlopeAngle, maxSlopeAngle);
                } // if
            } // foreach
        } // GenerateProximitySpawns()

        /// <summary>
        /// Perform checks on the proposed spawn position to ensure that it is valid.
        /// </summary>
        /// <param name="spawnData"></param>
        /// <param name="isValid"></param>
        /// <returns></returns>
        Vector3 ValidateSpawnPos(SpawnData spawnData, out bool isValid)
        {
            if (spawnData.ignoreRaycast && !spawnData.spawnInAir)
            {
                isValid = true;
                return spawnData.spawnPos;
            } // if

            var spawnable = spawnData.spawnPrefab.GetComponentInChildren<ISpawnable>();

            // If spawnable does not require a navmesh then just return as valid if not on an ignored layer
            if (spawnData.spawnPrefab != null)
            {
                if (spawnData.spawnInAir)
                {
                    // Just raycast for highest surface then randomize a min/max altitude above that point based on maxAltitude
                    RaycastHit hit;

                    int layerMask = 1 << 2; // ignore raycast layer
                    layerMask = ~layerMask;

                    // Does the ray intersect on any objects excluding the ignore raycast and triggers layer
                    if (Physics.Raycast(new Vector3(spawnData.spawnPos.x, spawnData.spawnPos.y + _raycastDistance, spawnData.spawnPos.z), Vector3.down, out hit, 
                        Mathf.Infinity, layerMask))
                    {
                        if (SoulLinkGlobal.Instance.DebugMode)
                        {
                            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                        } // if

                        var altitude = UnityEngine.Random.Range(spawnData.minAltitude, spawnData.maxAltitude + 1);
                        isValid = true;
                        return new Vector3(hit.point.x, hit.point.y + altitude, hit.point.z);
                    } // if
                }
                else
                {
                    if (spawnable != null && !spawnable.GetRequiresNavMesh())
                    {
                        isValid = true;
                        if (!IsPositionValid(spawnData.spawnPos.x, spawnData.spawnPos.y, spawnData.spawnPos.z, _raycastDistance))
                        {
                            isValid = false;
                            return Vector3.zero;
                        }
                        else
                        {
                            var yPos = GetAdjustedYPos(spawnData.spawnPos.x, spawnData.spawnPos.y, spawnData.spawnPos.z, _raycastDistance);
                            return new Vector3(spawnData.spawnPos.x, yPos, spawnData.spawnPos.z);
                        }
                    } // if
                } // if
            } // if

            // Validate navmesh position if spawn requires a navmesh
            if (spawnable != null && spawnable.GetRequiresNavMesh())
            {
                int tries = 0;
                var origSpawnPos = spawnData.spawnPos;
                isValid = false;
                while (!isValid && tries < VALIDATION_RETRIES)
                {
                    // TODO: revisit this idea in the future if there is a problem
                    // Do not allow this spawn position if it is inside minimum spawn range
                    //                if (Vector3.Distance(spawnData.spawnPos, PlayerTransform.position) < _minimumSpawnRange) 
                    //                {
                    //                    ++tries;
                    //                    continue;
                    //                } // if

                    // TODO: Add check for specific NavMesh areas. Must add that data to spawns.
                    //                int fieldMask = 1 << NavMesh.GetAreaFromName("Field");
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(spawnData.spawnPos, out hit, 2.0f, NavMesh.AllAreas))
                    {
                        spawnData.spawnPos = hit.position;
                        if (IsPositionValid(spawnData.spawnPos.x, spawnData.spawnPos.y, spawnData.spawnPos.z, _raycastDistance))
                        {
                            isValid = true;
                            return spawnData.spawnPos;
                        } // if
                    } // if

                    FailSpawn(spawnData, "Could not find valid spawn position at " + spawnData.spawnPos + ". Retrying nearby position.");

                    // Failed, so try to find nearby spot to spawn somewhere within 2f radius
                    spawnData.spawnPos = Utility.GetRandomPos(2f, 2f, origSpawnPos, _spawnMode, _spawnAxes);

                    ++tries;
                } // while
            }
            else
            {
                isValid = true;
                return spawnData.spawnPos;
            }

            isValid = false;
            return Vector3.zero;
        } // ValidateSpawnPos()

        /// <summary>
        /// Increment the current population for the specified prefab
        /// </summary>
        /// <param name="sourceObj"></param>
        public void PopulationAdd(Transform sourceObj)
        {
            if (_populations.ContainsKey(sourceObj))
            {
                _populations[sourceObj]++;
            }
            else
            {
                _populations[sourceObj] = 1;
            }
        } // PopulationAdd()

        /// <summary>
        /// Decrement the current population for the specified prefab
        /// </summary>
        /// <param name="sourceObj"></param>
        public void PopulationSubtract(Transform sourceObj)
        {
            if (sourceObj == null) return;

            // Remove from population
            if (_populations.ContainsKey(sourceObj))
            {
                _populations[sourceObj]--;
                SoulLinkGlobal.DebugLog(gameObject, "   Despawning " + sourceObj.name + ", population = " + _populations[sourceObj]);

                if (_populations[sourceObj] == 0)
                {
                    _populations.Remove(sourceObj);
                } // if
            } // if
        } // PopulationSubtract()

        /// <summary>
        /// Get the current population for the specified prefab.
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <returns></returns>
        public int GetPopulation(Transform sourceObj)
        {
            if (_populations.ContainsKey(sourceObj))
            {
                return _populations[sourceObj];
            } // if

            return 0;
        } // GetPopulation()

        /// <summary>
        /// Despawn
        /// </summary>
        /// <param name="spawn"></param>
        public void Despawn(Transform spawn)
        {
            ISpawnable spawnable = spawn.gameObject.GetComponentInChildren<ISpawnable>();
            if (spawnable == null) return;

            if (spawnable.GetSpawnArea() != null)
            {
                spawnable.GetSpawnArea().DecSpawnTotal();
            } // if

            if (spawnable.GetWaveSpawner() != null)
            {
//                spawnable.GetWaveSpawner().something;
            }

            PopulationSubtract(spawnable.GetSpawnPrefab());
            if (!spawnable.GetIgnoreTotalSpawns())
            {
                DecTotalSpawns(spawnable.GetSpawnCategory());
            } // if

            // If herdID is > 0 then remove from the herd.
            uint herdID = spawnable.GetHerdID();
            if (herdID > 0)
            {
                RemoveFromHerd(spawnable);
            }
            else
            {
                RandomValues.ReturnValue(spawnable.GetRandomChance());
            }

            PoolManager.Despawn(spawn);
        } // Despawn()

        /// <summary>
        /// Calculates the y position for the spawn by checking against walkable meshes that may be placed on the terrain.
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        public float GetYPos(Vector3 spawnPos)
        {
            Terrain terrain = Utility.TerrainContaining(spawnPos, _useMapMagic);
            if (terrain != null)
            {
                // Check above the terrain position to see if there might be something higher up to spawn onto
                float y = terrain.SampleHeight(spawnPos);
                return GetAdjustedYPos(spawnPos.x, y, spawnPos.z);
            } // if

            return GetAdjustedYPos(spawnPos.x, spawnPos.y, spawnPos.z, _raycastDistance);
        } // GetYPos()

        /// <summary>
        /// Samples the terrain at the specified position to get the height at that position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float GetTerrainYPos(float x, float y, float z)
        {
            Vector3 pos = new Vector3(x, y, z);
            Terrain terrain = Utility.TerrainContaining(pos, _useMapMagic);
            if (terrain != null)
            {
                return terrain.SampleHeight(pos);
            } // if

            SoulLinkGlobal.DebugLog(gameObject, "Failed to GetTerrainYPos at " + x + ", " + y + ", " + z);

            return 0f;
        } // GetTerrainYPos()

        /// <summary>
        /// GetTerrainYPos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float GetTerrainYPos(Vector3 pos)
        {
            return GetTerrainYPos(pos.x, pos.y, pos.z);
        } // GetTerrainYPos()

        /// <summary>
        /// Calculates the y position for the spawn by checking against walkable meshes that may be placed on the terrain.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public float GetAdjustedYPos(float x, float y, float z, float range = 30f)
        {
            if (_spawnMode == SpawnMode.SpawnIn2D) return y;

            RaycastHit hit;
            // Does the ray intersect on any objects excluding the ignore raycast and triggers layer
            if (Physics.Raycast(new Vector3(x, y + range, z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, ~_navMeshIgnoreLayers.value))
            {
                if (SoulLinkGlobal.Instance.DebugMode)
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                } // if
                return hit.point.y;
            } // if

            return y;
        } // GetAdjustedYPos()

        /// <summary>
        /// Determines if a point in the game world is a valid spawn position by checking a raycast against any layers that should be ignored.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool IsPositionValid(float x, float y, float z, float range = 30f)
        {
            RaycastHit hit;
            // Does the ray intersect on any objects on ignored layers. If we get a hit then we want to prevent spawning beneath it. In this particular case,
            // we do not want to consider Ignore Raycast as a valid layer to check.
            LayerMask includeMask = _navMeshIgnoreLayers;
            includeMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));
            if (Physics.Raycast(new Vector3(x, y + range, z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, includeMask))
            {
                if (!hit.collider.isTrigger)
                {
                    if (SoulLinkGlobal.Instance.DebugMode)
                    {
                        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    } // if
                    return false;
                } // if
            } // if

            return true;
        } // IsPositionValid()

        /// <summary>
        /// IsNavMeshLinkActivatorTag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsNavMeshLinkActivatorTag(string tag)
        {
            return _linkActivatorTags.Contains(tag);
        } // IsNavMeshLinkActivatorTag()

        #region PoolManager Functions
        /// <summary>
        /// Update the PoolManager with all the latest spawn information
        /// </summary>
        void UpdatePoolManager(SoulLinkSpawnerDatabase database)
        {
            if (database == null) return;

            if (PoolManager != null)
            {
                // Remove all previous items before updating so that any deleted spawns are removed also
                PoolManager.ClearPoolItems(database);

                // Go through all spawns and add them to PoolManager
                //
                // 1. Process Biome Spawn Data
                foreach (var item in database.biomeSpawnData)
                {
                    foreach (var todItems in item.timeOfDaySpawns.Values)
                    {
                        foreach (var spawnInfoData in todItems.spawnInfoData)
                        {
                            // If spawnType is Herd then process herd data instead of individual prefab
                            if (spawnInfoData.spawnType == SpawnType.Herd)
                            {
                                if (spawnInfoData.herd != null)
                                {
                                    UpdatePoolManager(spawnInfoData.herd.herdPrefabs, database);
                                }
                                else
                                {
                                    // TODO: a popup error and a way to remove from this database?
                                }
                            }
                            else if (spawnInfoData.spawnType == SpawnType.Prefab)
                            {
                                PoolManager.AddPoolItem(spawnInfoData.prefab, spawnInfoData.populationCap, database, spawnInfoData.reusable,
                                    spawnInfoData.reusable ? false : true);
                            }
                            else if (spawnInfoData.spawnType == SpawnType.Variants)
                            {
                                if (spawnInfoData.prefabVariants.Count > 0)
                                {
                                    foreach (var spawn in spawnInfoData.prefabVariants)
                                    {
                                        PoolManager.AddPoolItem(spawn, spawnInfoData.populationCap, database, spawnInfoData.reusable,
                                            spawnInfoData.reusable ? false : true);
                                    } // foreach
                                } // if
                            }

                            UpdatePoolManager(Database.GetProximitySpawns(spawnInfoData.spawnGuid), database);
                        } // foreach spawnData
                    } // for each todItems
                } // foreach item

                // 2. Process Area Spawn Data
                foreach (var item in database.areaSpawnData)
                {
                    foreach (var todItems in item.timeOfDaySpawns.Values)
                    {
                        foreach (var spawnInfoData in todItems.spawnInfoData)
                        {
                            // If spawnType is Herd then process herd data instead of individual prefab
                            if (spawnInfoData.spawnType == SpawnType.Herd && spawnInfoData.herd != null)
                            {
                                UpdatePoolManager(spawnInfoData.herd.herdPrefabs, database);
                            }
                            else if (spawnInfoData.spawnType == SpawnType.Prefab)
                            {
                                PoolManager.AddPoolItem(spawnInfoData.prefab, spawnInfoData.populationCap, database, spawnInfoData.reusable,
                                    spawnInfoData.reusable ? false : true);
                            }
                            else
                            {
                                if (spawnInfoData.prefabVariants.Count > 0)
                                {
                                    foreach (var spawn in spawnInfoData.prefabVariants)
                                    {
                                        PoolManager.AddPoolItem(spawn, spawnInfoData.populationCap, database, spawnInfoData.reusable,
                                            spawnInfoData.reusable ? false : true);
                                    } // foreach
                                } // if
                            }

                            UpdatePoolManager(database.GetProximitySpawns(spawnInfoData.spawnGuid), database);
                        } // foreach spawnData
                    } // for each todItems
                } // foreach item
            } // if
        } // UpdatePoolManager()

        /// <summary>
        /// Update PoolManager with a list of spawns
        /// </summary>
        /// <param name="spawns"></param>
        void UpdatePoolManager(List<SpawnInfoData> spawns, SoulLinkSpawnerDatabase database)
        {
            foreach (var spawn in spawns)
            {
                PoolManager.AddPoolItem(spawn.prefab, spawn.populationCap, database, spawn.reusable, spawn.reusable ? false : true);
            } // foreach spawn
        } // UpdatePoolManager()

        /// <summary>
        /// Update PoolManager with all spawns in the proximity spawns collection
        /// </summary>
        /// <param name="proximitySpawns"></param>
        void UpdatePoolManager(List<ProximitySpawnData> proximitySpawns, SoulLinkSpawnerDatabase database)
        {
            if (proximitySpawns == null) return;

            foreach (var proximitySpawn in proximitySpawns)
            {
                if (proximitySpawn.spawnInfoData.spawnType == SpawnType.Herd)
                {
                    if (proximitySpawn.spawnInfoData.herd != null)
                    {
                        UpdatePoolManager(proximitySpawn.spawnInfoData.herd.herdPrefabs, database);
                    } // if
                }
                else
                {
                    PoolManager.AddPoolItem(proximitySpawn.spawnInfoData.prefab, proximitySpawn.spawnInfoData.populationCap, database,
                        proximitySpawn.spawnInfoData.reusable, proximitySpawn.spawnInfoData.reusable ? false : true);
                }
            } // foreach
        } // UpdatePoolManager()
        #endregion

        #region Biome Query Functions

        /// <summary>
        /// QueryBiomes
        /// </summary>
        /// <param name="spawnPos"></param>
        public void QueryBiomes(Vector3 spawnPos)
        {
            _currentBiomes.Clear();

            if (_useMapMagic)
            {
#if MAPMAGIC2
                // Query the biome values in the spawn location
                foreach (var holder in _matricesHolders)
                {
                    foreach (var map in holder.maps)
                    {
                        if (DirectMatricesHolder.FindValueAtPosition(_mapMagicObject, map.Key, spawnPos.x, spawnPos.z) >= _biomeThreshold)
                        {
                            _currentBiomes.Add(map.Key);
                        }
                    } // foreach map
                } // foreach holder

                _currentBiomes.Add(IgnoreBiomesKey); // Always have this biome
#endif
            }
            else
            {
                // Check each available biome  to see if we are in it
                foreach (var biome in _biomes)
                {
                    if (biome.IsInBiome(spawnPos))
                    {
                        _currentBiomes.Add(biome.BiomeName);
                    }
                }
            }

            if (_currentBiomes.Count == 0)
            {
                SoulLinkGlobal.DebugLog(gameObject, "NO BIOMES AT " + spawnPos.x + "," + spawnPos.z);
            }
        } // QueryBiomes()


#if MAPMAGIC2
        /// <summary>
        /// FindHolder
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public DirectMatricesHolder FindHolder(float x, float z)
        {
            foreach (DirectMatricesHolder holder in _matricesHolders)
            {
                if (holder.ContainsPosition(x, z))
                {
                    return holder;
                } // if
            } // foreach

            return null;
        } // FindHolder()
#endif
#endregion
    } // class SoulLinkSpawner
} //namespace Magique.SoulLink
