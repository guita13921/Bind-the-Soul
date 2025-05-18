using UnityEditor;
using UnityEngine;

#if SOULLINK_USE_DS || SOULLINK_USE_QM
using PixelCrushers;
#endif

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(LevelManager), true)]
    public class LevelManagerEditor : Editor
    {
        private LevelManager _myTarget;
        private SerializedObject _mySerializedObject;

        static private string _newLevelName = "";
        static private bool _showEventsFoldoutState = false;

        // bool
        SerializedProperty AutoStartProp;

        // float

        // int
        SerializedProperty StartLevelIndexProp;

        // string

        // enum

        // object
        SerializedProperty OnLevelStartedSoundProp, OnLevelCompleteSoundProp, OutputAudioMixerGroupProp, OnLevelStartedEventProp, OnLevelCompleteEventProp;

        [MenuItem("GameObject/SoulLink/Level Manager", false, 11)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Level Manager");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            SceneView.lastActiveSceneView.MoveToView();
            var levelManager = go.AddComponent<LevelManager>();

            // Check if save system is in scene and add the saver component automatically if there
#if SOULLINK_USE_DS || SOULLINK_USE_QM
            var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
            if (saveSystem.Length != 0)
            {
                levelManager.gameObject.AddComponent<LevelManager_Saver>();
            } // if
#endif
        } // CreateCustomGameObject()

        private void OnEnable()
        {
            _myTarget = (LevelManager)target;
            _mySerializedObject = new SerializedObject(target);

            // bool property initializers
            AutoStartProp = _mySerializedObject.FindProperty("_autoStart");

            // float property initializers

            // int property initializers
            StartLevelIndexProp = _mySerializedObject.FindProperty("_startLevelIndex");

            // string property initializers

            // enum property initializers

            // object property initializers
            OnLevelStartedSoundProp = _mySerializedObject.FindProperty("_onLevelStartedSound");
            OnLevelCompleteSoundProp = _mySerializedObject.FindProperty("_onLevelCompleteSound");
            OutputAudioMixerGroupProp = _mySerializedObject.FindProperty("_outputAudioMixerGroup");
            OnLevelStartedEventProp = _mySerializedObject.FindProperty("_onLevelStartedEventHandler");
            OnLevelCompleteEventProp = _mySerializedObject.FindProperty("_onLevelCompleteEventHandler");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawLevelManagerGUI();
            DrawLevelsGUI();

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            _showEventsFoldoutState = EditorGUILayout.Foldout(_showEventsFoldoutState, "Level Manager Events", true);
            if (_showEventsFoldoutState)
            {
                EditorGUILayout.PropertyField(OnLevelStartedEventProp);
                EditorGUILayout.PropertyField(OnLevelCompleteEventProp);
            } // if
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void DrawLevelManagerGUI()
        {
            EditorUtils.BeginBoxStyleDark();

            EditorGUILayout.PropertyField(AutoStartProp);
            EditorGUILayout.PropertyField(StartLevelIndexProp);
            EditorGUILayout.PropertyField(OnLevelStartedSoundProp);
            EditorGUILayout.PropertyField(OnLevelCompleteSoundProp);
            EditorGUILayout.PropertyField(OutputAudioMixerGroupProp);

            // TODO: add the remaining properties

            EditorGUILayout.EndVertical();
        } // DrawLevelManagerGUI()

        void DrawLevelsGUI()
        {
            EditorGUI.indentLevel++;

            EditorUtils.BeginBoxStyleDark();
            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.LabelField("- - - - - Levels - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical(); // light

            EditorGUILayout.BeginHorizontal();

            _newLevelName = EditorGUILayout.TextField("Level Name", _newLevelName);

            bool buttonAdd = GUILayout.Button("Add Level", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            if (buttonAdd)
            {
                GameObject go = new GameObject("WaveManager - " + _newLevelName);
                go.transform.SetParent(_myTarget.transform);
                go.transform.localPosition = Vector3.zero;
                var newLevel = go.AddComponent<WaveManager>();
                newLevel.LevelName = _newLevelName;
                newLevel.GenerateGuid();

                _newLevelName = ""; // clear for next add
                _myTarget.Levels.Add(newLevel);
            } // if
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Levels will trigger in the order they are listed below", EditorUtils.guiStyle_Help);
            EditorGUILayout.EndVertical(); // dark

            for (int i = 0; i < _myTarget.Levels.Count; ++i)
            {
                // If the user deleted the wave object manually then we must detect that and remove it from our list
                if (_myTarget.Levels[i] == null)
                {
                    _myTarget.Levels.RemoveAt(i);
                    continue;
                } // if

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[" + _myTarget.Levels[i].LevelName + "]", EditorUtils.guiStyle_MediumBold);
                var editButton = GUILayout.Button("E", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                var moveUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                var moveDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));

                if (editButton)
                {
                    Selection.activeGameObject = _myTarget.Levels[i].gameObject;
                }

                if (moveUp)
                {
                    Utility.MoveItemUp<WaveManager>(_myTarget.Levels, i);
                    GUIUtility.ExitGUI();
                } // if
                if (moveDown)
                {
                    Utility.MoveItemDown<WaveManager>(_myTarget.Levels, i);
                    GUIUtility.ExitGUI();
                } // if

                bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                if (buttonRemove)
                {
                    // Make sure the user really wants to do this
                    if (EditorUtility.DisplayDialog(
                        "Confirm",
                        "Are you sure you want to remove this level?",
                        "OK", "CANCEL"))
                    {
                        DestroyImmediate(_myTarget.Levels[i].gameObject);
                        _myTarget.Levels.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    } // if
                } // if
                EditorGUILayout.EndHorizontal();
            } // foreach

            EditorGUI.indentLevel--;
        } // DrawLevelsGUI()
    } // LevelManagerEditor
} // namespace Magique.SoulLink
