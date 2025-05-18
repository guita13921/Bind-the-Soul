using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SoulLinkGlobal), true)]
    public class SoulLinkGlobalEditor : Editor
    {
        private SoulLinkGlobal _myTarget;
        private SerializedObject _mySerializedObject;

        // bool
        SerializedProperty DebugModeProp, WriteToFileProp;

        // float

        // int

        // string
        SerializedProperty LogFileNameProp;

        // enum

        // object
        SerializedProperty OutputAudioMixerGroupProp;

        private void OnEnable()
        {
            _myTarget = (SoulLinkGlobal)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // One time init

            // bool property initializers
            DebugModeProp = _mySerializedObject.FindProperty("_debugMode");
            WriteToFileProp = _mySerializedObject.FindProperty("_writeToFile");

            // float property initializers

            // int property initializers

            // string property initializers
            LogFileNameProp = _mySerializedObject.FindProperty("_logFileName");

            // enum property initializers

            // object property initializers
            OutputAudioMixerGroupProp = _mySerializedObject.FindProperty("_outputAudioMixerGroupProp");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorGUILayout.BeginHorizontal();
            EditorUtils.DrawSoulLinkLogo();
            EditorGUILayout.LabelField("Welcome to SoulLink, a professional suite of Artificial Intelligence components.", EditorUtils.guiStyle_Help);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            _myTarget.versionsFoldoutState = EditorGUILayout.Foldout(_myTarget.versionsFoldoutState, "Version Information", true);
            if (_myTarget.versionsFoldoutState)
            {
#if SOULLINK_SPAWNER
                EditorUtils.DrawSpawnerText(true);
#endif

#if SOULLINK_AI
                EditorUtils.DrawAIText(true);
#endif
            } // if
            EditorGUI.indentLevel--;

            DrawSoulLinkInfoGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the SoulLinkGlobal into Undo
                Undo.RecordObject(_myTarget, "SoulLinkGlobal Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void DrawSoulLinkInfoGUI()
        {
            EditorUtils.BeginBoxStyleLight();
            EditorGUILayout.EndVertical();

            _myTarget.OutputAudioMixerGroup = EditorGUILayout.ObjectField("Output Audio Mixer Group", _myTarget.OutputAudioMixerGroup,
                typeof(AudioMixerGroup), false) as AudioMixerGroup;

            EditorGUILayout.Separator();

            EditorGUILayout.HelpBox("Toggle Debug Mode on in order to receive additional logging to the console window and to a physical file on disk.", MessageType.Info);

            EditorGUILayout.PropertyField(DebugModeProp);
            if (_myTarget.DebugMode)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(WriteToFileProp);
                if (_myTarget.WriteToFile)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(LogFileNameProp);
                }
            } // if
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            bool buttonWeb = GUILayout.Button("Online Help", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonWeb)
            {
                Application.OpenURL("https://www.magiqueproductions.com/unity-assets/");
            }

            bool buttonDiscord = GUILayout.Button("Discord Channel", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonDiscord)
            {
                Application.OpenURL("https://discord.gg/ncW9UmU");
            }

            bool buttonSupport = GUILayout.Button("E-Mail Support", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonSupport)
            {
                Application.OpenURL("mailto:support@magiqueproductions.com");
            }
            EditorGUILayout.EndHorizontal();
        } // DrawSoulLinkGloablGUI()
    } // SoulLinkGlobalkEditor
} // namespace Magique.SoulLink
