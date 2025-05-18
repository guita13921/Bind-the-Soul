#if SOULLINK_USE_DS

using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(DSQuest), true)]
    public class DSQuestEditor: Editor
    {
        private DSQuest _myTarget;
        private SerializedObject _mySerializedObject;

        private DialogueDatabase _dialogueDatabase;
        private List<string> _variables = new List<string>();
        private int _variableSelected = 0;

        static private bool _questEntriesFoldoutState = true;

        // bool

        // float

        // int

        // string
        SerializedProperty QuestNameProp;

        // enum

        // object

        [MenuItem("GameObject/SoulLink/Dialogue System Quest", false, 12)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            var ds = Resources.FindObjectsOfTypeAll(typeof(PixelCrushers.DialogueSystem.DialogueSystemController));
            if (ds.Length == 0)
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "There is no Dialogue System in your scene.",
                    "OK"))
                {
                } // if

                return;
            } // if

            // Create a custom game object
            GameObject go = new GameObject("Dialogue System Quest");

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

            var quest = go.AddComponent<DSQuest>();

            // Check if save system is in scene and add the saver component automatically if there
            var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
            if (saveSystem.Length != 0)
            {
                quest.gameObject.AddComponent<Quest_Saver>();
            } // if
        } // CreateCustomGameObject()

        private void OnEnable()
        {
            _myTarget = (DSQuest)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            InitVariables();

            // bool property initializers

            // float property initializers

            // int property initializers

            // string property initializers
            QuestNameProp = _mySerializedObject.FindProperty("_questName");

            // enum property initializers

            // object property initializers
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawQuestDataGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the DSQuest into Undo
                Undo.RecordObject(_myTarget, "DSQuest Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void InitVariables()
        {
            _dialogueDatabase = EditorTools.FindInitialDatabase();

            if (_dialogueDatabase == null) return;

            var variables = _dialogueDatabase.variables;
            foreach (var variable in variables)
            {
                _variables.Add(variable.Name);
            } // foreach
        } // InitVariables()

        void DrawQuestDataGUI()
        {
            EditorUtils.BeginBoxStyleLight();

            EditorGUILayout.PropertyField(QuestNameProp);

            EditorUtils.BeginBoxStyleDark();

            EditorGUI.indentLevel++;
            _questEntriesFoldoutState = EditorGUILayout.Foldout(_questEntriesFoldoutState, "Quest Entries " + "(" + _myTarget.GetQuestEntries().Count + ")", true);
            if (_questEntriesFoldoutState)
            {
                List<QuestEntryData> removeList = new List<QuestEntryData>();
                if (_myTarget.GetQuestEntries().Count > 0)
                {
                    foreach (var questEntry in _myTarget.GetQuestEntries())
                    {
                        EditorGUILayout.BeginHorizontal();
                        questEntry.questEntryName = EditorGUILayout.TextField("Entry Name", questEntry.questEntryName);
                        bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(35));
                        if (buttonRemove)
                        {
                            removeList.Add(questEntry);
                        } // if
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Separator();
                        _variableSelected = _variables.IndexOf(questEntry.questVariableName);
                        _variableSelected = EditorGUILayout.Popup("Variable Name", _variableSelected, _variables.ToArray());
                        if (_variableSelected >= 0)
                        {
                            questEntry.questVariableName = _variables[_variableSelected];
                        } // if
                        questEntry.useGlobalQtyRequired = EditorGUILayout.Toggle("Use Global Qty Required", questEntry.useGlobalQtyRequired);

                        if (questEntry.useGlobalQtyRequired)
                        {
                            questEntry.globalQtyRequired = EditorGUILayout.IntField("Global Qty Required", questEntry.globalQtyRequired);
                        } // if

                        EditorGUI.indentLevel++;
                        questEntry.itemFoldoutState = EditorGUILayout.Foldout(questEntry.itemFoldoutState, "Items " + "(" + questEntry.questEntryItems.Count + ")", true);
                        if (questEntry.itemFoldoutState)
                        {
                            DrawQuestEntryItemsGUI(questEntry.questEntryItems);
                        } // if
                        EditorGUI.indentLevel--;

                        EditorUtils.BeginBgndStyle();
                        EditorGUILayout.Separator();
                        EditorGUILayout.EndVertical();
                    } // foreach

                    if (removeList.Count > 0)
                    {
                        foreach (var entry in removeList)
                        {
                            _myTarget.GetQuestEntries().Remove(entry);
                        } // foreach
                    } // if
                } // if
            } // if foldout 
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();
            bool buttonAdd = GUILayout.Button("Add Quest Entry", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAdd)
            {
                _myTarget.GetQuestEntries().Add(new QuestEntryData());
            } // if
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        } // DrawQuestDataGUI()

        void DrawQuestEntryItemsGUI(List<QuestEntryItemData> entryItems)
        {
            List<QuestEntryItemData> removeList = new List<QuestEntryItemData>();
            EditorGUILayout.Separator();
            foreach (var entryItem in entryItems)
            {
                EditorGUILayout.BeginHorizontal();
                entryItem.prefab = EditorGUILayout.ObjectField("Prefab", entryItem.prefab, typeof(Transform), false) as Transform;

                bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(35));
                if (buttonRemove)
                {
                    removeList.Add(entryItem);
                } // if
                EditorGUILayout.EndHorizontal();

                entryItem.requiredQty = EditorGUILayout.IntField("Required Qty", entryItem.requiredQty);
            } // foreach

            if (removeList.Count > 0)
            {
                foreach (var item in removeList)
                {
                    entryItems.Remove(item);
                } // foreach
            } // if

            EditorGUILayout.BeginHorizontal();
            bool buttonAdd = GUILayout.Button("Add Item", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonAdd)
            {
                entryItems.Add(new QuestEntryItemData());
            } // if
            EditorGUILayout.EndHorizontal();
        } // DrawQuestEntryItemsGUI()
    } // DSQuestEditor
} // namespace Magique.SoulLink

#endif