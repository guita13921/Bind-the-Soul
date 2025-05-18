using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(WaveManager), true)]
    public class WaverManagerEditor : Editor
    {
        private WaveManager _myTarget;
        private SerializedObject _mySerializedObject;

        static private string _newWaveName = "";

        // bool
        SerializedProperty AccumulateWaveRepeatsProp;

        // float

        // int

        // string
        SerializedProperty LevelNameProp;

        // enum

        // object

        private void OnEnable()
        {
            _myTarget = (WaveManager)target;
            _mySerializedObject = new SerializedObject(target);

            // bool property initializers
            AccumulateWaveRepeatsProp = _mySerializedObject.FindProperty("_accumulateWaveRepeats");

            // float property initializers

            // int property initializers

            // string property initializers
            LevelNameProp = _mySerializedObject.FindProperty("_levelName");

            // enum property initializers

            // object property initializers

        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawWaveManagerGUI();
            DrawWavesGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void DrawWaveManagerGUI()
        {
            if (_myTarget.LevelManager != null)
            {
                bool buttonLevelMgr = GUILayout.Button("Goto Level Manager", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                if (buttonLevelMgr)
                {
                    Selection.activeGameObject = _myTarget.LevelManager.gameObject;
                } // if
            } // if

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

            EditorUtils.BeginBoxStyleDark();

            EditorGUILayout.PropertyField(LevelNameProp);
            EditorGUILayout.PropertyField(AccumulateWaveRepeatsProp);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        } // DrawWaveManagerGUI()

        void DrawWavesGUI()
        {
            EditorGUI.indentLevel++;

            EditorUtils.BeginBoxStyleDark();
            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.LabelField("- - - - - Waves - - - - -", EditorUtils.guiStyle_CenteredText, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();

            _newWaveName = EditorGUILayout.TextField("Wave Name", _newWaveName);

            bool buttonAdd = GUILayout.Button("Add Wave", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            if (buttonAdd)
            {
                GameObject go = new GameObject("WaveSpawner - " + _newWaveName);
                go.transform.SetParent(_myTarget.transform);
                go.transform.localPosition = Vector3.zero;
                var newWave = go.AddComponent<WaveSpawner>();
                newWave.WaveName = _newWaveName;
                newWave.GenerateGuid();

                _newWaveName = ""; // clear for next add
                _myTarget.Waves.Add(newWave);
            } // if
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Waves will trigger in the order they are listed below", EditorUtils.guiStyle_Help);
            EditorGUILayout.EndVertical();

            for (int i = 0; i < _myTarget.Waves.Count; ++i)
            {
                // If the user deleted the wave object manually then we must detect that and remove it from our list
                if (_myTarget.Waves[i] == null)
                {
                    _myTarget.Waves.RemoveAt(i);
                    continue;
                } // if

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[" + _myTarget.Waves[i].WaveName + "]", EditorUtils.guiStyle_MediumBold);
                var editButton = GUILayout.Button("E", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                var moveUp = GUILayout.Button("▲", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                var moveDown = GUILayout.Button("▼", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));

                if (editButton)
                {
                    Selection.activeGameObject = _myTarget.Waves[i].gameObject;
                }

                if (moveUp)
                {
                    Utility.MoveItemUp<WaveSpawner>(_myTarget.Waves, i);
                    GUIUtility.ExitGUI();
                } // if
                if (moveDown)
                {
                    Utility.MoveItemDown<WaveSpawner>(_myTarget.Waves, i);
                    GUIUtility.ExitGUI();
                } // if

                bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                if (buttonRemove)
                {
                    // Make sure the user really wants to do this
                    if (EditorUtility.DisplayDialog(
                        "Confirm",
                        "Are you sure you want to remove this wave?",
                        "OK", "CANCEL"))
                    {
                        DestroyImmediate(_myTarget.Waves[i].gameObject);
                        _myTarget.Waves.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    } // if
                } // if
                EditorGUILayout.EndHorizontal();
            } // foreach

            EditorGUI.indentLevel--;
        } // DrawWavesGUI()

    } // WaveManagerEditor
} // namespace Magique.SoulLink
