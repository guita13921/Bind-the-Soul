#if SOULLINK_USE_QM

using PixelCrushers;
using PixelCrushers.QuestMachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(QMQuest), true)]
    public class QMQuestEditor : Editor
    {
        private QMQuest _myTarget;

        [MenuItem("GameObject/SoulLink/Quest Machine Quest", false, 12)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            var qm = Resources.FindObjectsOfTypeAll(typeof(PixelCrushers.QuestMachine.QuestMachineConfiguration));
            if (qm.Length == 0)
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "There is no Quest Machine in your scene.",
                    "OK"))
                {
                } // if

                return;
            } // if

            // Create a custom game object
            GameObject go = new GameObject("Quest Machine Quest");

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            var quest = go.AddComponent<QMQuest>();

            // Check if save system is in scene and add the saver component automatically if there
            var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
            if (saveSystem.Length != 0)
            {
                quest.gameObject.AddComponent<Quest_Saver>();
            } // if
        } // CreateCustomGameObject()

        private void OnEnable()
        {
            _myTarget = (QMQuest)target;
        } // OnEnable()


        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            if (_myTarget.gameObject.GetComponent<QMQuestManager>() == null)
            {
                EditorUtils.DrawSoulLinkLogo();
            } // if

            QMQuestEditorHelper helper = new QMQuestEditorHelper();
            helper.AssignTarget(_myTarget);
            helper.EditQMQuest();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()
    } // QMQuestEditor
} // namespace Magique.SoulLink

#endif