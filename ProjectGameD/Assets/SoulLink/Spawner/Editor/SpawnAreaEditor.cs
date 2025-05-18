using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if SOULLINK_USE_DS || SOULLINK_USE_QM
using PixelCrushers;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SpawnArea), true), CanEditMultipleObjects]
    public class SpawnAreaEditor : Editor
    {
        private SpawnArea _myTarget;
        private SerializedObject _mySerializedObject;
        private SoulLinkSpawner _spawner;

        private int _todSelected = 0;

        // bool
        SerializedProperty DistributeHerdsProp, BuildNavMeshProp, ResetAreaProp, OverrideTriggerRadiusProp, SuppressBiomeSpawnsProp, ManualTriggerProp,
            SpawnBossProp, UseLocalSpawnDataProp, MarkSpawnsAsPersistentProp, AutoTriggerOnResetProp;

        // float
        SerializedProperty MinSpawnRangeProp, MaxSpawnsProp, SpawnRadiusProp, DelaySpawnsProp, TriggerRadiusProp, ResetDurationProp, SuppressionRadiusProp,
            ClusterRangeProp, SpawnIntervalProp;

        // int
        SerializedProperty MaxResetsProp, MinChangeMaxProp, MaxChangeMaxProp, SpawnBossWaveNumberProp;

        // string
        SerializedProperty NameProp;

        // enum

        // object
        SerializedProperty OnTriggeredEventProp, OnStartSpawningEventProp, OnSpawningCompleteEventProp, OnWaveCompleteEventProp, OnSpawnBossEventProp, SpawnMethodProp,
            ResetTypeProp, ResetRepeatTypeProp, BossPrefabProp, BossSpawnPointProp, SplinePositioningProp;

        static private bool _showEventsFoldoutState = true;
        static private bool _spawnPointsFoldoutState = false;

        private SpawnAreaBounds _spawnAreaBoundsPrefab;


        [MenuItem("GameObject/SoulLink/Spawn Area", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Spawn Area");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            SceneView.lastActiveSceneView.MoveToView();
            var spawnArea = go.AddComponent<SpawnArea>();

            // Check if save system is in scene and add the saver component automatically if there
#if SOULLINK_USE_DS || SOULLINK_USE_QM
            var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
            if (saveSystem.Length != 0)
            {
                spawnArea.gameObject.AddComponent<SpawnArea_Saver>();
            } // if
#endif
        } // CreateCustomGameObject()

        private void OnEnable()
        {
            _myTarget = (SpawnArea)target;
            _mySerializedObject = new SerializedObject(targets);

            _spawner = FindObjectOfType<SoulLinkSpawner>();
            if (!VerifySpawnerSetup()) return;

            // bool property initializers
            DistributeHerdsProp = _mySerializedObject.FindProperty("_distributeHerdsFromCenter");
            BuildNavMeshProp = _mySerializedObject.FindProperty("_buildNavMeshAfterSpawning");
            ResetAreaProp = _mySerializedObject.FindProperty("_resetArea");
            OverrideTriggerRadiusProp = _mySerializedObject.FindProperty("_overrideTriggerRadius");
            SuppressBiomeSpawnsProp = _mySerializedObject.FindProperty("_suppressBiomeSpawns");
            ManualTriggerProp = _mySerializedObject.FindProperty("_manualTrigger");
            SpawnBossProp = _mySerializedObject.FindProperty("_spawnBoss");
            MarkSpawnsAsPersistentProp = _mySerializedObject.FindProperty("_markSpawnsAsPersistent");
            UseLocalSpawnDataProp = _mySerializedObject.FindProperty("_useLocalSpawnData");

            // float property initializers
            SpawnRadiusProp = _mySerializedObject.FindProperty("_spawnRadius");
            MinSpawnRangeProp = _mySerializedObject.FindProperty("_minimumSpawnRange");
            MaxSpawnsProp = _mySerializedObject.FindProperty("_maxSpawns");
            DelaySpawnsProp = _mySerializedObject.FindProperty("_delaySpawns");
            TriggerRadiusProp = _mySerializedObject.FindProperty("_triggerRadius");
            ResetDurationProp = _mySerializedObject.FindProperty("_resetDuration");
            SuppressionRadiusProp = _mySerializedObject.FindProperty("_suppressionRadius");
            ClusterRangeProp = _mySerializedObject.FindProperty("_clusterRange");
            SpawnIntervalProp = _mySerializedObject.FindProperty("_spawnInterval");

            // int property initializers
            MaxResetsProp = _mySerializedObject.FindProperty("_maxResets");
            MinChangeMaxProp = _mySerializedObject.FindProperty("_minChangeMaxSpawns");
            MaxChangeMaxProp = _mySerializedObject.FindProperty("_maxChangeMaxSpawns");
            SpawnBossWaveNumberProp = _mySerializedObject.FindProperty("_spawnBossOnWaveNumber");

            // string property initializers
            NameProp = _mySerializedObject.FindProperty("_areaName");

            // enum property initializers

            // object property initializers
            OnTriggeredEventProp = _mySerializedObject.FindProperty("_onTriggeredEventHandler");
            OnStartSpawningEventProp = _mySerializedObject.FindProperty("_onStartSpawningEventHandler");
            OnSpawningCompleteEventProp = _mySerializedObject.FindProperty("_onSpawningCompleteEventHandler");
            OnWaveCompleteEventProp = _mySerializedObject.FindProperty("_onWaveCompleteEventHandler");
            OnSpawnBossEventProp = _mySerializedObject.FindProperty("_onSpawnBossEventHandler");
            SpawnMethodProp = _mySerializedObject.FindProperty("_spawningMethod");
            SplinePositioningProp = _mySerializedObject.FindProperty("_splinePositioning");
            ResetTypeProp = _mySerializedObject.FindProperty("_resetType");
            ResetRepeatTypeProp = _mySerializedObject.FindProperty("_resetRepeatType");
            AutoTriggerOnResetProp = _mySerializedObject.FindProperty("_autoTriggerOnReset");
            BossPrefabProp = _mySerializedObject.FindProperty("_bossPrefab");
            BossSpawnPointProp = _mySerializedObject.FindProperty("_bossSpawnPoint");

            _spawnAreaBoundsPrefab = Resources.Load<SpawnAreaBounds>("SpawnAreaBounds");

            Utility.FindSpawnerComponents();
            Utility.InitTODNames();
            Utility.InitTextureNames();
            Utility.InitWeatherConditionNames();
            Utility.InitQuestData();
        } // OnEnable()

        bool VerifySpawnerSetup()
        {
            return (_spawner != null);
        } // VerifySpawnerSetup()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            if (!VerifySpawnerSetup())
            {
                EditorGUILayout.LabelField("The scene does not have a SoulLinkSpawner component. Did you forget to run Setup Scene from Tools/SoulLink/Spawner?", EditorUtils.guiStyle_Help);

                EditorGUILayout.EndVertical();
                return;
            } // if

            EditorGUILayout.PropertyField(NameProp);
            //EditorGUILayout.LabelField(_myTarget.SpawnAreaGuid); // Uncomment to see the Guid in the inspector

            EditorGUILayout.PropertyField(ManualTriggerProp);
            if (!ManualTriggerProp.boolValue)
            {
                EditorGUILayout.PropertyField(OverrideTriggerRadiusProp);
                if (OverrideTriggerRadiusProp.boolValue)
                {
                    EditorGUILayout.PropertyField(TriggerRadiusProp);
                } // if
            } // if

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(SpawnMethodProp);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            if (SpawnMethodProp.enumValueIndex == (int)SpawningMethod.SpawnRadius)
            {
                EditorGUILayout.PropertyField(SpawnRadiusProp);
                if (SpawnRadiusProp.floatValue < MinSpawnRangeProp.floatValue)
                {
                    EditorGUILayout.HelpBox("Spawn Radius can't be smaller than the Minimum Spawn Range.", MessageType.Error);
                } // if
                EditorGUILayout.PropertyField(MinSpawnRangeProp);
                if (MinSpawnRangeProp.floatValue > SpawnRadiusProp.floatValue)
                {
                    EditorGUILayout.HelpBox("Minimum Spawn Range can't be larger than the Spawn Radius.", MessageType.Error);
                } // if
            }
            else if (SpawnMethodProp.enumValueIndex == (int)SpawningMethod.SpawnBounds)
            {
                if (Selection.objects.Length == 1)
                {
                    for (int i = 0; i < _myTarget.SpawnAreaBounds.Count; ++i)
                    {
                        EditorGUILayout.LabelField("[Bounds " + (i + 1).ToString() + "]");
                        EditorGUILayout.BeginHorizontal();
                        _myTarget.SpawnAreaBounds[i] = (SpawnAreaBounds)EditorGUILayout.ObjectField(_myTarget.SpawnAreaBounds[i], typeof(SpawnAreaBounds), true);
                        bool buttonRemoveBounds = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                        if (buttonRemoveBounds)
                        {
                            DestroyImmediate(_myTarget.SpawnAreaBounds[i].gameObject);
                            _myTarget.SpawnAreaBounds.RemoveAt(i);
                        } // if
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                    } // for i

                    bool buttonAddBounds = GUILayout.Button("Add Bounds", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                    if (buttonAddBounds)
                    {
                        var spawnAreaBounds = Instantiate(_spawnAreaBoundsPrefab, _myTarget.transform);
                        _myTarget.SpawnAreaBounds.Add(spawnAreaBounds);
                    } // if
/*
                    bool buttonOldAddBounds = GUILayout.Button("Add Old Bounds", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                    if (buttonOldAddBounds)
                    {
                        var newBounds = new Bounds();
                        newBounds.center = _myTarget.transform.position + new Vector3(0f, 0.5f, 0f);
                        newBounds.size = Vector3.one;
                        _myTarget.SpawnBounds.Add(newBounds);
                    } // if

                    bool buttonConvert = GUILayout.Button("Convert", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                    if (buttonConvert)
                    {
                        ConvertFromOldBounds();
                    }
*/
                }
                else
                {
                    EditorGUILayout.LabelField("Cannot multi-edit bounds property", EditorUtils.guiStyle_Help);
                }
            }
            else if (SpawnMethodProp.enumValueIndex == (int)SpawningMethod.SpawnPoints)
            {
                if (Selection.objects.Length == 1)
                {
                    _spawnPointsFoldoutState = EditorGUILayout.Foldout(_spawnPointsFoldoutState, "Spawn Points (" + _myTarget.SpawnPoints.Count + ")", true);
                    if (_spawnPointsFoldoutState)
                    {
                        _myTarget.ConformPosition = EditorGUILayout.Toggle("Conform Position", _myTarget.ConformPosition);
                        _myTarget.ConformRotation = EditorGUILayout.Toggle("Conform Rotation", _myTarget.ConformRotation);
                        EditorGUILayout.Space();

                        for (int i = 0; i < _myTarget.SpawnPoints.Count; ++i)
                        {
                            EditorGUILayout.LabelField("[Point " + (i + 1).ToString() + "]");
                            EditorGUILayout.BeginHorizontal();
                            _myTarget.SpawnPoints[i].transform = (Transform)EditorGUILayout.ObjectField("Transform", _myTarget.SpawnPoints[i].transform, typeof(Transform), true);
                            bool buttonRemovePoint = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                            EditorGUILayout.EndHorizontal();

                            _myTarget.SpawnPoints[i].useRadius = EditorGUILayout.Toggle("Use Spawn Radius", _myTarget.SpawnPoints[i].useRadius);
                            if (_myTarget.SpawnPoints[i].useRadius)
                            {
                                EditorGUI.indentLevel++;

                                _myTarget.SpawnPoints[i].spawnRadius = EditorGUILayout.FloatField("Spawn Radius", _myTarget.SpawnPoints[i].spawnRadius);

                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.Separator();

                            if (buttonRemovePoint)
                            {
                                _myTarget.SpawnPoints.RemoveAt(i);
                            } // if
                        } // for i

                        bool buttonAddPoint = GUILayout.Button("Add Point", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                        if (buttonAddPoint)
                        {
                            var newPoint = new SpawnArea.SpawnPoint()
                            {
                                transform = null,
                                useRadius = false,
                                spawnRadius = 1f
                            };

                            _myTarget.SpawnPoints.Add(newPoint);
                        } // if
                    } // if
                }
                else
                {
                    EditorGUILayout.LabelField("Cannot multi-edit points property", EditorUtils.guiStyle_Help);
                }
            }
            else if (SpawnMethodProp.enumValueIndex == (int)SpawningMethod.Spline)
            {
                _myTarget.Spline = (BaseSpline)EditorGUILayout.ObjectField(_myTarget.Spline, typeof(BaseSpline), true);
                EditorGUILayout.PropertyField(SplinePositioningProp);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(ClusterRangeProp);
            EditorGUILayout.PropertyField(MaxSpawnsProp);
            EditorGUILayout.PropertyField(DelaySpawnsProp);
            EditorGUILayout.PropertyField(DistributeHerdsProp);
            EditorGUILayout.PropertyField(MarkSpawnsAsPersistentProp);
            EditorGUILayout.PropertyField(SpawnIntervalProp);
            EditorGUILayout.PropertyField(SuppressBiomeSpawnsProp);
            if (SuppressBiomeSpawnsProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(SuppressionRadiusProp);
                EditorGUI.indentLevel--;
            } // if
#if SOULLINK_USE_AINAV
            EditorGUILayout.PropertyField(BuildNavMeshProp);
#endif
            EditorGUILayout.PropertyField(ResetAreaProp);
            if (ResetAreaProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(AutoTriggerOnResetProp);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ResetTypeProp);
                EditorGUILayout.EndHorizontal();

                if (ResetTypeProp.intValue == (int)SpawnArea.ResetKind.GameTimeDuration || ResetTypeProp.intValue == (int)SpawnArea.ResetKind.RealtimeDuration ||
                    ResetTypeProp.intValue == (int)SpawnArea.ResetKind.Elimination)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(ResetDurationProp);
                    EditorGUILayout.PropertyField(MinChangeMaxProp);
                    EditorGUILayout.PropertyField(MaxChangeMaxProp);
                    EditorGUI.indentLevel--;
                } // if

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ResetRepeatTypeProp);
                EditorGUILayout.EndHorizontal();

                if (ResetRepeatTypeProp.intValue == (int)SpawnArea.ResetRepeatKind.UserDefined)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(MaxResetsProp);
                    EditorGUILayout.PropertyField(SpawnBossProp);
                    if (SpawnBossProp.boolValue == true)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(SpawnBossWaveNumberProp);
                        EditorGUILayout.PropertyField(BossPrefabProp);
                        EditorGUILayout.PropertyField(BossSpawnPointProp);
                        EditorGUI.indentLevel--;
                    } // if
                    EditorGUI.indentLevel--;
                } // if
                EditorGUI.indentLevel--;
            } // if

            EditorGUILayout.PropertyField(UseLocalSpawnDataProp);
            if (UseLocalSpawnDataProp.boolValue == true)
            {
                DrawAreaSpawnItemData();
            } // if

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            // Show some runtime debug information for this area
            if (Application.isPlaying)
            {
                EditorUtils.BeginBoxStyleDark();

                EditorGUILayout.LabelField("Is Enabled: ", _myTarget.Enabled.ToString());
                EditorGUILayout.LabelField("Is Valid: ", _myTarget.IsValid.ToString());
                EditorGUILayout.LabelField("Is Triggered: ", _myTarget.IsTriggered.ToString());

                EditorGUILayout.EndVertical();
            } // if

            EditorGUI.indentLevel++;
            _showEventsFoldoutState = EditorGUILayout.Foldout(_showEventsFoldoutState, "Events", true);
            if (_showEventsFoldoutState)
            {
                EditorUtils.BeginBoxStyleLight();
                EditorGUILayout.PropertyField(OnTriggeredEventProp);
                EditorGUILayout.PropertyField(OnStartSpawningEventProp);
                EditorGUILayout.PropertyField(OnSpawningCompleteEventProp);
                EditorGUILayout.PropertyField(OnWaveCompleteEventProp);
                EditorGUILayout.PropertyField(OnSpawnBossEventProp);
                EditorGUILayout.EndVertical();
            } // if foldout
            EditorGUI.indentLevel--;

            if (GUI.changed)
            {
                // Writing changes of the SpawnArea into Undo
                Undo.RecordObject(_myTarget, "SpawnArea Editor Modify");

                EditorUtility.SetDirty(_myTarget);
                _mySerializedObject.ApplyModifiedProperties();
            }
        } // OnInspectorGUI()

        void DrawAreaSpawnItemData()
        {
            if (_spawner.Database == null)
            {
                EditorGUILayout.LabelField("Spawner has no database assigned. Please assign a database to edit spawn data.", EditorUtils.guiStyle_Help);
                return;
            } // if

            Utility.InitTODNames();

            EditorGUILayout.Separator();

            string errorText = "";

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();

            _todSelected = EditorGUILayout.Popup("Pick a Time of Day", _todSelected, Utility.TimeOfDayNames.ToArray());

            bool buttonAdd = GUILayout.Button("Add", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(36));
            if (buttonAdd)
            {
                // Cannot be duplicate
                if (!_myTarget.TimeOfDaySpawns.ContainsKey(Utility.TimeOfDayNames[_todSelected]))
                {
                    _myTarget.TimeOfDaySpawns[Utility.TimeOfDayNames[_todSelected]] = new SpawnListData();
                }
                else
                {
                    errorText = "Cannot add Time of Day Spawns for '" + Utility.TimeOfDayNames[_todSelected] + "'. Time of Day is a duplicate";
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

            if (_myTarget.TimeOfDaySpawns.Count > 0)
            {
                DrawAreaSpawnInfoItemsGUI();
                EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
            }
            else
            {
                EditorGUILayout.LabelField("There are no Time of Day entries.");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        } // DrawAreaSpawnItemData()

        void DrawAreaSpawnInfoItemsGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            List<string> removeTODs = new List<string>();
            Dictionary<string, string> keyChanges = new Dictionary<string, string>();

            int count = 0;
            int todCount = _myTarget.TimeOfDaySpawns.Count;
            foreach (var tod in _myTarget.TimeOfDaySpawns)
            {
                List<SpawnInfoData> removeSpawns = new List<SpawnInfoData>();

                var rangeStr = _spawner.Database.timeOfDayData.Find(e => e.timeOfDayName == tod.Key).GetRangeString();
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

                foreach (var spawn in tod.Value.spawnInfoData)
                {
                    SpawnInfoDataEditorHelper helper = new SpawnInfoDataEditorHelper();
                    helper.AssignTarget(_spawner, _spawner.AutoAddSpawnableInterface, _spawner.SpawnableSettings);
                    helper.AssignDatabase(_spawner.Database);

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
                                _myTarget.TimeOfDaySpawns[tod.Key].spawnInfoData.Remove(spawn);
                            } // if
                        } // if

                        EditorUtility.SetDirty(_spawner.Database);
                        if (moveUp || moveDown || remove) GUIUtility.ExitGUI();
                    } // if

                    EditorUtils.HorizontalLine(EditorUtils.colorDarkGray, 2f, Vector3.one);
                    EditorGUILayout.Separator();
                } // foreach spawn

                EditorGUILayout.BeginHorizontal();

                bool buttonAdd = GUILayout.Button("Add Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
                if (buttonAdd)
                {
                    var newSpawn = new SpawnInfoData();
                    newSpawn.spawnGuid = Guid.NewGuid().ToString();
                    newSpawn.populationCap = _spawner.DefaultPopulationCap;
                    _myTarget.TimeOfDaySpawns[tod.Key].spawnInfoData.Add(newSpawn);
                } // if
                EditorGUILayout.EndHorizontal();

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
                ChangeTimeOfDayKey(item.Key, item.Value);
            } // foreach

            // Remove items here from time of day
            foreach (var todKey in removeTODs)
            {
                _myTarget.TimeOfDaySpawns.Remove(todKey);
            } // foreach

            EditorGUILayout.EndVertical();
        } // DrawAreaSpawnInfoItemsGUI()

        void ChangeTimeOfDayKey(string oldKey, string newKey)
        {
            // Check if the new key is already in the database
            if (_myTarget.TimeOfDaySpawns.ContainsKey(newKey))
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "Time of Day Entry already exists for " + newKey,
                    "OK"))
                {
                } // if

                return;
            } // if

            SpawnListData spawnListData = _myTarget.TimeOfDaySpawns[oldKey];
            _myTarget.TimeOfDaySpawns.Remove(oldKey);
            _myTarget.TimeOfDaySpawns.Add(newKey, spawnListData);
        } // ChangeTimeOfDayKey();

        private void OnSceneGUI()
        {
            var is2D = false;
            if (Camera.main != null)
            {
                is2D = (Camera.main.orthographic);
            } // if

            if (_myTarget.OverrideTriggerRadius && !_myTarget.ManualTrigger)
            {
                Handles.color = new Color(0f, 1f, 0f, 0.1f);
                Handles.DrawSolidDisc(_myTarget.transform.position, is2D ? Vector3.forward : Vector3.up, _myTarget.TriggerRadius);
            } // if

            if (_myTarget.SpawnMethod == SpawningMethod.SpawnRadius)
            {
                if (_myTarget.SpawnRadius > 0f)
                {
                    Handles.color = new Color(0f, 0f, 1f, 0.1f);
                    Handles.DrawSolidDisc(_myTarget.transform.position, is2D ? Vector3.forward : Vector3.up, _myTarget.SpawnRadius);
                    Handles.color = new Color(1f, 0f, 0f, 0.1f);
                    Handles.DrawSolidDisc(_myTarget.transform.position, is2D ? Vector3.forward : Vector3.up, _myTarget.MinimumSpawnRange);
                }
            }
            else if (_myTarget.SpawnMethod == SpawningMethod.SpawnPoints)
            {
                // Draw each spawn point; with radius if necessary
                foreach (var point in _myTarget.SpawnPoints)
                {
                    if (point.useRadius)
                    {
                        Handles.color = new Color(0f, 0f, 1f, 0.1f);
                        Handles.DrawSolidDisc(point.transform.position, is2D ? Vector3.forward : Vector3.up, point.spawnRadius);
                    } // if
                } // foreach
            }
        } // OnSceneGUI()
    } // class SpawnAreaEditor
} // namespace Magique.SoulLink
