using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(Spawnable), true), CanEditMultipleObjects]
    public class SpawnableEditor : Editor
    {
        private ISpawnable _myTarget;
        private SpawnableSettingsObject _spawnableSettings;

        private void OnEnable()
        {
            _myTarget = (ISpawnable)target;
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawSpawnableGUI();

            DrawDefaultInspector();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()

        void DrawSpawnableGUI()
        {
            EditorUtils.BeginBoxStyleDark();

            EditorGUILayout.BeginHorizontal();
            _spawnableSettings = EditorGUILayout.ObjectField(_spawnableSettings, typeof(SpawnableSettingsObject), false) as SpawnableSettingsObject;
            bool buttonImport = GUILayout.Button("Import", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();
            bool buttonExport = GUILayout.Button("Export", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));

            if (buttonImport)
            {
                Utility.ImportSpawnableSettings(_myTarget, _spawnableSettings);
                _spawnableSettings = null;
            } // if

            if (buttonExport)
            {
                Utility.ExportSpawnableSettings(_myTarget);
            } // if

            EditorGUILayout.EndVertical();
        } // DrawSpawnableGUI()
    } // SpawnableEditor
} // namespace Magique.SoulLink
