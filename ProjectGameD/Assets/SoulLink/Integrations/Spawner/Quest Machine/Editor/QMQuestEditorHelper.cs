#if SOULLINK_USE_QM

using PixelCrushers.QuestMachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class QMQuestEditorHelper
    {
        private QMQuest _myTarget;
        private SerializedObject _mySerializedObject;

        private List<string> _variables = new List<string>();
        private int _variableSelected = 0;

        static private bool _questEntriesFoldoutState = true;

        // bool

        // float

        // int

        // string

        // enum

        // object
        SerializedProperty QuestProp;

        public void AssignTarget(QMQuest target)
        {
            _myTarget = target;
            _mySerializedObject = new SerializedObject(_myTarget);

            EditorUtils.Init();

            // bool property initializers

            // float property initializers

            // int property initializers

            // string property initializers

            // enum property initializers

            // object property initializers
            QuestProp = _mySerializedObject.FindProperty("_quest");
        } // OnEnable()

        /// <summary>
        /// Edit a QMQuest object
        /// </summary>
        public bool EditQMQuest()
        {
            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            DrawQuestDataGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the QMQuest into Undo
                Undo.RecordObject(_myTarget, "QMQuest Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if

            return false;
        } // EditQMQuest()

        void DrawQuestDataGUI()
        {
            if (_myTarget.QuestData == null && _myTarget.GetQuestEntries().Count > 0)
            {
                if (_myTarget.GetQuestEntries().Count > 0)
                {
                    // Clear up old entires
                    _myTarget.GetQuestEntries().Clear();
                } // if

                return;
            } // if

            EditorUtils.BeginBoxStyleLight();

            EditorGUILayout.PropertyField(QuestProp);

            EditorUtils.BeginBoxStyleDark();

            if (_myTarget.QuestData == null)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                return;
            } // if

            EditorGUI.indentLevel++;
            _questEntriesFoldoutState = EditorGUILayout.Foldout(_questEntriesFoldoutState, "Quest Entries " + "(" + _myTarget.GetQuestEntries().Count + ")", true);
            if (_questEntriesFoldoutState)
            {
                List<QuestEntryData> removeList = new List<QuestEntryData>();
                if (_myTarget.QuestData != null && _myTarget.GetQuestEntries().Count > 0)
                {
                    foreach (var questEntry in _myTarget.GetQuestEntries())
                    {
                        EditorGUILayout.BeginHorizontal();

                        List<string> nodeNames = new List<string>();
                        foreach (var node in _myTarget.QuestData.nodeList)
                        {
                            // Only add nodes that have conditionals
                            CounterQuestCondition condition = (CounterQuestCondition)node.conditionSet.conditionList.Find(x => x is CounterQuestCondition);
                            if (condition != null)
                            {
                                nodeNames.Add(node.GetEditorName());
                            } // if
                        } // foreach

                        var nodeIndex = nodeNames.IndexOf(questEntry.questEntryName);
                        nodeIndex = EditorGUILayout.Popup("Entry Name", nodeIndex, nodeNames.ToArray());
                        if (nodeIndex >= 0)
                        {
                            questEntry.questEntryName = nodeNames[nodeIndex];
                        } // if

                        bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(35));
                        if (buttonRemove)
                        {
                            removeList.Add(questEntry);
                        } // if
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Separator();

                        // Make a variable list of only the conditions for the selected node
                        var selectedNode = GetSelectedNode(questEntry.questEntryName);
                        if (selectedNode != null)
                        {
                            _variables.Clear();
                            foreach (var condition in selectedNode.conditionSet.conditionList)
                            {
                                // Extract counter name from editor name
                                var editorName = condition.GetEditorName();
                                var parts = editorName.Split(' ');
                                if (parts.Length > 1)
                                {
                                    _variables.Add(parts[1]);
                                } // if
                            } // foreach

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

        private QuestNode GetSelectedNode(string entryName)
        {
            foreach (var node in _myTarget.QuestData.nodeList)
            {
                if (node.GetEditorName() == entryName)
                {
                    return node;
                } // if
            } // foreach

            return null;
        } // GetSelectedNode()

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
    } // QMQuestEditorHelper
} // namespace Magique.SoulLink

#endif