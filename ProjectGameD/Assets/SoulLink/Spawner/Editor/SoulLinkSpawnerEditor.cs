using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if SOULLINK_USE_DS || SOULLINK_USE_QM
using PixelCrushers;
#endif

#if MAPMAGIC2
using MapMagic.Core;
using MapMagic.Terrains;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SoulLinkSpawner), true)]
    public class SoulLinkSpawnerEditor : Editor
    {
        public enum Category
        {
            Biomes,
            Areas,
            TimeOfDay
        };

        public enum Section
        {
            Definitions,
            Spawns
        }

        public enum Definition
        {
            TimeOfDay,
            Biomes,
            Herds,
            Category
        }

        private SoulLinkSpawner _myTarget;
        private SerializedObject _mySerializedObject;

#if MAPMAGIC2
        private MapMagicObject _mapMagicObject;
        private DirectMatricesHolder[] _matricesHolders;
#endif

        static private List<string> _biomeNames;
        private int _biomeSelected = 0;
        private int _newBiomeSelected = 0;

        private Dictionary<string, int> _todSelected = new Dictionary<string, int>();
        private Dictionary<string, int> _todAreaSelected = new Dictionary<string, int>();

        private string newDBName = "";
        static private Dictionary<string, bool> ShowBiomeData = new Dictionary<string, bool>();
        static private Dictionary<string, bool> ShowAreaData = new Dictionary<string, bool>();
        private Dictionary<string, bool> ShowSpawnData = new Dictionary<string, bool>();

        private string newAreaName = "";

        private string[] SectionStrings = { "Definitions", "Spawns" };
        private string[] DefinitionStrings = { "Time of Day", "Biomes", "Herds", "Category" };
        private string[] CategoryStrings = { "by Biome", "by Area" };
        private List<Category> _categories;
        private List<string> _categoryLabels;
        static private int _categorySelected = 0;

        private List<Section> _sections;
        private List<string> _sectionLabels;
        static private int _sectionSelected = 0;

        private List<Definition> _definitions;
        private List<string> _definitionLabels;
        static private int _definitionSelected = 0;

        private float _textureThresholdValue = 0.5f;

        private bool _useMapMagic = false;

        static private bool _spawningOptionsFoldoutState = true;
        static private bool _navMeshOptionsFoldoutState = false;
        static private bool _debugOptionsFoldout = false;
#if SOULLINK_USE_AINAV
        private string _activatorTagSelected;
        static private bool _generateNavMeshFoldoutState = false;
        static private bool _generateNavMeshLinksFoldoutState = false;
#endif

        // bool
        SerializedProperty SpawnModeProp, GenerateNavMeshProp, GenerateNavMeshLinksProp, SpawnOutsidePlayerFOVProp, DespawnOutsidePlayerFOVProp, DisableBiomeSpawnsProp, DisableAreaSpawnsProp, 
            AutoDetectPlayerProp, AutoAddSpawnableProp, UseSpawnCirclesProp;

        // float
        SerializedProperty SpawnRadiusProp, TimeSyncFreqProp, WeatherSyncFreqProp, BiomeThresholdProp, MinSpawnRangeProp, NavCheckRadiusProp, MinElevationProp, MaxElevationProp, 
            MinSlopeAngleProp, MaxSlopeAngleProp, AgentSlopeProp, AgentStepHeightProp, AgentHeightProp, RaycastDistanceProp, ClusterRangeProp;

        // int
        SerializedProperty SpawnCirclesProp, DefaultPoulationpCapProp, SpawnRateProp, CellDivisionProp, LinksPerCellProp, DelaySpawnsOnStartProp;

        // string
        SerializedProperty PlayerTagProp, DefaultSpawnActionProp;

        // enum

        // object
        SerializedProperty PlayerTransformProp, DatabaseProp, NavMeshIgnoreLayersProp, SpawnValidatorProp, SpawnableSettingsProp, SpawnAxesProp;

        static private SpawnerHerd[] _herdResources;
        private Transform _playerTransform = null;

        private void OnEnable()
        {
            _myTarget = (SoulLinkSpawner)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            _myTarget.PoolManager = FindObjectOfType<PoolManager>();

            if (_myTarget.AutoDetectPlayer)
            {
                var obj = GameObject.FindGameObjectWithTag(_myTarget.PlayerTag);
                if (obj != null)
                {
                    _playerTransform = obj.transform;
                } // if
            }
            else
            {
                _playerTransform = _myTarget.PlayerTransform;
            }

#if MAPMAGIC2
            _mapMagicObject = FindObjectOfType<MapMagicObject>();
            _matricesHolders = FindObjectsOfType<DirectMatricesHolder>();
            _useMapMagic = (_mapMagicObject != null && _matricesHolders.Length > 0);
#endif

            if (_categories == null)
            {
                InitTabs();
            } // if

            Utility.FindSpawnerComponents();
            Utility.InitTextureNames();
            InitBiomeNames();
            InitAreaNames();
            Utility.InitTODNames();
            Utility.InitWeatherConditionNames();
            Utility.InitQuestData();

            // bool property initializers
            SpawnModeProp = _mySerializedObject.FindProperty("_spawnMode");
            GenerateNavMeshProp = _mySerializedObject.FindProperty("_generateNavMesh");
            GenerateNavMeshLinksProp = _mySerializedObject.FindProperty("_generateNavMeshLinks");
            SpawnOutsidePlayerFOVProp = _mySerializedObject.FindProperty("_spawnOutsidePlayerFOV");
            DespawnOutsidePlayerFOVProp = _mySerializedObject.FindProperty("_despawnOutsidePlayerFOV");
            DisableBiomeSpawnsProp = _mySerializedObject.FindProperty("_disableBiomeSpawns");
            DisableAreaSpawnsProp = _mySerializedObject.FindProperty("_disableAreaSpawns");
            AutoDetectPlayerProp = _mySerializedObject.FindProperty("_autoDetectPlayer");
            AutoAddSpawnableProp = _mySerializedObject.FindProperty("_autoAddSpawnableInterface");

            // float property initializers
            SpawnRadiusProp = _mySerializedObject.FindProperty("_spawnRadius");
            TimeSyncFreqProp = _mySerializedObject.FindProperty("_timeSyncFrequency");
            WeatherSyncFreqProp = _mySerializedObject.FindProperty("_weatherSyncFrequency");
            BiomeThresholdProp = _mySerializedObject.FindProperty("_biomeThreshold");
            MinSpawnRangeProp = _mySerializedObject.FindProperty("_minimumSpawnRange");
            SpawnRateProp = _mySerializedObject.FindProperty("_spawnRate");
            NavCheckRadiusProp = _mySerializedObject.FindProperty("_navCheckRadius");
            MinElevationProp = _mySerializedObject.FindProperty("_minElevation");
            MaxElevationProp = _mySerializedObject.FindProperty("_maxElevation");
            MinSlopeAngleProp = _mySerializedObject.FindProperty("_minSlopeAngle");
            MaxSlopeAngleProp = _mySerializedObject.FindProperty("_maxSlopeAngle");
            AgentSlopeProp = _mySerializedObject.FindProperty("_agentSlope");
            AgentStepHeightProp = _mySerializedObject.FindProperty("_agentStepHeight");
            AgentHeightProp = _mySerializedObject.FindProperty("_agentHeight");
            RaycastDistanceProp = _mySerializedObject.FindProperty("_raycastDistance");
            ClusterRangeProp = _mySerializedObject.FindProperty("_clusterRange");

            // int property initializers
            UseSpawnCirclesProp = _mySerializedObject.FindProperty("_useSpawnCircles");
            SpawnCirclesProp = _mySerializedObject.FindProperty("_spawnCircles");
            DefaultPoulationpCapProp = _mySerializedObject.FindProperty("_defaultPopulationCap");
            CellDivisionProp = _mySerializedObject.FindProperty("_cellDivisions");
            LinksPerCellProp = _mySerializedObject.FindProperty("_linksPerCell");
            DelaySpawnsOnStartProp = _mySerializedObject.FindProperty("_delaySpawnsOnStart");

            // string property initializers
            PlayerTagProp = _mySerializedObject.FindProperty("_playerTag");
            DefaultSpawnActionProp = _mySerializedObject.FindProperty("_defaultSpawnAction");

            // enum property initializers

            // object property initializers
            PlayerTransformProp = _mySerializedObject.FindProperty("_playerTransform");
            DatabaseProp = _mySerializedObject.FindProperty("_SoulLinkSpawnerDatabase");
            NavMeshIgnoreLayersProp = _mySerializedObject.FindProperty("_navMeshIgnoreLayers");
            SpawnValidatorProp = _mySerializedObject.FindProperty("_spawnValidator");
            SpawnableSettingsProp = _mySerializedObject.FindProperty("_spawnableSettings");
            SpawnAxesProp = _mySerializedObject.FindProperty("_spawnAxes");

            // Apply hidden attributes to biome components (fixes upgrade issue from 1.0.1 to 1.1.0)
            BiomeTerrain[] biomes = _myTarget.gameObject.GetComponents<BiomeTerrain>();
            foreach (var biome in biomes)
            {
                biome.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            } // foreach
        } // OnEnable()

        private void InitTabs()
        {
            _categories = EditorUtils.GetListFromEnum<Category>();
            _categoryLabels = new List<string>();
            foreach (var category in CategoryStrings)
            {
                _categoryLabels.Add(category);
            } // foreach

            _sections = EditorUtils.GetListFromEnum<Section>();
            _sectionLabels = new List<string>();
            foreach (var section in SectionStrings)
            {
                _sectionLabels.Add(section);
            } // foreach

            _definitions = EditorUtils.GetListFromEnum<Definition>();
            _definitionLabels = new List<string>();
            foreach (var definition in DefinitionStrings)
            {
                _definitionLabels.Add(definition);
            } // foreach
        } // InitTabs()

        private void LoadHerdResources()
        {
            _herdResources = Resources.LoadAll<SpawnerHerd>("");
        } // LoadHerdResources()

        private void InitBiomeNames()
        {
            if (_biomeNames == null)
            {
                _biomeNames = new List<string>();
            } // if

            if (_useMapMagic)
            {
#if MAPMAGIC2
                foreach (var holder in _matricesHolders)
                {
                    foreach (var map in holder.maps)
                    {
                        if (!_biomeNames.Contains(map.Key))
                        {
                            _biomeNames.Add(map.Key);
                            ShowBiomeData[map.Key] = false;
                        } // if
                    } // foreach
                } // foreach

                if (!_biomeNames.Contains(IgnoreBiomesKey))
                {
                    _biomeNames.Add(IgnoreBiomesKey);
                    ShowBiomeData[IgnoreBiomesKey] = false;
                } // if
#endif
            }
            else
            {
                if (_myTarget.Database != null)
                {
                    foreach (var biome in _myTarget.Biomes)
                    {
                        if (biome != null)
                        {
                            if (biome.BiomeName != null && !_biomeNames.Contains(biome.BiomeName))
                            {
                                _biomeNames.Add(biome.BiomeName);
                                ShowBiomeData[biome.BiomeName] = true;
                            }
                            else if (biome.BiomeName != null && !ShowBiomeData.ContainsKey(biome.BiomeName))
                            {
                                ShowBiomeData[biome.BiomeName] = true;
                            }
                        } // if
                    } // foreach

                    // Remove bad biomes 
                    for (int i = 0; i < _biomeNames.Count; ++i)
                    {
                        bool inDB = false;
                        foreach (var biome in _myTarget.Biomes)
                        {
                            inDB = (biome != null && biome.BiomeName == _biomeNames[i]);
                            if (inDB) break;
                        } // foreach

                        if (!inDB)
                        {
                            if (ShowBiomeData.ContainsKey(_biomeNames[i]))
                            {
                                ShowBiomeData.Remove(_biomeNames[i]);
                            } // if

                            _biomeNames.RemoveAt(i);
                        } // if
                    } // for i
                } // if
            } // if
        } // InitBiomeNames()

        private void InitAreaNames()
        {
            if (_myTarget.Database == null) return;

            foreach (var area in _myTarget.Database.areaSpawnData)
            {
                if (!ShowAreaData.ContainsKey(area.areaName))
                {
                    ShowAreaData[area.areaName] = true;
                } // if
            } // foreach

            // TODO: cleanup keys that no longer exist
        } // InitAreaNames()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginWindowStyle();

            EditorGUILayout.BeginHorizontal();
            EditorUtils.DrawSoulLinkLogo();
            EditorUtils.DrawSpawnerText();
            EditorGUILayout.EndHorizontal();

            DrawGeneralInfo();

            if (!Application.isPlaying)
            {
                if (_myTarget.Database != null)
                {
                    DrawSectionTabs();
                } // if
            }
            else
            {
                EditorGUILayout.LabelField("Changes to SoulLink Spawner cannot be made when the application is running. Please stop the game to make changes.", EditorUtils.guiStyle_Help);
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the SoulLinkSpawner into Undo
                Undo.RecordObject(_myTarget, "SoulLinkSpawner Editor Modify");

                EditorUtility.SetDirty(_myTarget);
                if (_myTarget.Database != null)
                {
                    EditorUtility.SetDirty(_myTarget.Database);
                } // if

                if (_myTarget.PoolManager != null)
                {
                    EditorUtility.SetDirty(_myTarget.PoolManager);
                } // if

                _mySerializedObject.ApplyModifiedProperties();

                if (!Application.isPlaying)
                {
                    // Automatically update PoolManager
//                    UpdatePoolManager();
                } // if
            } // if
        } // OnInspectorGUI()

        private void DrawSectionTabs()
        {
            _sectionSelected = GUILayout.Toolbar(_sectionSelected, _sectionLabels.ToArray(), EditorUtils.tabStyle);

            switch (_sectionSelected)
            {
                case 0:
                    DrawDefinitionTabs();
                    break;
                case 1:
                    DrawCategoryTabs();
                    break;
                default:
                    break;
            } // switch
        } // DrawSectionTabs()

        private void DrawDefinitionTabs()
        {
            _definitionSelected = GUILayout.Toolbar(_definitionSelected, _definitionLabels.ToArray());

            switch (_definitionSelected)
            {
                case 0:
                    DrawTimeOfDayGUI();
                    break;
                case 1:
                    if (_useMapMagic)
                    {
#if MAPMAGIC2
                        EditorGUILayout.LabelField("Biomes are discovered automatically when MapMagic is in use.", EditorUtils.guiStyle_Help);
#endif
                    }
                    else
                    {
                        DrawBiomesGUI();
                    }
                    break;
                case 2:
                    DrawHerdsGUI();
                    break;
                case 3:
                    DrawSpawnCategoriesGUI();
                    break;
                default:
                    break;
            } // switch
        } // DrawDefinitionTabs()

        private void DrawCategoryTabs()
        {
            _categorySelected = GUILayout.Toolbar(_categorySelected, _categoryLabels.ToArray());

            switch (_categorySelected)
            {
                case 0:
                    DrawSpawnsByBiomes();
                    break;
                case 1:
                    DrawSpawnsByArea();
                    break;
                default:
                    break;
            } // switch
        } // DrawCategoryTabs()

        void DrawGeneralInfo()
        {
            EditorUtils.BeginBoxStyleLight();

            if (_myTarget.Database == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Create New Database: ");
                newDBName = EditorGUILayout.TextField(newDBName);
                bool buttonCreate = GUILayout.Button("Create", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                if (buttonCreate)
                {
                    if (newDBName != "")
                    {
                        // Create new database
                        var dbName = "Assets/" + newDBName + ".asset";
                        SoulLinkSpawnerDatabase database = CreateInstance<SoulLinkSpawnerDatabase>();
                        database.name = newDBName;
                        AssetDatabase.CreateAsset(database, dbName);

                        // Assign database rsource to SpawnManager
                        var asset = AssetDatabase.LoadAssetAtPath(dbName, typeof(SoulLinkSpawnerDatabase)) as SoulLinkSpawnerDatabase;
                        _myTarget.Database = asset;
                        _myTarget.Database.Initialize();
                        newDBName = "";
                    } 
                    else
                    {
                        if (EditorUtility.DisplayDialog(
                            "Error",
                            "You must enter a database name to create a new database.",
                            "OK"))
                        {
                        } // if
                        GUIUtility.ExitGUI();
                    }
                } // if
                EditorGUILayout.EndHorizontal();
            } // if

            if (Application.isPlaying)
            {
                GUI.enabled = false;
            } // if

            EditorGUILayout.PropertyField(DatabaseProp);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(SpawnModeProp);
            if (SpawnModeProp.enumValueIndex == (int)SpawnMode.SpawnIn2D)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(SpawnAxesProp);
                EditorGUI.indentLevel--;
            } // if

            EditorGUILayout.BeginHorizontal();
            if (_myTarget.PoolManager == null)
            {
                EditorGUILayout.LabelField("No PoolManager Found. Add a PoolManager to your scene.", EditorUtils.guiStyle_Help);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(AutoDetectPlayerProp);
            if (_myTarget.AutoDetectPlayer)
            {
                PlayerTagProp.stringValue = EditorGUILayout.TagField("Player Tag", PlayerTagProp.stringValue);
            }
            else
            {
                EditorGUILayout.PropertyField(PlayerTransformProp);
            }

            EditorGUILayout.PropertyField(AutoAddSpawnableProp);
            if (_myTarget.AutoAddSpawnableInterface)
            {
                EditorGUI.indentLevel++;
                _myTarget.SpawnableSettings = EditorGUILayout.ObjectField("Spawnable Settings", _myTarget.SpawnableSettings, 
                    typeof(SpawnableSettingsObject), false) as SpawnableSettingsObject;
                EditorGUI.indentLevel--;
            } // if

            EditorGUILayout.PropertyField(DefaultSpawnActionProp);
            EditorGUILayout.Separator();

            EditorGUI.indentLevel++;
            _spawningOptionsFoldoutState = EditorGUILayout.Foldout(_spawningOptionsFoldoutState, "Spawning Options", true);
            if (_spawningOptionsFoldoutState)
            {
                // Save off spawn radius and min spawn range and make sure they are not changed to invalid values
                var spawnRadius = _myTarget.SpawnRadius;
                var minSpawnRange = _myTarget.MinimumSpawnRange;
                EditorGUILayout.PropertyField(SpawnRadiusProp);
                if (_myTarget.SpawnRadius < _myTarget.MinimumSpawnRange)
                {
                    EditorGUILayout.HelpBox("Spawn Radius can't be smaller than the Minimum Spawn Range.", MessageType.Error);
                } // if
                EditorGUILayout.PropertyField(MinSpawnRangeProp);
                if (_myTarget.MinimumSpawnRange > _myTarget.SpawnRadius)
                {
                    EditorGUILayout.HelpBox("Minimum Spawn Range can't be larger than the Spawn Radius.", MessageType.Error);
                } // if
                EditorGUILayout.PropertyField(UseSpawnCirclesProp);
                if (UseSpawnCirclesProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(SpawnCirclesProp);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(ClusterRangeProp);
                EditorGUILayout.PropertyField(SpawnRateProp);
                EditorGUILayout.PropertyField(DefaultPoulationpCapProp);
                EditorGUILayout.PropertyField(DelaySpawnsOnStartProp);

                if (_myTarget.SpawnMode == SpawnMode.SpawnIn3D)
                {
                    EditorGUILayout.PropertyField(MinElevationProp);
                    EditorGUILayout.PropertyField(MaxElevationProp);
                    EditorGUILayout.PropertyField(MinSlopeAngleProp);
                    EditorGUILayout.PropertyField(MaxSlopeAngleProp);
                } // if

                EditorGUILayout.PropertyField(SpawnOutsidePlayerFOVProp);
                EditorGUILayout.PropertyField(DespawnOutsidePlayerFOVProp);
                EditorGUILayout.PropertyField(SpawnValidatorProp);
                EditorGUILayout.PropertyField(TimeSyncFreqProp);
                EditorGUILayout.PropertyField(WeatherSyncFreqProp);
                if (_useMapMagic)
                {
#if MAPMAGIC2
                    EditorGUILayout.PropertyField(BiomeThresholdProp);
#endif
                } // if
            } // if
            EditorGUI.indentLevel--;

            if (Application.isPlaying)
                GUI.enabled = false;

            EditorGUI.indentLevel++;
            _navMeshOptionsFoldoutState = EditorGUILayout.Foldout(_navMeshOptionsFoldoutState, "Nav Mesh Options", true);
            if (_navMeshOptionsFoldoutState)
            {
                EditorGUILayout.PropertyField(NavMeshIgnoreLayersProp);
                EditorGUILayout.PropertyField(RaycastDistanceProp);

#if SOULLINK_USE_AINAV
                EditorGUILayout.PropertyField(GenerateNavMeshProp);

                if (_myTarget.GenerateNavMesh)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.HelpBox("This is an advanced feature normally reserved only for scenes that cannot pre-bake a navigation mesh " +
                        " or for testing purposes. Do not turn this feature on unless you know how to use it properly. Use of this feature " +
                        "requires special setup. Please refer to the documentation for complete details on the correct use of this feature.", MessageType.Warning);

                    _generateNavMeshFoldoutState = EditorGUILayout.Foldout(_generateNavMeshFoldoutState, "Generate Nav Mesh Options", true);
                    if (_generateNavMeshFoldoutState)
                    {
                        // Show any props specific to this feature
                        EditorGUILayout.PropertyField(CellDivisionProp);
                        EditorGUILayout.PropertyField(NavCheckRadiusProp);
                        EditorGUILayout.PropertyField(AgentSlopeProp);
                        EditorGUILayout.PropertyField(AgentStepHeightProp);
                        EditorGUILayout.PropertyField(AgentHeightProp);
                        EditorGUILayout.PropertyField(GenerateNavMeshLinksProp);

                        if (_myTarget.GenerateNavMeshLinks)
                        {
                            EditorGUI.indentLevel++;
                            _generateNavMeshLinksFoldoutState = EditorGUILayout.Foldout(_generateNavMeshLinksFoldoutState, "Generate Nav Mesh Links Options", true);
                            if (_generateNavMeshLinksFoldoutState)
                            {
                                EditorGUILayout.PropertyField(LinksPerCellProp);

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Tags to Activate Nav Mesh Links");
                                _activatorTagSelected = EditorGUILayout.TagField(_activatorTagSelected);

                                bool buttonAdd = GUILayout.Button("+", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                                if (buttonAdd)
                                {
                                    if (!_myTarget.LinkActivatorTags.Contains(_activatorTagSelected))
                                    {
                                        _myTarget.LinkActivatorTags.Add(_activatorTagSelected);

                                    } // if
                                } // if
                                EditorGUILayout.EndHorizontal();

                                for (int i = 0; i < _myTarget.LinkActivatorTags.Count; ++i)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(60));
                                    EditorGUILayout.LabelField(_myTarget.LinkActivatorTags[i]);

                                    bool buttonDelete = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                                    if (buttonDelete)
                                    {
                                        _myTarget.LinkActivatorTags.RemoveAt(i);
                                    } // if

                                    EditorGUILayout.EndHorizontal();
                                } // foreach
                            } // if foldout
                            EditorGUI.indentLevel--;
                        } // if
                    } // if foldout
                    EditorGUI.indentLevel--;
                } // if
#endif
            } // if foldout
            EditorGUI.indentLevel--;

            GUI.enabled = true;

            EditorGUI.indentLevel++;
            _debugOptionsFoldout = EditorGUILayout.Foldout(_debugOptionsFoldout, "Debug Options", true);
            if (_debugOptionsFoldout)
            {
                EditorGUILayout.PropertyField(DisableBiomeSpawnsProp);
                EditorGUILayout.PropertyField(DisableAreaSpawnsProp);
            } // if f0ldout
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        } // DrawGeneralInfo()

        void DrawSpawnsByBiomes()
        {
            if (_myTarget.Database == null) return;

            InitBiomeNames();
            Utility.InitTODNames();

            if (_biomeNames.Count == 0)
            {
                EditorGUILayout.LabelField("No Biomes defined. Go to Definitions and select the Biomes tab to start defining your biomes.", EditorUtils.guiStyle_Help);
                return;
            } // if

            if (Utility.TimeOfDayNames != null && Utility.TimeOfDayNames.Count == 0)
            {
                EditorGUILayout.LabelField("No Time of Day definitions found. Go to Definitions and select the Time of Day tab to start defining your Time of Day items.", EditorUtils.guiStyle_Help);
                return;
            } // if

            EditorUtils.BeginWindowStyle();

            string errorText = "";

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            int index = _biomeSelected;
            _biomeSelected = EditorGUILayout.Popup("Biomes", index, _biomeNames.ToArray());
            bool buttonAdd = GUILayout.Button("Add", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Select an available Biome from the the drop-down list above and then click Add", EditorUtils.guiStyle_Help);

            if (buttonAdd)
            {
                // Cannot be duplicate
                if (_myTarget.Database.biomeSpawnData.Find(x => x.biomeName == _biomeNames[_biomeSelected]) == null)
                {
                    _myTarget.Database.biomeSpawnData.Add(new BiomeSpawnData() { biomeName = _biomeNames[_biomeSelected] });
                }
                else
                {
                    errorText = "Cannot add Biome '" + _biomeNames[_biomeSelected] + "'. Biome is a duplicate";
                }
            }

            EditorGUILayout.EndVertical();

            // Draw existing biome spawn data here and then show Add button so user can add more
            EditorUtils.BeginBoxStyleDark();

            bool anyShowing = ShowBiomeData.ContainsValue(true);

            if (_myTarget.Database.biomeSpawnData.Count > 0)
            {
                bool buttonShowHideAll = GUILayout.Button(anyShowing ? "Hide All" : "Show All", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonShowHideAll)
                {
                    var keys = ShowBiomeData.Keys;
                    List<string> strings = new List<string>();
                    strings.AddRange(keys);

                    for (int i = 0; i < strings.Count; ++i)
                    {
                        ShowBiomeData[strings[i]] = !anyShowing;
                    }
                }
            } // if

            for (int i = 0; i < _myTarget.Database.biomeSpawnData.Count; ++i)
            {
                var biomeName = _myTarget.Database.biomeSpawnData[i].biomeName;
                if (ShowBiomeData.ContainsKey(biomeName))
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(biomeName, EditorUtils.guiStyle_LargeBold);

                    bool buttonDisable = GUILayout.Button(_myTarget.Database.biomeSpawnData[i].disabled ? " " : "√", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
//                    bool buttonUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
//                    bool buttonDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    bool buttonToggle = GUILayout.Button(ShowBiomeData[biomeName] ? "Hide" : "Show", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                    EditorGUILayout.EndHorizontal();

                    if (buttonDisable)
                    {
                        _myTarget.Database.biomeSpawnData[i].disabled = !_myTarget.Database.biomeSpawnData[i].disabled;
                    } // if

                    if (buttonToggle)
                    {
                        ShowBiomeData[biomeName] = !ShowBiomeData[biomeName];
                    } // if

                    if (!ShowBiomeData[biomeName])
                    {
                        EditorGUI.indentLevel--;
                        continue;
                    } // if

                    EditorGUILayout.BeginHorizontal();
                    int newIndex = _newBiomeSelected;
                    _newBiomeSelected = EditorGUILayout.Popup("Change Biome", newIndex, _biomeNames.ToArray());

                    bool buttonChangeBiome = GUILayout.Button("Change", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                    if (buttonChangeBiome)
                    {
                        // If the biome already exists in this database then show error message; add possible merge feature later
                        if (_myTarget.Database.biomeSpawnData.Find(e => e.biomeName == _biomeNames[_newBiomeSelected]) != null)
                        {
                            errorText = "Spawn data already exists for this Biome.";
                        }
                        else
                        {
                            if (_myTarget.Database.biomeSpawnData[i].biomeName != _biomeNames[_newBiomeSelected])
                            {
                                _myTarget.Database.biomeSpawnData[i].biomeName = _biomeNames[_newBiomeSelected];
                            } // if
                        }
                    } // if
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;

                    // Display error popup dialog if necessary
                    if (errorText != string.Empty)
                    {
                        if (EditorUtility.DisplayDialog(
                            "Error",
                            errorText,
                            "OK"))
                        {
                        } // if
                    } // if

                    _myTarget.Database.biomeSpawnData[i] = DrawBiomeSpawnItemData(_myTarget.Database.biomeSpawnData[i]);

                    if (buttonRemove)
                    {
                        // Make sure the user really wants to do this
                        if (EditorUtility.DisplayDialog(
                            "Confirm",
                            "Are you sure you want to remove this Biome's Spawns?",
                            "OK", "CANCEL"))
                        {
                            _myTarget.Database.biomeSpawnData.RemoveAt(i);
                            GUIUtility.ExitGUI();
                        } // if
                    } // if
                    EditorUtils.BeginWindowStyle();
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndVertical();
                    //EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // if
            } // for i

            EditorGUILayout.EndVertical();
        } // DrawSpawnsByBiomes()

        BiomeSpawnData DrawBiomeSpawnItemData(BiomeSpawnData spawnData)
        {
            Utility.InitTODNames();

            EditorGUILayout.Separator();

            string errorText = "";

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();

            int index = _todSelected.ContainsKey(spawnData.biomeName) ? _todSelected[spawnData.biomeName] : 0;
            index = EditorGUILayout.Popup("Pick a Time of Day", index, Utility.TimeOfDayNames.ToArray());

            bool buttonAdd = GUILayout.Button("Add", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonAdd)
            {
                // Cannot be duplicate
                if (!spawnData.timeOfDaySpawns.ContainsKey(Utility.TimeOfDayNames[index]))
                {
                    spawnData.timeOfDaySpawns[Utility.TimeOfDayNames[index]] = new SpawnListData();
                }
                else
                {
                    errorText = "Cannot add Time of Day Spawns for '" + Utility.TimeOfDayNames[index] + "'. Time of Day is a duplicate";
                }
            } // if
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Select a Time of Day from the drop-down list above and then click Add", EditorUtils.guiStyle_Help);
            EditorGUI.indentLevel--;

            EditorUtils.BeginBoxStyleLight();

            // Display error popup dialog if necessary
            if (errorText != string.Empty)
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    errorText,
                    "OK"))
                {
                } // if
            } // if

            if (DatabaseHasEntries())
            {
                spawnData = DrawSpawnInfoItemsGUI(spawnData);
            }
            else
            {
                EditorGUILayout.LabelField("There are no Time of Day entries in the database.");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            _todSelected[spawnData.biomeName] = index;

            // Return the modified element
            return spawnData;
        } // DrawBiomeSpawnItemData()

        AreaData DrawAreaSpawnItemData(AreaData areaData)
        {
            EditorGUILayout.Separator();

            string errorText = "";

            EditorGUI.indentLevel++;
            newAreaName = EditorGUILayout.TextField("Area Name", areaData.areaName);

            if (_myTarget.Database.areaSpawnData.Find(e => e.areaName == newAreaName && e != areaData) != null && newAreaName != "")
            {
                EditorGUILayout.LabelField("Area Name is a duplicate");
            }
            else if (newAreaName == "")
            {
                EditorGUILayout.LabelField("Area Name cannot be empty");
            }
            else
            {
                // Update ShowAreaData key
                if (ShowAreaData.ContainsKey(areaData.areaName))
                {
                    ShowAreaData[newAreaName] = ShowAreaData[areaData.areaName];
                    ShowAreaData.Remove(areaData.areaName);
                } // if

                areaData.areaName = newAreaName;

                // Update all SpawnArea components in the scene to the new name
                SpawnArea[] spawnAreas = Resources.FindObjectsOfTypeAll(typeof(SpawnArea)) as SpawnArea[];
                foreach (var spawnArea in spawnAreas)
                {
                    if (spawnArea.AreaName == areaData.areaName)
                    {
                        spawnArea.AreaName = newAreaName;
                        areaData.areaName = newAreaName;
                    } // if
                } // foreach
            }

            EditorGUILayout.BeginHorizontal();

            int index = _todAreaSelected.ContainsKey(areaData.areaName) ? _todAreaSelected[areaData.areaName] : 0;
            index = EditorGUILayout.Popup("Pick a Time of Day", index, Utility.TimeOfDayNames.ToArray());

            bool buttonAdd = GUILayout.Button("Add", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonAdd)
            {
                // Cannot be duplicate
                if (!areaData.timeOfDaySpawns.ContainsKey(Utility.TimeOfDayNames[index]))
                {
                    areaData.timeOfDaySpawns[Utility.TimeOfDayNames[index]] = new SpawnListData();
                }
                else
                {
                    errorText = "Cannot add Time of Day Spawns for '" + Utility.TimeOfDayNames[index] + "'. Time of Day is a duplicate";
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Display error popup dialog if necessary
            if (errorText != string.Empty)
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    errorText,
                    "OK"))
                {
                } // if
            } // if

            EditorUtils.BeginBoxStyleLight();

            if (DatabaseHasEntries())
            {
                areaData = DrawAreaSpawnInfoItemsGUI(areaData);
                EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
            }
            else
            {
                EditorGUILayout.LabelField("There are no Time of Day entries in the database.");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            _todAreaSelected[areaData.areaName] = index;

            // Return the modified element
            return areaData;
        } // DrawAreaSpawnItemData()

        BiomeSpawnData DrawSpawnInfoItemsGUI(BiomeSpawnData spawnData)
        {
            EditorGUILayout.Separator();

            EditorUtils.BeginBoxStyleLight();

            List<string> removeTODs = new List<string>();
            Dictionary<string, string> keyChanges = new Dictionary<string, string>();
            foreach (var tod in spawnData.timeOfDaySpawns)
            {
                var rangeStr = _myTarget.Database.timeOfDayData.Find(e => e.timeOfDayName == tod.Key).GetRangeString();
                var todText = tod.Key + " (" + rangeStr + ")";
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width((EditorGUIUtility.currentViewWidth / 2.5f) - (GUI.skin.label.CalcSize(new GUIContent(todText)).x) / 2));
                EditorGUILayout.LabelField(todText, EditorUtils.guiStyle_LargeBold);
                EditorGUILayout.EndHorizontal();

                // User can edit the time of day, but insure a duplicate isn't selected. Merge duplicates if requested.
                int index = Utility.TimeOfDayNames.IndexOf(tod.Key);
                index = EditorGUILayout.Popup("Change Time of Day", index, Utility.TimeOfDayNames.ToArray());
                if (tod.Key != Utility.TimeOfDayNames[index])
                {
                    keyChanges.Add(tod.Key, Utility.TimeOfDayNames[index]);
                } // if

                // Foldout for spawns in this time of day group
                EditorGUI.indentLevel++;
                var spawnFoldoutKey = spawnData.biomeName + tod.Key;
                if (ShowSpawnData.ContainsKey(spawnFoldoutKey))
                {
                    ShowSpawnData[spawnFoldoutKey] = EditorGUILayout.Foldout(ShowSpawnData[spawnFoldoutKey], "Spawns (" + tod.Value.spawnInfoData.Count + ")", true);
                }
                else
                {
                    ShowSpawnData[spawnFoldoutKey] = true;
                }

                if (ShowSpawnData[spawnFoldoutKey])
                {
                    int count = 0;
                    foreach (var spawn in tod.Value.spawnInfoData)
                    {
                        EditorUtils.BeginBoxStyleDark();
                        EditorGUILayout.Separator();

                        SpawnInfoDataEditorHelper helper = new SpawnInfoDataEditorHelper();
                        helper.AssignTarget(_myTarget, _myTarget.AutoAddSpawnableInterface, _myTarget.SpawnableSettings);
                        helper.AssignDatabase(_myTarget.Database);

                        bool moveUp = false;
                        bool moveDown = false;
                        bool remove = false;
                        if (helper.EditSpawn(spawn, out moveUp, out moveDown, out remove))
                        {
                            if (moveUp)
                            {
                                Utility.MoveItemUp(tod.Value.spawnInfoData, tod.Value.spawnInfoData.IndexOf(spawn));
                            } // if

                            if (moveDown)
                            {
                                Utility.MoveItemDown(tod.Value.spawnInfoData, tod.Value.spawnInfoData.IndexOf(spawn));
                            } // if

                            if (remove)
                            {
                                // Make sure the user really wants to do this
                                if (EditorUtility.DisplayDialog(
                                    "Confirm",
                                    "Are you sure you want to remove this spawn?",
                                    "OK", "CANCEL"))
                                {
                                    spawnData.timeOfDaySpawns[tod.Key].spawnInfoData.Remove(spawn);
                                } // if
                            } // if

                            EditorUtility.SetDirty(_myTarget.Database);
                            if (moveUp || moveDown || remove) GUIUtility.ExitGUI();
                        } // if

                        EditorGUILayout.Separator();
                        EditorGUILayout.EndVertical();

                        ++count;
                        if (count < spawnData.timeOfDaySpawns.Count)
                        {
                            EditorUtils.HorizontalLine(EditorUtils.colorDarkGray, 2f, Vector3.one);
                            EditorGUILayout.Separator();
                        } // if
                    } // foreach spawn

                    EditorGUILayout.BeginHorizontal();

                    bool buttonAdd = GUILayout.Button("Add Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                    if (buttonAdd)
                    {
                        var newSpawn = new SpawnInfoData();
                        newSpawn.spawnGuid = Guid.NewGuid().ToString();
                        newSpawn.populationCap = _myTarget.DefaultPopulationCap;
                        spawnData.timeOfDaySpawns[tod.Key].spawnInfoData.Add(newSpawn);
                    } // if
                    EditorGUILayout.LabelField("Add a spawn to the " + tod.Key + " time of day", EditorUtils.guiStyle_Help);
                    EditorGUILayout.EndHorizontal();
                } // if foldout true
                EditorGUI.indentLevel--;

                // Delete button for time of day groups
                EditorGUILayout.BeginHorizontal();
                bool buttonDeleteTOD = GUILayout.Button("Delete Time of Day", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                EditorGUILayout.LabelField("Delete all spawns from the " + tod.Key + " time of day", EditorUtils.guiStyle_Help);
                EditorGUILayout.EndHorizontal();
                if (buttonDeleteTOD)
                {
                    // Make sure the user really wants to do this
                    if (EditorUtility.DisplayDialog(
                        "Confirm",
                        "Are you sure you want to remove this Time of Day's Spawns?",
                        "OK", "CANCEL"))
                    {
                        spawnData.timeOfDaySpawns.Remove(tod.Key);
                        GUIUtility.ExitGUI();
                    } // if
                } // if
                
                EditorUtils.HorizontalLine(EditorUtils.colorDarkGray, 2f, Vector3.one);
                EditorGUILayout.Separator();
            } // foreach time of day

            // Make any key changes here
            foreach (var item in keyChanges)
            {
                ChangeTimeOfDayKey(spawnData, item.Key, item.Value);
            } // foreach

            EditorGUILayout.EndVertical();

            return spawnData;
        } // DrawSpawnInfoItemsGUI()

        AreaData DrawAreaSpawnInfoItemsGUI(AreaData areaData)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            List<string> removeTODs = new List<string>();
            Dictionary<string, string> keyChanges = new Dictionary<string, string>();

            int count = 0;
            int todCount = areaData.timeOfDaySpawns.Count;
            foreach (var tod in areaData.timeOfDaySpawns)
            {
                List<SpawnInfoData> removeSpawns = new List<SpawnInfoData>();

                var rangeStr = _myTarget.Database.timeOfDayData.Find(e => e.timeOfDayName == tod.Key).GetRangeString();
                var todText = tod.Key + " (" + rangeStr + ")";
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width((EditorGUIUtility.currentViewWidth / 2.5f) - (GUI.skin.label.CalcSize(new GUIContent(todText)).x) / 2));
                EditorGUILayout.LabelField(todText, EditorUtils.guiStyle_LargeBold);
                EditorGUILayout.EndHorizontal();

                // User can edit the time of day, but insure a duplicate isn't selected. Merge duplicates if requested.
                int index = Utility.TimeOfDayNames.IndexOf(tod.Key);
                index = EditorGUILayout.Popup("Change Time of Day", index, Utility.TimeOfDayNames.ToArray());
                if (tod.Key != Utility.TimeOfDayNames[index])
                {
                    keyChanges.Add(tod.Key, Utility.TimeOfDayNames[index]);
                } // if

                // Foldout for spawns in this time of day group
                EditorGUI.indentLevel++;
                var spawnFoldoutKey = areaData.areaName + tod.Key;
                if (ShowSpawnData.ContainsKey(spawnFoldoutKey))
                {
                    ShowSpawnData[spawnFoldoutKey] = EditorGUILayout.Foldout(ShowSpawnData[spawnFoldoutKey], "Spawns", true);
                }
                else
                {
                    ShowSpawnData[spawnFoldoutKey] = true;
                }

                if (ShowSpawnData[spawnFoldoutKey])
                {
                    foreach (var spawn in tod.Value.spawnInfoData)
                    {
                        SpawnInfoDataEditorHelper helper = new SpawnInfoDataEditorHelper();
                        helper.AssignTarget(_myTarget, _myTarget.AutoAddSpawnableInterface, _myTarget.SpawnableSettings);
                        helper.AssignDatabase(_myTarget.Database);

                        bool moveUp = false;
                        bool moveDown = false;
                        bool remove = false;
                        if (helper.EditSpawn(spawn, out moveUp, out moveDown, out remove))
                        {
                            if (moveUp)
                            {
                                Utility.MoveItemUp(tod.Value.spawnInfoData, tod.Value.spawnInfoData.IndexOf(spawn));
                            } // if

                            if (moveDown)
                            {
                                Utility.MoveItemDown(tod.Value.spawnInfoData, tod.Value.spawnInfoData.IndexOf(spawn));
                            } // if

                            if (remove)
                            {
                                // Make sure the user really wants to do this
                                if (EditorUtility.DisplayDialog(
                                    "Confirm",
                                    "Are you sure you want to remove this spawn?",
                                    "OK", "CANCEL"))
                                {
                                    areaData.timeOfDaySpawns[tod.Key].spawnInfoData.Remove(spawn);
                                } // if
                            } // if

                            EditorUtility.SetDirty(_myTarget.Database);
                            if (moveUp || moveDown || remove) GUIUtility.ExitGUI();
                        } // if

                        EditorUtils.HorizontalLine(EditorUtils.colorDarkGray, 2f, Vector3.one);
                        EditorGUILayout.Separator();
                    } // foreach spawn

                    EditorGUILayout.BeginHorizontal();

                    bool buttonAdd = GUILayout.Button("Add Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                    if (buttonAdd)
                    {
                        var newSpawn = new SpawnInfoData();
                        newSpawn.spawnGuid = Guid.NewGuid().ToString();
                        newSpawn.populationCap = _myTarget.DefaultPopulationCap;
                        areaData.timeOfDaySpawns[tod.Key].spawnInfoData.Add(newSpawn);
                    } // if
                    EditorGUILayout.EndHorizontal();
                } // if foldout true
                EditorGUI.indentLevel--;

                // Delete button for time of day groups
                bool buttonDeleteTOD = GUILayout.Button("Delete Time of Day", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonDeleteTOD)
                {
                    removeTODs.Add(tod.Key);
                } // if

                ++count;
                if (count != todCount)
                {
                    EditorUtils.HorizontalLine(EditorUtils.colorDarkGray, 1, Vector2.one);
                } // if
            } // foreach time of day

            // Make any key changes here
            foreach (var item in keyChanges)
            {
                ChangeTimeOfDayKey(areaData, item.Key, item.Value);
            } // foreach

            // Remove items here from time of day
            foreach (var todKey in removeTODs)
            {
                areaData.timeOfDaySpawns.Remove(todKey);
            } // foreach

            EditorGUILayout.EndVertical();

            return areaData;
        } // DrawAreaSpawnInfoItemsGUI()

        void ChangeTimeOfDayKey(BiomeSpawnData spawnData, string oldKey, string newKey)
        {
            // Check if the new key is already in the database
            if (spawnData.timeOfDaySpawns.ContainsKey(newKey))
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "Time of Day Entry already exists for " + newKey,
                    "OK"))
                {
                } // if

                return;
            } // if

            SpawnListData spawnListData = spawnData.timeOfDaySpawns[oldKey];
            spawnData.timeOfDaySpawns.Remove(oldKey);
            spawnData.timeOfDaySpawns.Add(newKey, spawnListData);
        } // ChangeTimeOfDayKey();

        void ChangeTimeOfDayKey(AreaData spawnData, string oldKey, string newKey)
        {
            // Check if the new key is already in the database
            if (spawnData.timeOfDaySpawns.ContainsKey(newKey))
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "Time of Day Entry already exists for " + newKey,
                    "OK"))
                {
                } // if

                return;
            } // if
            
            SpawnListData spawnListData = spawnData.timeOfDaySpawns[oldKey];
            spawnData.timeOfDaySpawns.Remove(oldKey);
            spawnData.timeOfDaySpawns.Add(newKey, spawnListData);
        } // ChangeTimeOfDayKey();

        private void DrawSpawnsByArea()
        {
            if (_myTarget.Database == null) return;

            Utility.InitTODNames();
            InitAreaNames();

            if (Utility.TimeOfDayNames.Count == 0)
            {
                EditorGUILayout.LabelField("No Time of Day definitions found. Go to Definitions and select the Time of Day tab to start defining your Time of Day items.", EditorUtils.guiStyle_Help);
                return;
            } // if

            EditorUtils.BeginWindowStyle();

            EditorUtils.BeginBoxStyleDark();

            bool anyShowing = ShowAreaData.ContainsValue(true);

            if (_myTarget.Database.areaSpawnData.Count > 0)
            {
                bool buttonShowHideAll = GUILayout.Button(anyShowing ? "Hide All" : "Show All", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonShowHideAll)
                {
                    var keys = ShowAreaData.Keys;
                    List<string> strings = new List<string>();
                    strings.AddRange(keys);

                    for (int j = 0; j < strings.Count; ++j)
                    {
                        ShowAreaData[strings[j]] = !anyShowing;
                    }
                }
            } // if

            for (int i = 0; i < _myTarget.Database.areaSpawnData.Count; ++i)
            {
                EditorGUI.indentLevel++;
                var areaName = _myTarget.Database.areaSpawnData[i].areaName;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(areaName, EditorUtils.guiStyle_LargeBold, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                bool buttonCreate = GUILayout.Button("S", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(28));
                if (buttonCreate)
                {
                    // Create a custom game object
                    GameObject go = new GameObject("Spawn Area - " + areaName);

                    // Register the creation in the undo system
                    Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
                    Selection.activeObject = go;

                    SceneView.lastActiveSceneView.MoveToView();
                    var spawnArea = go.AddComponent<SpawnArea>();
                    spawnArea.AreaName = areaName;
                    spawnArea.DistributeHerdsFromCenter = true;

                    // Check if save system is in scene and add the saver component automatically if there
#if SOULLINK_USE_DS || SOULLINK_USE_QM
                    var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
                    if (saveSystem.Length != 0)
                    {
                        var saver = spawnArea.gameObject.AddComponent<SpawnArea_Saver>();
                        saver.key = Guid.NewGuid().ToString();
                    } // if
#endif
                } // if

                bool buttonDelete = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
//                bool buttonUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
//                bool buttonDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                bool buttonToggle = GUILayout.Button((ShowAreaData.ContainsKey(areaName) && ShowAreaData[areaName]) ? "Hide" : "Show", GUILayout.Height(EditorGUIUtility.singleLineHeight), 
                    GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                if (buttonToggle && ShowAreaData.ContainsKey(areaName))
                {
                    ShowAreaData[areaName] = !ShowAreaData[areaName];
                } // if
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                if (buttonDelete)
                {
                    // Make sure the user really wants to do this
                    if (EditorUtility.DisplayDialog(
                        "Confirm",
                        "Are you sure you want to remove this area?",
                        "OK", "CANCEL"))
                    {

                        _myTarget.Database.areaSpawnData.RemoveAt(i);
                        GUI.changed = true;
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    if (ShowAreaData.ContainsKey(areaName) && ShowAreaData[areaName])
                    {
                        _myTarget.Database.areaSpawnData[i] = DrawAreaSpawnItemData(_myTarget.Database.areaSpawnData[i]);
                    } // if
                }
            } // for i

            EditorGUILayout.EndVertical();

            bool buttonAdd = GUILayout.Button("Add Area", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAdd)
            {
                _myTarget.Database.areaSpawnData.Add(new AreaData() { areaName = "Unnamed" });
            } // if

            EditorGUILayout.EndVertical();
        } // DrawSpawnsByArea()

        private void DrawTimeOfDayGUI()
        {
            if (_myTarget.Database == null) return;

            EditorGUILayout.Separator();

            EditorUtils.BeginBgndStyleDark();

            for (int i = 0; i < _myTarget.Database.timeOfDayData.Count; ++i)
            {
                var tod = _myTarget.Database.timeOfDayData[i];

                EditorUtils.BeginBoxStyleLight();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Time of Day Name: ");
                tod.timeOfDayName = EditorGUILayout.TextField(tod.timeOfDayName);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Start Time: ");
                tod.startTime = EditorGUILayout.FloatField(tod.startTime);
                EditorGUILayout.LabelField(Utility.BuildTimeString(tod.startTime));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("End Time: ");
                tod.endTime = EditorGUILayout.FloatField(tod.endTime);
                EditorGUILayout.LabelField(Utility.BuildTimeString(tod.endTime));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                bool buttonDelete = GUILayout.Button("Delete Time of Day", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonDelete)
                {
                    _myTarget.Database.timeOfDayData.RemoveAt(i);
                } // if

                EditorGUILayout.Separator();
            } // foreach

            EditorGUILayout.EndVertical();

            bool buttonAdd = GUILayout.Button("Add Time of Day", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAdd)
            {
                _myTarget.Database.timeOfDayData.Add(new TimeOfDayData());
            } // if
        } // DrawTimeOfDayGUI()

        private void DrawBiomesGUI()
        {
            for (int i = 0; i < _myTarget.Biomes.Count; ++i)
            { 
                BiomeTerrainEditorHelper helper = new BiomeTerrainEditorHelper();
                helper.AssignTarget(_myTarget.Biomes[i]);
                helper.DrawGeneralInfo();
                helper.DrawTextureRulesGUI();
                helper.DrawVolumeRulesGUI();
                //helper.DrawLayerRulesGUI();
                helper.DrawElevationRangeGUI();

#if LANDSCAPE_BUILDER
                helper.DrawStencilRulesGUI();
#endif
                bool buttonDelete = GUILayout.Button("Remove Biome", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));
                if (buttonDelete)
                {
                    var biomes = _myTarget.GetComponentsInChildren<BiomeTerrain>();
                    foreach (var biome in biomes)
                    {
                        if (biome == _myTarget.Biomes[i])
                        {
                            DestroyImmediate(_myTarget.Biomes[i]);
                        } // if
                    } // foreach
                    _myTarget.Biomes.RemoveAt(i);
                    GUIUtility.ExitGUI();
                } // if

                EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);

                helper.UpdateObject();
            } // foreach

            EditorGUILayout.BeginHorizontal();

            bool buttonAddBiome = GUILayout.Button("Add Biome", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));
            //bool buttonImport = GUILayout.Button("Import Biome", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));

            EditorGUILayout.EndHorizontal();

            if (buttonAddBiome)
            {
                var newBiome = _myTarget.gameObject.AddComponent<BiomeTerrain>();
                newBiome.BiomeName = "Unnamed";
                newBiome.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                _myTarget.Biomes.Add(newBiome);
            } // if
        } // DrawBiomesGUI()

        private void DrawHerdsGUI()
        {
            if (_herdResources == null || _herdResources.Length == 0)
            {
                bool buttonLoad = GUILayout.Button("Load Herds", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));
                if (buttonLoad)
                {
                    LoadHerdResources();

                    if (_herdResources.Length == 0)
                    {
                        EditorGUILayout.LabelField("No herds defined.");
                    } // if
                } // if
            } // if

//            if (_herdResources == null || _herdResources.Length == 0) return;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Important Note: Herds are global assets that can be used across Spawner databases." +
                " Adding or removing them here will affect all your SoulLinkSpawner setups.", EditorUtils.guiStyle_Help);
            EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);

            if (_herdResources != null)
            {
				foreach (var herd in _herdResources)
				{
					SpawnerHerdEditorHelper helper = new SpawnerHerdEditorHelper();
					helper.AssignTarget(herd);
					helper.DrawGeneralInfo();
					helper.DrawSpawnInfoItemsGUI();

					bool buttonDelete = GUILayout.Button("Remove Herd", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));
					if (buttonDelete)
					{
						if (EditorUtility.DisplayDialog(
							"Warning",
							"Removing this Herd will completely remove it from your entire project. Are you sure?",
							"OK", "CANCEL"))
						{
							Resources.UnloadAsset(herd);
							var path = AssetDatabase.GetAssetPath(herd);
							AssetDatabase.DeleteAsset(path);

							LoadHerdResources();
							GUIUtility.ExitGUI();
						} // if
					} // if

					EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);

					helper.UpdateObject();
				} // foreach
			} // if
            EditorGUILayout.EndVertical();

            bool buttonAddHerd = GUILayout.Button("Add Herd", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAddHerd)
            {
                // Have user enter a path and name for the new herd asset
                string path = EditorUtility.SaveFilePanel("New Herd Asset", Application.dataPath, "SpawnerHerd", "asset");
                if (path.Length != 0)
                {
                    // strip off prefix path data so it starts with Assets. If Assets is not in the path then throw and error.
                    int pos = path.IndexOf("Assets");
                    if (pos == -1)
                    {
                        if (EditorUtility.DisplayDialog(
                            "Error",
                            "Invalid path. Asset must be created within the Assets folder of your project.",
                            "OK"))
                        {
                        } // if
                    }
                    else
                    {
                        if (path.IndexOf("Resources") == -1)
                        {
                            if (EditorUtility.DisplayDialog(
                                "Error",
                                "Invalid path. Asset must be created within a Resources sub-folder of your project.",
                                "OK"))
                            {
                            } // if
                        }
                        else
                        {

                            var projectPath = path.Substring(pos, path.Length - pos);

                            // Create new herd asset
                            SpawnerHerd herd = CreateInstance<SpawnerHerd>();
                            herd.herdName = "Unnamed";
                            herd.minPopulation = 5;
                            herd.maxPopulation = 10;
                            herd.spawnRadius = 8f;
                            AssetDatabase.CreateAsset(herd, projectPath);

                            LoadHerdResources();
                            GUIUtility.ExitGUI();
                        }
                    }
                } // if
            } // if
        } // DrawHerdsGUI()

        void DrawSpawnCategoriesGUI()
        {
            if (_myTarget.Database == null) return;

            EditorGUILayout.Separator();

            EditorUtils.BeginBgndStyleDark();

            bool reInitCatNames = false;
            for (int i = 0; i < _myTarget.Database.spawnCategories.Count; ++i)
            {
                string oldCatName = _myTarget.Database.spawnCategories[i].categoryName;

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = (_myTarget.Database.spawnCategories[i].categoryName != DEFAULT_SPAWN_CATEGORY_NAME);
                _myTarget.Database.spawnCategories[i].categoryName = EditorGUILayout.TextField("Category Name:", _myTarget.Database.spawnCategories[i].categoryName);
                if (oldCatName != _myTarget.Database.spawnCategories[i].categoryName)
                {
                    reInitCatNames = true;
                } // if
                bool buttonDelete = GUI.enabled ? GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE)) : false;
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;
                 _myTarget.Database.spawnCategories[i].maxSpawns = EditorGUILayout.IntField("Max Spawns:", _myTarget.Database.spawnCategories[i].maxSpawns);
                if (buttonDelete)
                {
                    // Make sure the user really wants to do this
                    if (EditorUtility.DisplayDialog(
                        "Confirm",
                        "Are you sure you want to remove this Spawn category?",
                        "OK", "CANCEL"))
                    {
                        _myTarget.Database.spawnCategories.RemoveAt(i);
                        Utility.InitSpawnCategoryNames();
                        GUIUtility.ExitGUI();
                    } // if
                } // if
            } // for i

            bool buttonAdd = GUILayout.Button("Add Category", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(100));
            if (buttonAdd)
            {
                _myTarget.Database.spawnCategories.Add(new SpawnCategoryData() { categoryName = "Unnamed Category", maxSpawns = 50 });
                Utility.InitSpawnCategoryNames();
            } // if

            if (reInitCatNames)
            {
                Utility.InitSpawnCategoryNames();
            } // if

            EditorGUILayout.EndVertical();
        } // DrawSpawnCategoriesGUI()

        private bool DatabaseHasEntries()
        {
            return (_myTarget.Database != null && _myTarget.Database.timeOfDayData.Count > 0);
        } // DatabaseHasEntries()

        /// <summary>
        /// Checks if there are entries for the specified database name and lets the user decide to keep them or remove them
        /// </summary>
        /// <param name="dbName"></param>
        void PoolManagerIntegrityCheck(string dbName)
        {
            if (_myTarget.PoolManager == null) return;

            if (_myTarget.PoolManager.CategoryExists(dbName))
            {
                if (!EditorUtility.DisplayDialog(
                    "Warning",
                    "PoolManager already contains items for a previous database." +
                    "Remove these entires or retain them for use as a Region?",
                    "KEEP", "REMOVE"))
                {
                    _myTarget.PoolManager.RemoveCategory(dbName);
                } // if

                GUIUtility.ExitGUI();
            } // if
        } // PoolManagerIntegrityCheck()

#if MAPMAGIC2
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

        public float FindValueAtPosition(string name, float x, float z)
        {
            DirectMatricesHolder holder = FindHolder(x, z);

            if (holder != null)
            {
                return holder.ValueAtPosition(name, x, z);
            }
            else
            {
                return 0;
            }
        } // FindValueAtPosition()
#endif

        private void OnSceneGUI()
        {
            if (_myTarget.SpawnRadius > 0f)
            {
                if (_playerTransform != null)
                {
                    Handles.color = new Color(0f, 0f, 1f, 0.1f);
                    Handles.DrawSolidDisc(_playerTransform.position, _myTarget.SpawnMode == SpawnMode.SpawnIn2D ? Vector3.forward : Vector3.up, _myTarget.SpawnRadius);
                    Handles.color = new Color(1f, 0f, 0f, 0.1f);
                    Handles.DrawSolidDisc(_playerTransform.position, _myTarget.SpawnMode == SpawnMode.SpawnIn2D ? Vector3.forward : Vector3.up, _myTarget.MinimumSpawnRange);
                }
            }
        } // OnSceneGUI()
    } // class SoulLinkSpawnerEditor
} // namespace Magique.SoulLink


