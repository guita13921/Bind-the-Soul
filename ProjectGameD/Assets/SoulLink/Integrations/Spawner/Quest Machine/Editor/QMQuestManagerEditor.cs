#if SOULLINK_USE_QM

using PixelCrushers;
using PixelCrushers.QuestMachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(QMQuestManager), true)]
    public class QMQuestManagerEditor : Editor
    {
        private SerializedObject _mySerializedObject;
        private QMQuestManager _myTarget;

        private void OnEnable()
        {
            _myTarget = (QMQuestManager)target;
            _mySerializedObject = new SerializedObject(_myTarget);
        } // OnEnable()

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
                _myTarget.gameObject.AddComponent<QMQuest>();
            } // if
        } // DrawQuestDataGUI()
    } // QMQuestManagerEditor
} // namespace Magique.SoulLink

#endif