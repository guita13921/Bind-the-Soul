using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SpawnRegion), true)]
    public class SpawnRegionEditor : Editor
    {
        private SpawnRegion _myTarget;
        private SerializedObject _mySerializedObject;

        // bool

        // float

        // int

        // string
        SerializedProperty TriggerTagProp;

        // enum

        // object
        SerializedProperty DatabaseProp, OnTriggeredEventProp;

        [MenuItem("GameObject/SoulLink/Spawn Region", false, 11)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Spawn Region");

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            SceneView.lastActiveSceneView.MoveToView();
            var region = go.AddComponent<SpawnRegion>();
            region.GetComponent<BoxCollider>().size = new Vector3(128f, 64f, 128f); // just a reasonable default size
        } // CreateCustomGameObject()

        private void OnEnable()
        {
            _myTarget = (SpawnRegion)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // bool property initializers

            // float property initializers

            // int property initializers

            // string property initializers
            TriggerTagProp = _mySerializedObject.FindProperty("_triggerTag");

            // enum property initializers

            // object property initializers
            DatabaseProp = _mySerializedObject.FindProperty("_database");
            OnTriggeredEventProp = _mySerializedObject.FindProperty("_onTriggeredEventHandler");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            TriggerTagProp.stringValue = EditorGUILayout.TagField("Trigger Tag", TriggerTagProp.stringValue);
            EditorGUILayout.PropertyField(DatabaseProp);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.PropertyField(OnTriggeredEventProp);
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the SpawnRegion into Undo
                Undo.RecordObject(_myTarget, "SpawnRegion Editor Modify");

                EditorUtility.SetDirty(_myTarget);
                _mySerializedObject.ApplyModifiedProperties();
            }
        } // OnInspectorGUI()

        private void OnSceneGUI()
        {
            Handles.color = new Color(0f, 0f, 1f, 1f);
            var bounds = _myTarget.GetComponent<BoxCollider>().bounds;
            Handles.DrawWireCube(bounds.center, bounds.size);
        } // OnSceneGUI()

    } // class SpawnRegionEditor
} // namespace Magique.SoulLink
