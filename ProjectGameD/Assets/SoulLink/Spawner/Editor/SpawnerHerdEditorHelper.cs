using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

namespace Magique.SoulLink
{
    public class SpawnerHerdEditorHelper 
    {
        private SpawnerHerd _myTarget;
        private SerializedObject _mySerializedObject;

        // bool

        // float

        // int
        SerializedProperty MinPopulationProp, MaxPopulationProp, SpawnRadiusProp;

        // string
        SerializedProperty NameProp;

        // enum

        // object

        public void AssignTarget(SpawnerHerd target)
        {
            _myTarget = target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // bool property initializers

            // float property initializers
            SpawnRadiusProp = _mySerializedObject.FindProperty("spawnRadius");

            // int property initializers
            MinPopulationProp = _mySerializedObject.FindProperty("minPopulation");
            MaxPopulationProp = _mySerializedObject.FindProperty("maxPopulation");

            // string property initializers
            NameProp = _mySerializedObject.FindProperty("herdName");

            // enum property initializers

            // object property initializers
        } // AssignTarget()

        public void UpdateObject()
        {
            if (GUI.changed && _myTarget != null)
            {
                // Writing changes of the SpawnManager into Undo
                Undo.RecordObject(_myTarget, "SpawnerHerd Editor Helper Modify");

                EditorUtility.SetDirty(_myTarget);
                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // UpdateObject()

        public void DrawGeneralInfo()
        {
            if (_myTarget == null) return;

            EditorUtils.BeginBoxStyleLight();

            EditorGUILayout.PropertyField(NameProp);
            EditorGUILayout.PropertyField(MinPopulationProp);
            EditorGUILayout.PropertyField(MaxPopulationProp);
            EditorGUILayout.PropertyField(SpawnRadiusProp);

            EditorGUILayout.EndVertical();
        } // DrawGeneralInfo()

        public void DrawSpawnInfoItemsGUI()
        {
            if (_myTarget == null) return;

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            List<SpawnInfoData> removeSpawns = new List<SpawnInfoData>();

            EditorGUI.indentLevel++;
            _myTarget.foldoutState = EditorGUILayout.Foldout(_myTarget.foldoutState, "Spawns (" + _myTarget.herdPrefabs.Count + ")");
            if (_myTarget.foldoutState)
            {
                foreach (var spawn in _myTarget.herdPrefabs)
                {
                    // Force to Prefab type (Herds cannot contain herds at this time)
                    spawn.spawnType = SpawnType.Prefab;

                    if (spawn.prefab != null)
                    {
                        EditorGUILayout.LabelField("[" + spawn.prefab.name + "]", EditorUtils.guiStyle_LargeBold);
                    } // if
                    spawn.prefab = EditorGUILayout.ObjectField("Prefab", spawn.prefab, typeof(Transform), false) as Transform;

                    // Population Cap is only relevant to individual spawns, not herds
                    spawn.populationCap = EditorGUILayout.IntField("Population Cap", spawn.populationCap);
                    if (spawn.populationCap <= 0)
                    {
                        spawn.populationCap = 1;
                    } // if

                    spawn.probability = EditorGUILayout.IntField("Probability", spawn.probability);
                    spawn.ignorePlayerFOV = EditorGUILayout.Toggle("Ignore Player FOV", spawn.ignorePlayerFOV);

                    spawn.minScale = EditorGUILayout.FloatField("Min Scale", spawn.minScale);
                    spawn.maxScale = EditorGUILayout.FloatField("Max Scale", spawn.maxScale);
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
                        spawn.minSlopeAngle = EditorGUILayout.FloatField("     Min Slope Angle", spawn.minSlopeAngle);
                        spawn.maxSlopeAngle = EditorGUILayout.FloatField("     Max Slope Angle", spawn.maxSlopeAngle);
                    } // if

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Separator();
                    bool buttonDelete = GUILayout.Button("Remove Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
                    if (buttonDelete)
                    {
                        removeSpawns.Add(spawn);
                    } // if
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                } // foreach spawn

                // Remove items as necessary
                foreach (var spawn in removeSpawns)
                {
                    _myTarget.herdPrefabs.Remove(spawn);
                } // foreach

                EditorGUILayout.BeginHorizontal();

                bool buttonAdd = GUILayout.Button("Add Spawn", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
                if (buttonAdd)
                {
                    _myTarget.herdPrefabs.Add(new SpawnInfoData());
                } // if
                EditorGUILayout.EndHorizontal();
            } // if foldout
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        } // DrawSpawnInfoItemsGUI()
    } // class SpawnerHerdEditorHelper
} // namespace Magique.SoulLink
