using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(BaseGameTime), true)]
    public class BaseGameTimeEditor : Editor
    {
        private BaseGameTime _myTarget;
        private SerializedObject _mySerializedObject;

        // bool

        // float
        SerializedProperty DayLengthInMinutesProp;

        // int
        SerializedProperty CurrentHourProp, CurrentMinuteProp, CurrentDayProp, CurrentMonthProp, CurrentYearProp;

        // string

        // enum

        // object

        private void OnEnable()
        {
            _myTarget = (BaseGameTime)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // float property initializers
            DayLengthInMinutesProp = _mySerializedObject.FindProperty("_dayLengthInMinutes");

            // int property initializers
            CurrentHourProp = _mySerializedObject.FindProperty("_currentHour");
            CurrentMinuteProp = _mySerializedObject.FindProperty("_currentMinute");
            CurrentDayProp = _mySerializedObject.FindProperty("_currentDay");
            CurrentMonthProp = _mySerializedObject.FindProperty("_currentMonth");
            CurrentYearProp = _mySerializedObject.FindProperty("_currentYear");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            EditorGUILayout.EndVertical();

            DrawGameTimeInfoGUI();

            if (GUI.changed)
            {
                // Writing changes of the BaseGameTime into Undo
                Undo.RecordObject(_myTarget, "BaseGameTime Editor Modify");

                EditorUtility.SetDirty(_myTarget);
                _mySerializedObject.ApplyModifiedProperties();
            }
        } // OnInspectorGUI()

        void DrawGameTimeInfoGUI()
        {
            if (_myTarget.GetSoulLinkControlsTime())
            {
                GUI.enabled = !Application.isPlaying;
                EditorUtils.BeginBoxStyleDark();
                EditorGUILayout.PropertyField(DayLengthInMinutesProp);
                EditorGUILayout.PropertyField(CurrentHourProp);
                EditorGUILayout.PropertyField(CurrentMinuteProp);
            }
            else
            {
                EditorUtils.BeginBoxStyleDark();
                EditorGUILayout.LabelField("Day Length in Minutes: ", _myTarget.GetDayLength().ToString());
                EditorGUILayout.LabelField("Current Hour: ", _myTarget.GetHour().ToString());
                EditorGUILayout.LabelField("Current Minute: ", _myTarget.GetMinute().ToString());
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(Utility.BuildTimeString(_myTarget.GetTime()));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            if (_myTarget.GetSoulLinkControlsDate())
            {
                GUI.enabled = !Application.isPlaying;
                EditorUtils.BeginBoxStyleDark();
                EditorGUILayout.PropertyField(CurrentDayProp);
                EditorGUILayout.PropertyField(CurrentMonthProp);
                EditorGUILayout.PropertyField(CurrentYearProp);
            }
            else
            {
                EditorUtils.BeginBoxStyleDark();
                EditorGUILayout.LabelField("Current Day: ", _myTarget.GetDay().ToString());
                EditorGUILayout.LabelField("Current Month: ", _myTarget.GetMonth().ToString());
                EditorGUILayout.LabelField("Current Year: ", _myTarget.GetYear().ToString());
            }

            EditorGUILayout.EndVertical();
        } // DrawGameTimeInfoGUI()
    } // BasegameTimeEditor
} // namespace Magique.SoulLink
