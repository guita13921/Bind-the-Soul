#if SOULLINK_USE_DS

using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(DSQuestManager), true)]
    public class DSQuestManagerEditor : Editor
    {
        private SerializedObject _mySerializedObject;
        private DSQuestManager _myTarget;

        private void OnEnable()
        {
            _myTarget = (DSQuestManager)target;
            _mySerializedObject = new SerializedObject(_myTarget);
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawQuestDataGUI();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()

        void DrawQuestDataGUI()
        {
            bool buttonAddQuest = GUILayout.Button("Add Quest", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));

            if (buttonAddQuest)
            {
                _myTarget.gameObject.AddComponent<DSQuest>();
            } // if
        } // DrawQuestDataGUI()
    } // DSQuestManagerEditor
} // namespace Magique.SoulLink

#endif