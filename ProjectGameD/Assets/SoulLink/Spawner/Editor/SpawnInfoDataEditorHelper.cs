using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if EMERALD_AI_PRESENT
using EmeraldAI;
using EmeraldAI.Utility;
#endif

#if INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
#endif

#if SOULLINK_USE_MAC
using MalbersAnimations.Controller.AI;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnInfoDataEditorHelper
    {
        private SoulLinkSpawner _myTarget;

        private List<string> _seasonNames = new List<string>();

        private float _textureThresholdValue = 0.5f;
        private float _globalSnowThresholdValue = 0.5f;
        private bool _autoAddSpawnable = false;
        private SpawnableSettingsObject _spawnableSettings;
        private SoulLinkSpawnerDatabase _database;
        private bool _showSpawnName = false;

        static private Transform _newVariant = null;
        static private SpawnInfoData _copySource = new SpawnInfoData();
        static private bool _canCopy = false;

        public void AssignTarget(SoulLinkSpawner target, bool autoAddSpawnable, SpawnableSettingsObject spawnableSettings, bool showSpawnName = true)
        {
            _myTarget = target;

            _showSpawnName = showSpawnName;
            InitSeasonNames();
            Utility.InitSpawnCategoryNames();

            _autoAddSpawnable = autoAddSpawnable;
            _spawnableSettings = spawnableSettings;
        } // AssignTarget()

        public void AssignDatabase(SoulLinkSpawnerDatabase database)
        {
            _database = database;
        } // AssignDatabase()

        public bool EditSpawn(SpawnInfoData spawn, out bool moveUp, out bool moveDown, out bool remove, bool allowProximitySpawns = true)
        {
            bool dbChanged = false;
            moveUp = false;
            moveDown = false;
            remove = false;

            var prevSpawnHerd = spawn.herd;
            var prevSpawnPrefab = spawn.prefab;

            // If this spawn does not have a Guid, then generate it now
            if (string.IsNullOrEmpty(spawn.spawnGuid))
            {
                spawn.spawnGuid = Guid.NewGuid().ToString();
            } // if

            if (!_showSpawnName)
            {
                spawn.hideSpawn = !EditorGUILayout.Foldout(!spawn.hideSpawn, "Spawn Data", true);
            } // if

            if (_showSpawnName)
            {
                EditorGUILayout.BeginHorizontal();
                if (spawn.spawnType == SpawnType.Prefab)
                {
                    if (spawn.prefab != null && _showSpawnName)
                    {
                        EditorGUILayout.LabelField("[" + spawn.prefab.name + "]", EditorUtils.guiStyle_MediumBold);
                    } // if
                }
                else if (spawn.spawnType == SpawnType.Herd)
                {
                    if (spawn.herd != null && _showSpawnName)
                    {
                        EditorGUILayout.LabelField("[" + spawn.herd.herdName + "]", EditorUtils.guiStyle_MediumBold);
                    } // if
                }
                else if (spawn.spawnType == SpawnType.Variants)
                {
                    if (_showSpawnName)
                    {
                        EditorGUILayout.LabelField("[" + spawn.variantsName + "]", EditorUtils.guiStyle_MediumBold);
                    } // if
                }

                bool buttonDisable = GUILayout.Button(spawn.disable ? " " : "√", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                if (buttonDisable)
                {
                    spawn.disable = !spawn.disable;
                    dbChanged = true;
                } // if

                moveUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                moveDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));

                var buttonCopy = GUILayout.Button("C", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                GUI.enabled = _canCopy; 
                var buttonPaste = GUILayout.Button("P", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                GUI.enabled = true;
                if (buttonCopy)
                {
                    _copySource = spawn;
                    _canCopy = true;
                } // if

                if (buttonPaste)
                {
                    CopySpawnWindow.ShowEditor(_copySource, spawn);
                    dbChanged = true;
                    GUIUtility.ExitGUI();
                }

                if (_showSpawnName)
                {
                    remove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                } // if

                bool buttonToggle = GUILayout.Button(!spawn.hideSpawn ? "Hide" : "Show", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
                if (buttonToggle)
                {
                    spawn.hideSpawn = !spawn.hideSpawn;
                    dbChanged = true;
                } // if

                if (spawn.prefab != prevSpawnPrefab || spawn.herd != prevSpawnHerd) dbChanged = true;

                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.LabelField(spawn.spawnGuid); // Uncomment to see the Guid in the inspector
            }
            if (dbChanged || moveUp || moveDown || remove) return true;
            if (spawn.hideSpawn) return dbChanged;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spawn Type: ");
            // TODO: if the type is changed then clear the previous reference
            spawn.spawnType = (SpawnType)(EditorGUILayout.EnumPopup(spawn.spawnType) as SpawnType?);
            EditorGUILayout.EndHorizontal();

            if (spawn.spawnType == SpawnType.Prefab)
            {
                spawn.prefab = EditorGUILayout.ObjectField("Prefab", spawn.prefab, typeof(Transform), false) as Transform;
                if (spawn.prefab != null)
                {
                    var spawnable = spawn.prefab.GetComponent<ISpawnable>();
                    if (spawnable == null)
                    {
                        if (_autoAddSpawnable)
                        {
                            // Add ISpawnable interface automatically to the prefab
                            AddISpawnable(spawn.prefab);
                        }
                        EditorGUILayout.HelpBox("The prefab you have selected does not have an ISpawnable interface.", MessageType.Error);
                    } // if

                    Utility.AddSpawnActionComponentByName(_myTarget.DefaultSpawnAction, spawn.prefab.gameObject);
                } // if
            }
            else if (spawn.spawnType == SpawnType.Herd)
            {
                spawn.herd = EditorGUILayout.ObjectField("Herd", spawn.herd, typeof(SpawnerHerd), false) as SpawnerHerd;
                if (spawn.herd != null)
                {
                    // Check that all herd prefabs have a Spawnable component
                    bool herdErrors = false;
                    foreach (var herdSpawn in spawn.herd.herdPrefabs)
                    {
                        if (herdSpawn.prefab != null)
                        {
                            var spawnable = herdSpawn.prefab.GetComponent<ISpawnable>();
                            if (spawnable == null)
                            {
                                herdErrors = true;
                                break;
                            }
                        } // if
                    } // foreach

                    if (herdErrors)
                    {
                        EditorGUILayout.HelpBox("One or more prefabs in the selected herd does not have an ISpawnable interface.", MessageType.Error);
                    } // if
                }
            }
            else if (spawn.spawnType == SpawnType.Variants)
            {
                spawn.variantsName = EditorGUILayout.TextField("Variants Name", spawn.variantsName);

                EditorGUILayout.HelpBox("To add a variant, drag the prefab into the field below and click the + button.", MessageType.Info);

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                _newVariant = EditorGUILayout.ObjectField("Prefab", _newVariant, typeof(Transform), false) as Transform;
                bool buttonAddVariant = GUILayout.Button("+", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                EditorGUILayout.EndHorizontal();

                if (_newVariant != null && buttonAddVariant)
                {
                    var spawnable = _newVariant.GetComponent<ISpawnable>();
                    if (spawnable == null)
                    {
                        if (_autoAddSpawnable)
                        {
                            // Add ISpawnable interface automatically to the prefab
                            AddISpawnable(_newVariant);
                        }
                        EditorGUILayout.HelpBox("The prefab you have selected does not have an ISpawnable interface.", MessageType.Error);
                    } // if

                    Utility.AddSpawnActionComponentByName(_myTarget.DefaultSpawnAction, _newVariant.gameObject);

                    if (!spawn.prefabVariants.Contains(_newVariant))
                    {
                        spawn.prefabVariants.Add(_newVariant);
                        _newVariant = null;
                    } // if

                    spawn.variantsFoldoutState = true;
                } // if

                spawn.variantsFoldoutState = EditorGUILayout.Foldout(spawn.variantsFoldoutState, "Variants (" + spawn.prefabVariants.Count + ")", true);
                if (spawn.variantsFoldoutState)
                {
                    // List all the variants and show a delete button
                    for (int i = 0; i < spawn.prefabVariants.Count; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(spawn.prefabVariants[i].name);
                        bool buttonRemoveVariant = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                        EditorGUILayout.EndHorizontal();

                        if (buttonRemoveVariant)
                        {
                            spawn.prefabVariants.RemoveAt(i);
                        } // if
                    } // for i
                } // if
                EditorGUI.indentLevel--;

                EditorGUILayout.Separator();
            }

            // Select Spawn Category
            int index = Utility.SpawnCategoryNames.IndexOf(spawn.spawnCategory);
            index = EditorGUILayout.Popup("Spawn Category", index, Utility.SpawnCategoryNames.ToArray());
            spawn.spawnCategory = Utility.SpawnCategoryNames[index];

            if (spawn.spawnType != SpawnType.Herd)
            {
                // Population Cap is only relevant to individual spawns, not herds
                spawn.populationCap = EditorGUILayout.IntField("Population Cap", spawn.populationCap);
                if (spawn.populationCap <= 0)
                {
                    spawn.populationCap = 1;
                } // if
                EditorGUILayout.HelpBox("This is a global population cap for this prefab. " + 
                    "Spawner will always use the highest value from all places this is defined for this prefab. " +
                    "Variants all receive the same population cap.", MessageType.Info);
            } // if

            spawn.ignoreTotalSpawns = EditorGUILayout.Toggle("Ignore Total Spawns", spawn.ignoreTotalSpawns);
            spawn.probability = EditorGUILayout.IntField("Probability", spawn.probability);
            spawn.ignorePlayerFOV = EditorGUILayout.Toggle("Ignore Player FOV", spawn.ignorePlayerFOV);

            spawn.reusable = EditorGUILayout.Toggle("Reusable", spawn.reusable);

            EditorGUILayout.Separator();

            if (spawn.spawnType == SpawnType.Prefab || spawn.spawnType == SpawnType.Variants)
            {
                spawn.minScale = EditorGUILayout.FloatField("Min Scale", spawn.minScale);
                spawn.maxScale = EditorGUILayout.FloatField("Max Scale", spawn.maxScale);
            } // if

            EditorGUILayout.Separator();

            spawn.overrideElevation = EditorGUILayout.Toggle("Override Elevation", spawn.overrideElevation);
            if (spawn.overrideElevation)
            {
                spawn.minElevation = EditorGUILayout.FloatField("     Min Elevation", spawn.minElevation);
                spawn.maxElevation = EditorGUILayout.FloatField("     Max Elevation", spawn.maxElevation);
            } // if

            spawn.overrideSlopeAngle = EditorGUILayout.Toggle("Override Slope Angle", spawn.overrideSlopeAngle);
            if (spawn.overrideSlopeAngle)
            {
                spawn.minSlopeAngle = EditorGUILayout.Slider("     Min Slope Angle", spawn.minSlopeAngle, 0f, 90f);
                spawn.maxSlopeAngle = EditorGUILayout.Slider("     Max Slope Angle", spawn.maxSlopeAngle, 0f, 90f);
            } // if

            spawn.spawnInAir = EditorGUILayout.Toggle("Spawn in Air", spawn.spawnInAir);
            if (spawn.spawnInAir)
            {
                EditorGUI.indentLevel++;
                spawn.minAltitude = EditorGUILayout.FloatField("Min Altitude", spawn.minAltitude);
                spawn.maxAltitude = EditorGUILayout.FloatField("Max Altitude", spawn.maxAltitude);
                EditorGUI.indentLevel--;
            } // if

            EditorGUI.indentLevel++;

            // Proximity Spawns
            if (allowProximitySpawns)
            {
                EditorGUILayout.Separator();
                dbChanged = ProximitySpawnsGUI(spawn);
                EditorGUILayout.Separator();
            } // if

            bool buttonToggleFilters = GUILayout.Button(spawn.showFilters ? "Hide Filters" : "Show Filters", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(112));
            if (buttonToggleFilters)
            {
                spawn.showFilters = !spawn.showFilters;
            } // if

            if (spawn.showFilters)
            {
                // Texture Filters
                if (Utility.TextureNames.Count > 0)
                {
                    TextureFiltersGUI(spawn);
                } // if

                // Weather Filters
                WeatherFiltersGUI(spawn);

                // Season Filters
                SeasonFiltersGUI(spawn);

                // Quest Filters
                QuestFiltersGUI(spawn);

#if SOULLINK_USE_GLOBALSNOW
                // Global Snow Filters
                GlobalSnowFiltersGUI(spawn);
#endif

                // Proximity Filters
                ProximityFiltersGUI(spawn);
            } // if

            EditorGUI.indentLevel--;

            return dbChanged;
        } // EditSpawn()

        /// <summary>
        /// Add the best possible ISpawnable available based on prefab components
        /// </summary>
        /// <param name="prefab"></param>
        public void AddISpawnable(Transform prefab)
        {
#if EMERALD_AI_PRESENT
            if (prefab.gameObject.GetComponent<EmeraldAIInitializer>())
            {
                prefab.gameObject.AddComponent<Spawnable_EmeraldAI>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

#if INVECTOR_AI_TEMPLATE
            if (prefab.gameObject.GetComponent<vControlAICombat>())
            {
                prefab.gameObject.AddComponent<Spawnable_InvectorFSMAI>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

#if SOULLINK_USE_GKC
            if (prefab.gameObject.GetComponentInChildren<health>())
            {
                var spawnable = prefab.gameObject.AddComponent<Spawnable_GKCAI>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

#if SOULLINK_USE_MAC
            if (prefab.gameObject.GetComponentInChildren<MAnimalAIControl>())
            {
                var spawnable = prefab.gameObject.AddComponent<Spawnable_MAC>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

#if SURVIVAL_ENGINE

            if (prefab.gameObject.GetComponentInChildren<SurvivalEngine.Selectable>())
            {
                var spawnable = prefab.gameObject.AddComponent<Spawnable_SurvivalEngine>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

#if FARMING_ENGINE

            if (prefab.gameObject.GetComponentInChildren<FarmingEngine.Selectable>())
            {
                var spawnable = prefab.gameObject.AddComponent<Spawnable_SurvivalEngine>();
                UpdateSpawnableSettings(prefab);
                return;
            } // if
#endif

            // Just add the generic Spawnable component if nothing specialized is needed
            prefab.gameObject.AddComponent<Spawnable>();
            UpdateSpawnableSettings(prefab);
        } // AddISpawnable(

        /// <summary>
        /// If _spawnableSettings is not null then copy the values to the newly added Spawnable component
        /// </summary>
        /// <param name="prefab"></param>
        void UpdateSpawnableSettings(Transform prefab)
        {
            if (_spawnableSettings == null) return;

            var spawnable = prefab.gameObject.GetComponent<ISpawnable>();
            if (spawnable != null)
            {
                spawnable.ImportSettings(_spawnableSettings);
            } // if
        } // UpdateSpawnableSettings()

        private void InitSeasonNames()
        {
            _seasonNames.AddRange(Enum.GetNames(typeof(Season)));
        } // InitSeasonNames()

        /// <summary>
        /// Edit texture filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void TextureFiltersGUI(SpawnInfoData spawn)
        {
            EditorGUILayout.BeginHorizontal();
            spawn.texFilterFoldoutState = EditorGUILayout.Foldout(spawn.texFilterFoldoutState, "Texture Filters (" + spawn.textureRules.Count + ")", true);
            bool buttonAddTexRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.texFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.textureRules.Count; ++i)
                {
                    var texRule = spawn.textureRules[i];
                    if (texRule.textureIndex < Utility.TextureNames.Count)
                    {
                        EditorGUILayout.LabelField(Utility.TextureNames[texRule.textureIndex]);

                        EditorGUILayout.BeginHorizontal();

                        EditorUtils.SaveLabelWidth();
                        EditorUtils.SetLabelWidth("Terrain Layer:          ");
                        texRule.textureIndex = EditorGUILayout.Popup("Terrain Layer: ", texRule.textureIndex, Utility.TextureNames.ToArray(), GUILayout.Width(200));

                        EditorUtils.SetLabelWidth("Rule:          ");
                        texRule.textureRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", texRule.textureRule);

                        EditorUtils.SetLabelWidth("Threshhold:          ");
                        texRule.threshold = EditorGUILayout.FloatField("Threshold: ", texRule.threshold, GUILayout.Width(164));
                        EditorUtils.RestoreLabelWidth();

                        bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                        if (buttonDeleteRule)
                        {
                            spawn.textureRules.RemoveAt(i);
                            if (spawn.textureRules.Count == 0)
                            {
                                spawn.texFilterFoldoutState = false;
                            } // if
                        } // if

                        EditorGUILayout.EndHorizontal();
                        EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                    } // if
                } // foreach texRule
                EditorGUILayout.EndVertical();
            } // if foldout is true

            if (buttonAddTexRule)
            {
                spawn.textureRules.Add(new TextureRuleData()
                { textureIndex = spawn.textureSelected, textureRule = (FilterRule)spawn.texRuleSelected, threshold = _textureThresholdValue });

                spawn.texFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // TextureFiltersGUI()

        /// <summary>
        /// Edit weather filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void WeatherFiltersGUI(SpawnInfoData spawn)
        {
            EditorGUILayout.BeginHorizontal();
            spawn.weatherFilterFoldoutState = EditorGUILayout.Foldout(spawn.weatherFilterFoldoutState, "Weather Filters (" + spawn.weatherRules.Count + ")", true);
            bool buttonAddWeatherRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.weatherFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.weatherRules.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    var weatherRule = spawn.weatherRules[i];
                    EditorUtils.SaveLabelWidth();
                    EditorUtils.SetLabelWidth("Condition:          ");
                    weatherRule.weatherCondition = (WeatherCondition)EditorGUILayout.Popup("Condition: ", (int)weatherRule.weatherCondition,
                        Utility.WeatherConditionNames.ToArray(), GUILayout.Width(200));

                    if (weatherRule.weatherCondition != WeatherCondition.Temperature)
                    {
                        EditorUtils.SetLabelWidth("Rule:          ");
                        weatherRule.weatherRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", weatherRule.weatherRule);

                        EditorUtils.SetLabelWidth("Threshhold:          ");
                        weatherRule.threshold = EditorGUILayout.FloatField("Threshold: ", weatherRule.threshold, GUILayout.Width(164));
                    } // if

                    // Temperature data here
                    if (weatherRule.weatherCondition == WeatherCondition.Temperature)
                    {
                        EditorUtils.SetLabelWidth("Min Temp:          ");
                        weatherRule.minTemperature = EditorGUILayout.FloatField("Min Temp: ", weatherRule.minTemperature, GUILayout.Width(130));

                        EditorUtils.SetLabelWidth("Max Temp:          ");
                        weatherRule.maxTemperature = EditorGUILayout.FloatField("Max Temp: ", weatherRule.maxTemperature, GUILayout.Width(130));
                    } // if
                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    if (buttonDeleteRule)
                    {
                        spawn.weatherRules.RemoveAt(i);
                        if (spawn.weatherRules.Count == 0)
                        {
                            spawn.weatherFilterFoldoutState = false;
                        } // if
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach weather rule

                EditorGUILayout.EndVertical();
            } // if foldout is true

            if (buttonAddWeatherRule)
            {
                spawn.weatherRules.Add(new WeatherRuleData()
                {
                    weatherCondition = WeatherCondition.Temperature,
                    weatherRule = (FilterRule)spawn.weatherRuleSelected,
                    threshold = 0.5f,
                    minTemperature = -50f,
                    maxTemperature = 100f
                });

                spawn.weatherFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // WeatherFiltersGUI()

        /// <summary>
        /// Edit season filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void SeasonFiltersGUI(SpawnInfoData spawn)
        {
            EditorGUILayout.BeginHorizontal();
            spawn.seasonFilterFoldoutState = EditorGUILayout.Foldout(spawn.seasonFilterFoldoutState, "Season Filters (" + spawn.seasonRules.Count + ")", true);
            var buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.seasonFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.seasonRules.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    var seasonRule = spawn.seasonRules[i];
                    EditorUtils.SaveLabelWidth();
                    EditorUtils.SetLabelWidth("Season:           ");

                    int seasonIndex = _seasonNames.IndexOf(seasonRule.season.ToString());
                    seasonIndex = EditorGUILayout.Popup("Season: ", seasonIndex, _seasonNames.ToArray());
                    if (seasonIndex != -1) seasonRule.season = (Season)Enum.Parse(typeof(Season), _seasonNames[seasonIndex]);

                    EditorUtils.SetLabelWidth("Rule:           ");
                    seasonRule.seasonRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", seasonRule.seasonRule);

                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    if (buttonDeleteRule)
                    {
                        spawn.seasonRules.RemoveAt(i);
                        if (spawn.seasonRules.Count == 0)
                        {
                            spawn.seasonFilterFoldoutState = false;
                        } // if
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach season rule
                EditorGUILayout.EndVertical();
            } // if foldout

            if (buttonAddRule)
            {
                spawn.seasonRules.Add(new SeasonRuleData()
                {
                    seasonRule = (FilterRule)spawn.seasonRuleSelected
                });

                spawn.seasonFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // SeasonFiltersGUI()

        /// <summary>
        /// Edit global snow filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void GlobalSnowFiltersGUI(SpawnInfoData spawn)
        {
            EditorGUILayout.BeginHorizontal();
            spawn.globalFilterSnowFoldoutState = EditorGUILayout.Foldout(spawn.globalFilterSnowFoldoutState, "Global Snow Filter (" + spawn.globalSnowRules.Count + ")", true);

            bool buttonAddSnowRule = false;
            if (spawn.globalSnowRules.Count == 0) // only allowed to add a single rule
            {
                buttonAddSnowRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            } // if

            EditorGUILayout.EndHorizontal();

            if (spawn.globalFilterSnowFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.globalSnowRules.Count; ++i)
                {
                    var snowRule = spawn.globalSnowRules[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorUtils.SetLabelWidth("Rule:          ");
                    snowRule.filterRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", snowRule.filterRule);

                    EditorUtils.SetLabelWidth("Threshhold:          ");
                    snowRule.threshold = EditorGUILayout.FloatField("Threshold: ", snowRule.threshold, GUILayout.Width(164));
                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    if (buttonDeleteRule)
                    {
                        spawn.globalSnowRules.RemoveAt(i);
                        if (spawn.globalSnowRules.Count == 0)
                        {
                            spawn.globalFilterSnowFoldoutState = false;
                        } // if
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // if
                EditorGUILayout.EndVertical();
            } // if foldout is true

            if (buttonAddSnowRule)
            {
                spawn.globalSnowRules.Add(new GlobalSnowRuleData()
                { filterRule = (FilterRule)spawn.snowRuleSelected, threshold = _globalSnowThresholdValue });

                spawn.texFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // GlobalSnowFiltersGUI()

        /// <summary>
        /// Edit quest filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void QuestFiltersGUI(SpawnInfoData spawn)
        {
            if (Utility.QuestManager == null) return;

            EditorGUILayout.BeginHorizontal();
            spawn.questFilterFoldoutState = EditorGUILayout.Foldout(spawn.questFilterFoldoutState, "Quest Filters (" + spawn.questRules.Count + ")", true);
            var buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.questFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.questRules.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();

                    var questRule = spawn.questRules[i];
                    EditorUtils.SaveLabelWidth();
                    EditorUtils.SetLabelWidth("Quest:           ");

                    int questIndex = GetQuestIndex(questRule.questName);
                    questIndex = EditorGUILayout.Popup("Quest: ", questIndex, Utility.QuestNames.ToArray());
                    if (questIndex != -1) questRule.questName = Utility.QuestNames[questIndex];

                    EditorUtils.SetLabelWidth("Rule:           ");
                    questRule.questRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", questRule.questRule);

                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    if (buttonDeleteRule)
                    {
                        spawn.questRules.RemoveAt(i);
                        if (spawn.questRules.Count == 0)
                        {
                            spawn.questFilterFoldoutState = false;
                        } // if
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach quest rule
                EditorGUILayout.EndVertical();
            } // if foldout

            if (buttonAddRule)
            {
                spawn.questRules.Add(new QuestRuleData()
                {
                    questRule = (FilterRule)spawn.questRuleSelected
                });

                spawn.questFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // QuestFiltersGUI()

        /// <summary>
        /// Edit proximity filter rule
        /// </summary>
        /// <param name="spawn"></param>
        void ProximityFiltersGUI(SpawnInfoData spawn)
        {
            EditorGUILayout.BeginHorizontal();
            spawn.proximityFilterFoldoutState = EditorGUILayout.Foldout(spawn.proximityFilterFoldoutState, "Proximity Filters (" + spawn.proximityRules.Count + ")", true);
            bool buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.proximityFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < spawn.proximityRules.Count; ++i)
                {
                    EditorGUILayout.BeginVertical();
                    if (spawn.proximityRules[i].spawnPrefab != null)
                    {
                        EditorGUILayout.LabelField("[" + spawn.proximityRules[i].spawnPrefab.name + "]");
                    }

                    EditorGUILayout.BeginHorizontal();

                    var proximityRule = spawn.proximityRules[i];
                    EditorUtils.SaveLabelWidth();
                    EditorUtils.SetLabelWidth("Prefab:          ");
                    proximityRule.spawnPrefab = EditorGUILayout.ObjectField("Prefab", proximityRule.spawnPrefab, typeof(Transform), false) as Transform;
                    if (proximityRule.spawnPrefab != null)
                    {
                        var spawnable = proximityRule.spawnPrefab.GetComponent<ISpawnable>();
                        if (spawnable == null)
                        {
                            EditorGUILayout.HelpBox("The prefab you have selected does not have an ISpawnable interface.", MessageType.Error);
                        }
                    }

                    EditorUtils.SetLabelWidth("Rule:          ");
                    proximityRule.proximityRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", proximityRule.proximityRule);

                    EditorUtils.SetLabelWidth("Range:          ");
                    proximityRule.range = EditorGUILayout.FloatField("Range: ", proximityRule.range, GUILayout.Width(164));

                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                    if (buttonDeleteRule)
                    {
                        spawn.proximityRules.RemoveAt(i);
                        if (spawn.proximityRules.Count == 0)
                        {
                            spawn.proximityFilterFoldoutState = false;
                        } // if
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach proximity rule

                EditorGUILayout.EndVertical();
            } // if foldout is true

            if (buttonAddRule)
            {
                spawn.proximityRules.Add(new ProximityRuleData()
                {
                    proximityRule = (FilterRule)spawn.proximityRuleSelected,
                    range = 15f,
                });

                spawn.proximityFilterFoldoutState = true; // automatically open foldout for editing the new filter
            } // if
        } // WeatherFiltersGUI()

        /// <summary>
        /// Get the index for the specified quest name
        /// </summary>
        /// <param name="questName"></param>
        /// <returns></returns>
        int GetQuestIndex(string questName)
        {
            return Utility.QuestNames.IndexOf(questName);
        } // GetQuestIndex()

        bool ProximitySpawnsGUI(SpawnInfoData spawn)
        {
            bool dbChanged = false;
            if (_database == null || spawn.spawnGuid == null) return dbChanged;

            // Get count of proximity spawns from database by spawnGuid
            var proximitySpawnCount = _database.GetProximitySpawnsCount(spawn.spawnGuid);
            var proximitySpawns = _database.GetProximitySpawns(spawn.spawnGuid);

            EditorGUILayout.BeginHorizontal();
            spawn.proximitySpawnsFoldoutState = EditorGUILayout.Foldout(spawn.proximitySpawnsFoldoutState, "Proximity Spawns (" + proximitySpawnCount + ")", true);
            var buttonAddSpawn = GUILayout.Button("Add Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.MEDIUM_BTN_SIZE));
            EditorGUILayout.EndHorizontal();

            if (spawn.proximitySpawnsFoldoutState && proximitySpawns != null)
            {
                EditorUtils.BeginBgndStyleDark();

                List<ProximitySpawnData> removeSpawns = new List<ProximitySpawnData>();

                foreach (var proximitySpawn in proximitySpawns)
                {
                    SpawnInfoDataEditorHelper helper = new SpawnInfoDataEditorHelper();
                    helper.AssignTarget(_myTarget, _autoAddSpawnable, _spawnableSettings);

                    bool moveUp = false;
                    bool moveDown = false;
                    bool remove = false;
                    if (helper.EditSpawn(proximitySpawn.spawnInfoData, out moveUp, out moveDown, out remove, false))
                    {
                        if (moveUp)
                        {
                            Utility.MoveItemUp(proximitySpawns, proximitySpawns.IndexOf(proximitySpawn));
                        } // if

                        if (moveDown)
                        {
                            Utility.MoveItemDown(proximitySpawns, proximitySpawns.IndexOf(proximitySpawn));
                        } // if

                        if (remove)
                        {
                            removeSpawns.Add(proximitySpawn);
                        } // if

                        EditorUtility.SetDirty(_myTarget.Database);
                        if (moveUp || moveDown) GUIUtility.ExitGUI();
                    } // if

                    if (!proximitySpawn.spawnInfoData.hideSpawn)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("--Proximity Spawning Range--");
                        proximitySpawn.minRange = EditorGUILayout.FloatField("Min Range", proximitySpawn.minRange);
                        proximitySpawn.maxRange = EditorGUILayout.FloatField("Max Range", proximitySpawn.maxRange);

                        EditorUtils.HorizontalLine(Color.gray, 2f, Vector3.one);
                        EditorGUILayout.Separator();
                    } // if !hidden
                } // foreach

                EditorGUILayout.EndVertical();

                // Remove items as necessary
                foreach (var proximitySpawn in removeSpawns)
                {
                    _database.RemoveProximitySpawn(spawn.spawnGuid, proximitySpawn);
                    dbChanged = true;
                } // foreach
            } // if foldout

            if (buttonAddSpawn)
            {
                _database.AddProximitySpawn(spawn.spawnGuid,
                    new ProximitySpawnData()
                    {
                        minRange = 5f,
                        maxRange = 30f,
                        spawnInfoData = new SpawnInfoData()
                        {
                            spawnGuid = string.Empty,
                            populationCap = _myTarget.DefaultPopulationCap
                        }
                    });
                
                dbChanged = true;
                spawn.proximitySpawnsFoldoutState = true; // automatically open foldout for editing the new filter
            } // if

            return dbChanged;
        } // ProximitySpawnsGUI()
    } // class SpawnInfoDataEditorHelper
} // namespace Magique.SoulLink
