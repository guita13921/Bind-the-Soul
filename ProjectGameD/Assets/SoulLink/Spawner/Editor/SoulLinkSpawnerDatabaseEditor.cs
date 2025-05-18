using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SoulLinkSpawnerDatabase), true)]

    public class SoulLinkSpawnerDatabaseEditor : Editor
    {
        private SoulLinkSpawnerDatabase _myTarget;
        private SerializedObject _mySerializedObject;

        private bool _editDatabase = false;

        // string properties
        SerializedProperty VersionProp, AuthorProp, DescriptionProp;

        private void OnEnable()
        {
            _myTarget = (SoulLinkSpawnerDatabase)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            VersionProp = _mySerializedObject.FindProperty("version");
            AuthorProp = _mySerializedObject.FindProperty("author");
            DescriptionProp = _mySerializedObject.FindProperty("description");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorGUILayout.BeginHorizontal();
            EditorUtils.DrawSoulLinkLogo();
            EditorUtils.DrawSpawnerText();
            EditorGUILayout.EndHorizontal();

            DrawDatabaseInfo();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the SoulLinkSpawnerDatabase into Undo
                Undo.RecordObject(_myTarget, "SoulLinkSpawnerDatabase Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void DrawDatabaseInfo()
        {
            EditorGUILayout.LabelField("Spawner Database", EditorUtils.guiStyle_MediumBold);
            EditorGUILayout.PropertyField(VersionProp);
            EditorGUILayout.PropertyField(AuthorProp);
            EditorGUILayout.PropertyField(DescriptionProp);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("In order to maintain the integrity of the database, you should not directly edit database entries here. " +
                                        "If you are an advanced user and wish to make direct changes, turn on the edit option below. Use at your own risk.", 
                                        EditorUtils.guiStyle_Help);

            _editDatabase = EditorGUILayout.Toggle("Edit Database: ", _editDatabase);
            if (_editDatabase)
            {
                DrawDefaultInspector();
            } // if
        } // DrawDatabaseInfo()
    } // SoulLinkSpawnerDatabaseEditor()
} // namespace Magique.SoulLink
