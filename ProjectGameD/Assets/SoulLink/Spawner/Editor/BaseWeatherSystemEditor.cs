using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(BaseWeatherSystem), true)]
    public class BaseWeatherSystemEditor : Editor
    {
        private BaseWeatherSystem _myTarget;
        private SerializedObject _mySerializedObject;

        // bool
        SerializedProperty CustomTempCurveProp, ElevationAffecsTempProp, WeatherAffectsTempProp, WindAffectsTempProp, SeasonBasedTempProp;

        // float
        SerializedProperty PeakTempProp, FallPeakTempProp, WinterPeakTempProp, SpringPeakTempProp, SummerPeakTempProp, RainFactorProp, SnowFactorProp, WindFactorProp;

        // int

        // string

        // enum

        // object
        SerializedProperty TemperatureCurveProp, ElevationCurveProp, TemperaturScaleProp;

        private void OnEnable()
        {
            _myTarget = (BaseWeatherSystem)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // bool property initializers
            SeasonBasedTempProp = _mySerializedObject.FindProperty("_seasonBasedTemperature");
            CustomTempCurveProp = _mySerializedObject.FindProperty("_customTemperatureCurve");              
            ElevationAffecsTempProp = _mySerializedObject.FindProperty("_elevationAffectsTemperature");
            WeatherAffectsTempProp = _mySerializedObject.FindProperty("_weatherAffectsTemperature");
            WindAffectsTempProp = _mySerializedObject.FindProperty("_windAffectsTemperature");

            // float property initializers
            PeakTempProp = _mySerializedObject.FindProperty("_peakTemperature");
            FallPeakTempProp = _mySerializedObject.FindProperty("_fallPeakTemperature");
            WinterPeakTempProp = _mySerializedObject.FindProperty("_winterPeakTemperature");
            SpringPeakTempProp = _mySerializedObject.FindProperty("_springPeakTemperature");
            SummerPeakTempProp = _mySerializedObject.FindProperty("_summerPeakTemperature");
            RainFactorProp = _mySerializedObject.FindProperty("_rainFactor");
            SnowFactorProp = _mySerializedObject.FindProperty("_snowFactor");
            WindFactorProp = _mySerializedObject.FindProperty("_windFactor");

            // int property initializers

            // string property initializers

            // enum property initializers

            // object property initializers
            TemperatureCurveProp = _mySerializedObject.FindProperty("_temperatureCurve");
            ElevationCurveProp = _mySerializedObject.FindProperty("_elevationCurve");
            TemperaturScaleProp = _mySerializedObject.FindProperty("_temperatureScale");
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawTemperatureGUI();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Writing changes of the BaseWeatherSystem into Undo
                Undo.RecordObject(_myTarget, "BaseWeatherSystem Editor Modify");

                EditorUtility.SetDirty(_myTarget);

                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // OnInspectorGUI()

        void DrawTemperatureGUI()
        {
            EditorGUILayout.PropertyField(TemperaturScaleProp);

            if (!_myTarget.HasTemperatureFeature())
            {
                EditorGUILayout.HelpBox("The third party weather system you are using does not have its own temperature control. It is recommended that you turn on some or all of the custom temperature features below.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("The third party weather system you are using has its own temperature control. However, you may enhance this by turning on some or all of the custom temperature features below.", MessageType.Info);
            }

            EditorUtils.BeginBoxStyleDark();

            if (!_myTarget.HasTemperatureFeature())
            { 
                EditorGUILayout.PropertyField(CustomTempCurveProp);
                if (_myTarget.HasCustomTemperatureCurve())
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(TemperatureCurveProp);

                    EditorGUILayout.PropertyField(SeasonBasedTempProp);
                    if (_myTarget.UsesSeasonBasedTemperature())
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(FallPeakTempProp);
                        EditorGUILayout.PropertyField(WinterPeakTempProp);
                        EditorGUILayout.PropertyField(SpringPeakTempProp);
                        EditorGUILayout.PropertyField(SummerPeakTempProp);
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(PeakTempProp);
                    }
                    EditorGUI.indentLevel--;
                }
            } // if

            EditorGUILayout.PropertyField(WeatherAffectsTempProp);
            if (_myTarget.WeatherAffectsTemperature())
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(RainFactorProp);
                EditorGUILayout.PropertyField(SnowFactorProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(WindAffectsTempProp);
            if (_myTarget.WindAffectsTemperature())
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(WindFactorProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(ElevationAffecsTempProp);
            if (_myTarget.ElevationAffectsTemperature())
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(ElevationCurveProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

        } // DrawTemperatureGUI()
    } // BaseWeatherSystemEditor
} // namespace Magique.SoulLink
