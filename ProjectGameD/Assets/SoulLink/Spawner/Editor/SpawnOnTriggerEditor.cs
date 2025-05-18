using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SpawnOnTrigger), true), CanEditMultipleObjects]
    public class SpawnOnTriggerEditor : Editor
    {
        private SpawnOnTrigger _myTarget;

        private void OnEnable()
        {
            _myTarget = (SpawnOnTrigger)target;
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawDefaultInspector();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()

        private void OnSceneGUI()
        {
            Handles.color = new Color(0f, 1f, 0f, 0.1f);
            Handles.DrawSolidDisc(_myTarget.transform.position, Vector3.up, _myTarget.TriggerRange);
        } // OnSceneGUI()
    } // SpawnOnTriggerEditor
} // namespace Magique.SoulLink
