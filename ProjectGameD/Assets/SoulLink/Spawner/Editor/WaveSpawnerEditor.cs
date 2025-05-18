using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using static Magique.SoulLink.SpawnArea;
using static Magique.SoulLink.WaveItemData;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(WaveSpawner), true)]
    public class WaverSpawnerEditor : Editor
    {
        private WaveSpawner _myTarget;
        private SerializedObject _mySerializedObject;

        private SoulLinkSpawner _spawner;

        private int _selectedTOD = 0;
        static private bool _showEventsFoldoutState = false;

        private SpawnAreaBounds _spawnAreaBoundsPrefab;

        // bool
        SerializedProperty DistributeHerdsProp, RepeatWaveProp, StartPausedProp, MarkSpawnsAsPersistentProp, ResetWaveOnNextDayProp;

        // float
        SerializedProperty RepeatDelayProp;

        // int
        SerializedProperty DelaySpawnsProp, NumberOfRepeatsProp;

        // string
        SerializedProperty WaveNameProp, TimeOfDayProp;

        // enum

        // object/other
        SerializedProperty RepeatTypeProp, OnWaveStartedEventProp, OnWaveCompletedEventProp, OnWaveItemRepeatStartedEventProp, OnWaveItemRepeatCompletedEventProp;

        private bool _createNewBounds = true;

        private void OnEnable()
        {
            _myTarget = (WaveSpawner)target;
            _mySerializedObject = new SerializedObject(target);

            // bool property initializers
            DistributeHerdsProp = _mySerializedObject.FindProperty("_distributeHerdsFromCenter");
            RepeatWaveProp = _mySerializedObject.FindProperty("_repeatWave");
            StartPausedProp = _mySerializedObject.FindProperty("_startPaused");
            MarkSpawnsAsPersistentProp = _mySerializedObject.FindProperty("_markSpawnsAsPersistent");
            ResetWaveOnNextDayProp = _mySerializedObject.FindProperty("_resetWaveOnNextDay");

            // float property initializers
            RepeatDelayProp = _mySerializedObject.FindProperty("_repeatDelay");

            // int property initializers
            DelaySpawnsProp = _mySerializedObject.FindProperty("_delaySpawns");
            NumberOfRepeatsProp = _mySerializedObject.FindProperty("_numberOfRepeats");

            // string property initializers
            WaveNameProp = _mySerializedObject.FindProperty("_waveName");
            TimeOfDayProp = _mySerializedObject.FindProperty("_timeOfDay");

            // enum property initializers

            // object property initializers
            OnWaveStartedEventProp = _mySerializedObject.FindProperty("_onWaveStartedEventHandler");
            OnWaveCompletedEventProp = _mySerializedObject.FindProperty("_onWaveCompletedEventHandler");
            OnWaveItemRepeatStartedEventProp = _mySerializedObject.FindProperty("_onWaveItemRepeatStartedEventHandler");
            OnWaveItemRepeatCompletedEventProp = _mySerializedObject.FindProperty("_onWaveItemRepeatCompletedEventHandler");

            RepeatTypeProp = _mySerializedObject.FindProperty("_repeatType");
            _spawner = FindObjectOfType<SoulLinkSpawner>();

            Utility.FindSpawnerComponents();
            Utility.InitTODNames();
            Utility.InitTextureNames();
            Utility.InitWeatherConditionNames();
            Utility.InitQuestData();

            _spawnAreaBoundsPrefab = Resources.Load<SpawnAreaBounds>("SpawnAreaBounds");
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

            DrawWaveSpawnerGUI();
            DrawWaveItemsGUI();

            EditorGUI.indentLevel++;
            _showEventsFoldoutState = EditorGUILayout.Foldout(_showEventsFoldoutState, "Wave Events", true);
            if (_showEventsFoldoutState)
            {
                EditorGUILayout.PropertyField(OnWaveStartedEventProp);
                EditorGUILayout.PropertyField(OnWaveCompletedEventProp);
                EditorGUILayout.PropertyField(OnWaveItemRepeatStartedEventProp);
                EditorGUILayout.PropertyField(OnWaveItemRepeatCompletedEventProp);
            } // if
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        /// <summary>
        /// Draw the main data for the wave spawner
        /// </summary>
        void DrawWaveSpawnerGUI()
        {
            bool buttonWaveMgr = GUILayout.Button("Goto Wave Manager", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonWaveMgr && _myTarget.WaveManager != null)
            {
                Selection.activeGameObject = _myTarget.WaveManager.gameObject;
            } // if

            EditorUtils.BeginBoxStyleLight();

            if (string.IsNullOrEmpty(_myTarget.GetGuid()))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Guid is missing", EditorUtils.guiStyle_Help);
                bool buttonGUID = GUILayout.Button("Generate GUID", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonGUID)
                {
                    _myTarget.GenerateGuid();
                    EditorUtility.SetDirty(_myTarget);
                } // if
                EditorGUILayout.EndHorizontal();
            } // if

            // TODO

            EditorGUILayout.PropertyField(WaveNameProp);

            if (Utility.TimeOfDayNames.Count > 0)
            {
                _selectedTOD = Utility.TimeOfDayNames.IndexOf(TimeOfDayProp.stringValue);
                if (_selectedTOD == -1) _selectedTOD = 0;
                _selectedTOD = EditorGUILayout.Popup("Time of Day", _selectedTOD, Utility.TimeOfDayNames.ToArray());
                _myTarget.TimeOfDay = Utility.TimeOfDayNames[_selectedTOD];
            }
            else
            {
                // TODO: warning that no time of days are defined
            }

            EditorGUILayout.PropertyField(ResetWaveOnNextDayProp);
            EditorGUILayout.PropertyField(StartPausedProp);
            EditorGUILayout.PropertyField(DelaySpawnsProp);
            EditorGUILayout.PropertyField(DistributeHerdsProp);
            EditorGUILayout.PropertyField(MarkSpawnsAsPersistentProp);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(RepeatWaveProp);
            if (RepeatWaveProp.boolValue == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(RepeatTypeProp);

                if (RepeatTypeProp.intValue != (int)ResetRepeatKind.Unlimited)
                {
                    EditorGUILayout.PropertyField(NumberOfRepeatsProp);
                } // if

                EditorGUILayout.PropertyField(RepeatDelayProp);
                EditorGUI.indentLevel--;
            }
            else
            {
                // clear repeat values
                NumberOfRepeatsProp.intValue = 0;
                RepeatDelayProp.floatValue = 0f;
            }

            EditorGUILayout.EndVertical();
        } // DrawWaveSpawner()

        /// <summary>
        /// Draw each wave item in this wave spawner
        /// </summary>
        void DrawWaveItemsGUI()
        {
            EditorGUI.indentLevel++;

            EditorUtils.BeginBoxStyleDark();
            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.LabelField("- - - - - Wave Items - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            if (_myTarget.WaveItems.Count > 0)
            {
                bool anyShowing = _myTarget.WaveItems.Find(e => e.hideItem == false) != null;
                bool buttonShowHideAll = GUILayout.Button(anyShowing ? "Hide All" : "Show All", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonShowHideAll)
                {
                    foreach (var waveItem in _myTarget.WaveItems)
                    {
                        waveItem.hideItem = anyShowing;
                    } // foreach

                    EditorUtility.SetDirty(_myTarget);
                } // if
            } // if

            foreach (var waveItem in _myTarget.WaveItems)
            {
                DrawGUIWaveItem(waveItem);
            } // foreach

            bool buttonAdd = GUILayout.Button("Add Wave Item", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAdd)
            {
                _myTarget.AddWaveItem();
            } // if

            EditorGUI.indentLevel--;
        } // DrawWaveItemsGUI()

        /// <summary>
        /// Draw the individual wave item with the associated spawn data
        /// </summary>
        /// <param name="waveItem"></param>
        void DrawGUIWaveItem(WaveItemData waveItem)
        {
            EditorUtils.BeginBoxStyleLight();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[" + waveItem.itemName + "]", EditorUtils.guiStyle_MediumBold);

            bool buttonDisable = GUILayout.Button(waveItem.disable ? " " : "√", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
            bool buttonUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
            bool buttonDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
            bool buttonDelete = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(28));
            bool buttonToggle = GUILayout.Button(!waveItem.hideItem ? "Hide" : "Show", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            if (buttonToggle)
            {
                waveItem.hideItem = !waveItem.hideItem;
            } // if

            if (buttonDisable)
            {
                waveItem.disable = !waveItem.disable;
            } // if

            if (buttonUp)
            {
                Utility.MoveItemUp(_myTarget.WaveItems, _myTarget.WaveItems.IndexOf(waveItem));
                GUIUtility.ExitGUI();
            } // if

            if (buttonDown)
            {
                Utility.MoveItemDown(_myTarget.WaveItems, _myTarget.WaveItems.IndexOf(waveItem));
                GUIUtility.ExitGUI();
            } // if

            EditorGUILayout.EndHorizontal();

            if (!waveItem.hideItem)
            {
                waveItem.itemName = EditorGUILayout.TextField("Item Name", waveItem.itemName);
//                EditorGUILayout.LabelField("Guid", waveItem.guid);

                waveItem.startType = (StartKind)EditorGUILayout.EnumPopup("Start Type", waveItem.startType);
                if (waveItem.startType == StartKind.Auto)
                {
                    waveItem.delayStartWave = EditorGUILayout.FloatField("Delay Start", waveItem.delayStartWave);
                }
                else
                {
                    waveItem.delayStartWave = 0f;
                }

                if (waveItem.startType == StartKind.WaveItemComplete)
                {
                    EditorGUI.indentLevel++;
                    waveItem.waveItemDependency = EditorGUILayout.TextField("Wave Item Name", waveItem.waveItemDependency);
                    EditorGUI.indentLevel--;
                } // if

                // Draw the spawn info data here
                EditorUtils.BeginBoxStyleDark();
                EditorUtils.BeginBoxStyleLight();
                EditorGUILayout.LabelField("- - - - - Spawning - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                SpawnInfoDataEditorHelper helper = new SpawnInfoDataEditorHelper();
                helper.AssignTarget(_spawner, _spawner.AutoAddSpawnableInterface, _spawner.SpawnableSettings, false);
                helper.AssignDatabase(_spawner.Database);

                bool moveUp = false; // unused in this case
                bool moveDown = false; // unused in this case
                bool remove = false; // unused in this case
                if (helper.EditSpawn(waveItem.spawnInfoData, out moveUp, out moveDown, out remove))
                {
                    EditorUtility.SetDirty(_myTarget);
                } // if

                waveItem.clusterRange = EditorGUILayout.FloatField("Cluster Range", waveItem.clusterRange);
                waveItem.minMaxSpawns = EditorGUILayout.Vector2Field(waveItem.spawnInfoData.spawnType == SpawnType.Herd ? "Min/Max Herds" : "Min/Max Spawns", waveItem.minMaxSpawns);
                waveItem.randomizeSpawnInterval = EditorGUILayout.Toggle("Randomize Spawn Interval", waveItem.randomizeSpawnInterval);
                if (waveItem.randomizeSpawnInterval)
                {
                    EditorGUI.indentLevel++;
                    waveItem.randomizeForEach = EditorGUILayout.Toggle("Randomize for Each Spawn", waveItem.randomizeForEach);
                    waveItem.spawnInterval = EditorGUILayout.FloatField("Spawn Interval Min", waveItem.spawnInterval);
                    waveItem.spawnIntervalMax = EditorGUILayout.FloatField("Spawn Interval Max", waveItem.spawnIntervalMax);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    waveItem.spawnInterval = EditorGUILayout.FloatField("Spawn Interval", waveItem.spawnInterval);
                }
                waveItem.splinePath = EditorGUILayout.ObjectField("Spline Path", waveItem.splinePath, typeof(Transform), false) as Transform;

                // Draw the remaining wave item data here
                EditorUtils.BeginBoxStyleDark();
                EditorUtils.BeginBoxStyleLight();
                EditorGUILayout.LabelField("- - - - - Trigger - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                waveItem.spawningTriggerMethod = (SpawningTriggerMethod)EditorGUILayout.EnumPopup("Spawning Trigger Method", waveItem.spawningTriggerMethod);
                if (waveItem.spawningTriggerMethod == SpawningTriggerMethod.OverrideRadius)
                {
                    EditorGUI.indentLevel++;

                    waveItem.overrideTriggerRadius = EditorGUILayout.FloatField("Override Trigger Radius", waveItem.overrideTriggerRadius);

                    EditorGUI.indentLevel--;
                } // if

                waveItem.triggerOrigin = (Transform)EditorGUILayout.ObjectField("Trigger Origin", waveItem.triggerOrigin, typeof(Transform), true);

                waveItem.spawningMethod = (SpawningMethod)EditorGUILayout.EnumPopup("Spawning Method", waveItem.spawningMethod);
                if (waveItem.spawningMethod == SpawningMethod.SpawnRadius)
                {
                    EditorGUI.indentLevel++;

                    waveItem.spawnRadius = EditorGUILayout.FloatField("Spawn Radius", waveItem.spawnRadius);
                    waveItem.minimumSpawnRange = EditorGUILayout.FloatField("Minimum Spawn Range", waveItem.minimumSpawnRange);

                    EditorGUI.indentLevel--;
                }
                else if (waveItem.spawningMethod == SpawningMethod.SpawnBounds)
                {
                    for (int i = 0; i < waveItem.spawnAreaBounds.Count; ++i)
                    {
                        EditorGUILayout.LabelField("[Bounds " + (i + 1).ToString() + "]");
                        EditorGUILayout.BeginHorizontal();
                        waveItem.spawnAreaBounds[i] = (SpawnAreaBounds)EditorGUILayout.ObjectField(waveItem.spawnAreaBounds[i], typeof(SpawnAreaBounds), true);
                        bool buttonRemoveBounds = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                        if (buttonRemoveBounds)
                        {
                            if (waveItem.spawnAreaBounds[i] != null) DestroyImmediate(waveItem.spawnAreaBounds[i].gameObject);
                            waveItem.spawnAreaBounds.RemoveAt(i);
                        } // if
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                    } // for i

                    EditorGUILayout.BeginHorizontal();
                    bool buttonAddBounds = GUILayout.Button("Add Bounds", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                    _createNewBounds = EditorGUILayout.Toggle("Create New Bounds", _createNewBounds);
                    if (buttonAddBounds)
                    {
                        if (_createNewBounds)
                        {
                            var spawnAreaBounds = Instantiate(_spawnAreaBoundsPrefab, _myTarget.transform);
                            waveItem.spawnAreaBounds.Add(spawnAreaBounds);
                        }
                        else
                        {
                            waveItem.spawnAreaBounds.Add(null);
                        }
                    } // if
                    EditorGUILayout.EndHorizontal();
                }
                else if (waveItem.spawningMethod == SpawningMethod.SpawnPoints)
                {
                    waveItem.spawnPointsFoldoutState = EditorGUILayout.Foldout(waveItem.spawnPointsFoldoutState, "Spawn Points (" + waveItem.spawnPoints.Count + ")", true);
                    if (waveItem.spawnPointsFoldoutState)
                    {
                        for (int i = 0; i < waveItem.spawnPoints.Count; ++i)
                        {
                            EditorGUILayout.LabelField("[Point " + (i + 1).ToString() + "]");
                            EditorGUILayout.BeginHorizontal();
                            waveItem.spawnPoints[i].transform = (Transform)EditorGUILayout.ObjectField("Transform", waveItem.spawnPoints[i].transform, typeof(Transform), true);
                            bool buttonRemovePoint = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                            EditorGUILayout.EndHorizontal();

                            waveItem.spawnPoints[i].useRadius = EditorGUILayout.Toggle("Use Spawn Radius", waveItem.spawnPoints[i].useRadius);
                            if (waveItem.spawnPoints[i].useRadius)
                            {
                                EditorGUI.indentLevel++;

                                waveItem.spawnPoints[i].spawnRadius = EditorGUILayout.FloatField("Spawn Radius", waveItem.spawnPoints[i].spawnRadius);

                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.Separator();

                            if (buttonRemovePoint)
                            {
                                waveItem.spawnPoints.RemoveAt(i);
                            } // if
                        } // for i

                        bool buttonAddPoint = GUILayout.Button("Add Point", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                        if (buttonAddPoint)
                        {
                            var newPoint = new SpawnPoint()
                            {
                                transform = null,
                                useRadius = false,
                                spawnRadius = 1f
                            };

                            waveItem.spawnPoints.Add(newPoint);
                        } // if
                    } // if
                }
                else if (waveItem.spawningMethod == SpawningMethod.Spline)
                {
                    EditorGUI.indentLevel++;

                    waveItem.spline = (BaseSpline)EditorGUILayout.ObjectField("Spline", waveItem.spline, typeof(BaseSpline), true);
                    waveItem.splinePositioning = (SplinePositioning)EditorGUILayout.EnumPopup("Spline Positioning", waveItem.splinePositioning);

                    EditorGUI.indentLevel--;
                }

                EditorUtils.BeginBoxStyleDark();
                EditorUtils.BeginBoxStyleLight();
                EditorGUILayout.LabelField("- - - - - Repeating - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                waveItem.repeatItem = EditorGUILayout.Toggle("Repeat Item", waveItem.repeatItem);
                if (waveItem.repeatItem)
                {
                    EditorGUI.indentLevel++;

                    var prevRepeatType = waveItem.repeatType;
                    waveItem.repeatType = (ResetRepeatKind)EditorGUILayout.EnumPopup("Repeat Type", waveItem.repeatType);
                    if (waveItem.repeatType != ResetRepeatKind.Unlimited)
                    {
                        waveItem.numberOfRepeats = EditorGUILayout.IntField("Number of Repeats", waveItem.numberOfRepeats);
                        if (prevRepeatType == ResetRepeatKind.Unlimited)
                        {
                            waveItem.numberOfRepeats = 0;
                        } // if
                    }
                    else
                    {
                        waveItem.numberOfRepeats = int.MaxValue - 1;
                    }
                    waveItem.repeatDelay = EditorGUILayout.FloatField("Repeat Delay", waveItem.repeatDelay);
                    waveItem.repeatChangeType = (RepeatChangeKind)EditorGUILayout.EnumPopup("Repeat Change Type", waveItem.repeatChangeType);
                    if (waveItem.repeatChangeType != RepeatChangeKind.NoChange)
                    {
                        waveItem.repeatMinMaxChange = EditorGUILayout.Vector2Field("Min/Max Change", waveItem.repeatMinMaxChange);
                    }
                    else
                    {
                        waveItem.repeatMinMaxChange = Vector2.zero;
                    }

                    EditorGUI.indentLevel--;
                }
                else
                {
                    waveItem.numberOfRepeats = 0;
                }

                EditorUtils.BeginBoxStyleDark();
                EditorUtils.BeginBoxStyleLight();
                EditorGUILayout.LabelField("- - - - - Completion - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                waveItem.completeType = (CompleteKind)EditorGUILayout.EnumPopup("Complete Type", waveItem.completeType);
            } // if !hide

            if (buttonDelete)
            {
                // Make sure the user really wants to do this
                if (EditorUtility.DisplayDialog(
                    "Confirm",
                    "Are you sure you want to remove this wave item?",
                    "OK", "CANCEL"))
                {
                    _myTarget.RemoveWaveItem(waveItem);
                } // if
                GUIUtility.ExitGUI();
            } // if

            EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);

            EditorGUILayout.EndVertical();
        } // DrawGUIWaveItem()

        private void OnSceneGUI()
        {
            var is2D = false;
            if (Camera.main != null)
            {
                is2D = (Camera.main.orthographic);
            } // if

            foreach (var waveItem in _myTarget.WaveItems)
            {
                if (waveItem.disable) continue;

                if (waveItem.spawningTriggerMethod == SpawningTriggerMethod.OverrideRadius)
                {
                    Handles.color = new Color(0f, 1f, 0f, 0.1f);
                    Vector3 position = waveItem.triggerOrigin != null ? waveItem.triggerOrigin.position : _myTarget.transform.position;
                    Handles.DrawSolidDisc(position, is2D ? Vector3.forward : Vector3.up, waveItem.overrideTriggerRadius);

                    if (waveItem.spawnRadius > 0f && waveItem.spawningMethod != SpawningMethod.SpawnPoints)
                    {
                        Handles.color = new Color(0f, 0f, 1f, 0.1f);
                        Handles.DrawSolidDisc(position, is2D ? Vector3.forward : Vector3.up, waveItem.spawnRadius);
                        Handles.color = new Color(1f, 0f, 0f, 0.1f);
                        Handles.DrawSolidDisc(position, is2D ? Vector3.forward : Vector3.up, waveItem.minimumSpawnRange);
                    }
                } 
                else if (waveItem.spawningTriggerMethod == SpawningTriggerMethod.GlobalRadius)
                {
                    Handles.color = new Color(0f, 1f, 0f, 0.1f);
                    Vector3 position = waveItem.triggerOrigin != null ? waveItem.triggerOrigin.position : _myTarget.transform.position;
                    Handles.DrawSolidDisc(position, is2D ? Vector3.forward : Vector3.up, _spawner.SpawnRadius);

                    if (waveItem.spawningMethod == SpawningMethod.SpawnRadius)
                    {
                        Handles.color = new Color(0f, 0f, 1f, 0.1f);
                        Handles.DrawSolidDisc(position, is2D ? Vector3.forward : Vector3.up, waveItem.spawnRadius);
                    } // if
                }

                if (waveItem.spawningMethod == SpawningMethod.SpawnPoints)
                {
                    // Draw each spawn point; with radius if necessary
                    foreach (var point in waveItem.spawnPoints)
                    {
                        if (point.useRadius)
                        {
                            Handles.color = new Color(0f, 0f, 1f, 0.1f);
                            Handles.DrawSolidDisc(point.transform.position, is2D ? Vector3.forward : Vector3.up, point.spawnRadius);
                        } // if
                    } // foreach
                }
            } // foreach waveItem
        } // OnSceneGUI()
    } // WaveSpawnerEditor
} // namespace Magique.SoulLink
