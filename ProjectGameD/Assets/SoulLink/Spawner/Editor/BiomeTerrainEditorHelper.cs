using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

#if LANDSCAPE_BUILDER
using LandscapeBuilder;
#endif

#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class BiomeTerrainEditorHelper 
    {
        private BiomeTerrain _myTarget;
        private SerializedObject _mySerializedObject;

        private List<string> _textureNames = new List<string>();

        private bool _usePolaris = false;

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

        // bool
        SerializedProperty UseElevationRangeProp;

        // float

        // int

        // string
        SerializedProperty NameProp;

        // enum

        // object
        SerializedProperty FilterClauseProp, ElevationRangeProp;

#if LANDSCAPE_BUILDER
        private LBLandscape _landscape;
        private LBStencil[] _stencils;
        private Dictionary<LBStencil, List<LBStencilLayer>> _stencilLayers = new Dictionary<LBStencil, List<LBStencilLayer>>();
        private List<string> _layerNames = new List<string>();
#endif

        public void AssignTarget(BiomeTerrain target)
        {
            if (target == null)
            {
                Debug.LogError("BiomeTerrainEditorHelper Error: Trying to assign null BiomeTerrain target.");
                return;
            } // if

            _myTarget = target;
            _mySerializedObject = new SerializedObject(_myTarget);

            // bool property initializers
            UseElevationRangeProp = _mySerializedObject.FindProperty("_useElevationRange");

            // float property initializers

            // int property initializers

            // string property initializers
            NameProp = _mySerializedObject.FindProperty("_biomeName");

            // enum property initializers

            // object property initializers
            FilterClauseProp = _mySerializedObject.FindProperty("_filterClause");
            ElevationRangeProp = _mySerializedObject.FindProperty("_elevationRange");

#if LANDSCAPE_BUILDER
            _landscape = UnityEngine.Object.FindObjectOfType<LBLandscape>();
            if (_landscape != null)
            {
                _stencils = _landscape.GetComponentsInChildren<LBStencil>();

                foreach (var stencil in _stencils)
                {
                    _stencilLayers[stencil] = new List<LBStencilLayer>();
                    foreach (var layer in stencil.stencilLayerList)
                    {
                        _stencilLayers[stencil].Add(layer);
                    } // foreach
                } // foreach
            } // if
#endif

#if GRIFFIN_2021
            _terrains = UnityEngine.Object.FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif

            InitTextureNames();
        } // AssignTarget()

        private void InitTextureNames()
        {
            if (_usePolaris)
            {
#if GRIFFIN_2021
                var mapCount = _terrains[0] != null ? _terrains[0].TerrainData.Shading.SplatControlMapCount : 0;
                for (int i = 0; i < mapCount; ++i)
                {
                    for (int j = 1; j < 5; ++j)
                    {
                        string layerName = "Layer_" + (j + (i * 4));
                        _textureNames.Add(layerName);
                    } // for j
                } // for i
#endif
            }
            else
            {
                _textureNames.Clear();
                var targetTerrain = Terrain.activeTerrain;
                if (targetTerrain != null && targetTerrain.terrainData != null)
                {
                    foreach (var layer in targetTerrain.terrainData.terrainLayers)
                    {
                        if (layer != null)
                        {
                            _textureNames.Add(layer.name);
                        } // if
                    } // foreach layer
                } // if
            }
        } // InitTextureNames()

        public void UpdateObject()
        {
            if (GUI.changed && _myTarget != null)
            {
                // Writing changes of the BiomeTerrain into Undo
                Undo.RecordObject(_myTarget, "BiomeTerrain Editor Helper Modify");

                EditorUtility.SetDirty(_myTarget);
                _mySerializedObject.ApplyModifiedProperties();
            } // if
        } // UpdateObject()

        public void DrawGeneralInfo()
        {
            if (_myTarget == null)
            {
                EditorGUILayout.LabelField("Target is null.");
                return;
            }

            EditorUtils.BeginBoxStyleLight();

            // TODO: if this name is changed and it already exists by the old name in _biomeNames then rename it so the reference is not lost
            EditorGUILayout.PropertyField(NameProp);

            EditorGUILayout.EndVertical();
        } // DrawGeneralInfo()

        public void DrawTextureRulesGUI()
        {
            if (_myTarget == null || _textureNames.Count == 0)
            {
                if (_myTarget == null)
                {
                    EditorGUILayout.LabelField("Target is null.");
                }
                if (_textureNames.Count == 0)
                {
#if GRIFFIN_2021
                if (_terrains[0] != null && _terrains[0].TerrainData.Shading.SplatControlMapCount == 0)
                {
                    EditorGUILayout.LabelField("Error: Polaris terrain does not contain any splat maps.");
                }
#else
                    EditorGUILayout.LabelField("Error: Texture Names are empty.");
#endif
                }
                return;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            // Texture Filters
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            _myTarget.texFilterFoldoutState = EditorGUILayout.Foldout(_myTarget.texFilterFoldoutState, "Texture Filters (" + _myTarget.TextureRules.Count + ")", true);
            bool buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();

            if (_myTarget.texFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                EditorGUILayout.PropertyField(FilterClauseProp);
                if (_myTarget.FilterClause == FilterClause.All)
                {
                    EditorGUILayout.HelpBox("All texture filters must pass their checks to be considered in this biome.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Any texture filter can pass their check to be considered in this biome.", MessageType.Info);
                }

                for (int i = 0; i < _myTarget.TextureRules.Count; ++i)
                {
                    var texRule = _myTarget.TextureRules[i];

                    if (texRule.textureIndex < _textureNames.Count)
                    {
                        EditorGUILayout.LabelField(_textureNames[texRule.textureIndex]);
                        EditorGUILayout.BeginHorizontal();

                        EditorUtils.SaveLabelWidth();
                        EditorUtils.SetLabelWidth("Terrain Layer:     ");
                        texRule.textureIndex = EditorGUILayout.Popup("Terrain Layer: ", texRule.textureIndex, _textureNames.ToArray(), GUILayout.Width(200));

                        EditorUtils.SetLabelWidth("Rule:     ");
                        texRule.textureRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", texRule.textureRule);

                        EditorUtils.SetLabelWidth("Threshhold:     ");
                        texRule.threshold = EditorGUILayout.FloatField("Threshold: ", texRule.threshold, GUILayout.Width(128));
                        EditorUtils.RestoreLabelWidth();

                        bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                        if (buttonDeleteRule)
                        {
                            _myTarget.TextureRules.RemoveAt(i);
                        } // if

                        EditorGUILayout.EndHorizontal();
                        EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                    } // if
                } // foreach texRule

                EditorGUILayout.EndVertical();
            } // if foldout is true
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (buttonAddRule)
            {
                _myTarget.TextureRules.Add(new TextureRuleData()
                { textureIndex = _myTarget.textureSelected, textureRule = (FilterRule)_myTarget.texRuleSelected, threshold = 0.5f });

                _myTarget.texFilterFoldoutState = true;
            } // if

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        } // DrawTextureRulesGUI()

        public void DrawVolumeRulesGUI()
        {
            if (_myTarget == null)
            {
                EditorGUILayout.LabelField("Target is null.");
                return;
            } // if

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            // Volume Filters
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            _myTarget.volFilterFoldoutState = EditorGUILayout.Foldout(_myTarget.volFilterFoldoutState, "Volume Filters (" + _myTarget.VolumeRules.Count + ")", true);
            bool buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();

            if (_myTarget.volFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < _myTarget.VolumeRules.Count; ++i)
                {
                    var volRule = _myTarget.VolumeRules[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorUtils.SaveLabelWidth();
                    volRule.volume = (Collider)EditorGUILayout.ObjectField(volRule.volume, typeof(Collider), true, GUILayout.Width(175));

                    var currRule = volRule.filterRule;
                    EditorUtils.SetLabelWidth("Rule:     ");
                    volRule.filterRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", volRule.filterRule);
                    EditorUtils.RestoreLabelWidth();
                    if (currRule != volRule.filterRule)
                    {
                        switch (volRule.filterRule)
                        {
                            case FilterRule.Include:
                                volRule.depth = 999f;
                                break;
                            case FilterRule.Exclude:
                                volRule.depth = 0f;
                                break;
                        } // switch
                    } // if

                    EditorUtils.SetLabelWidth("Depth:     ");
                    volRule.depth = EditorGUILayout.FloatField("Depth: ", volRule.depth);

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                    if (buttonDeleteRule)
                    {
                        _myTarget.VolumeRules.RemoveAt(i);
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach volRule

                EditorGUILayout.EndVertical();
            } // if foldout is true
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (buttonAddRule)
            {
                _myTarget.VolumeRules.Add(new VolumeRuleData()
                { filterRule = (FilterRule)_myTarget.volRuleSelected, depth = (FilterRule)_myTarget.volRuleSelected == FilterRule.Exclude ? 0f : 999f });

                _myTarget.volFilterFoldoutState = true;
            } // if

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        } // DrawVolumeRulesGUI()

        public void DrawLayerRulesGUI()
        {
            if (_myTarget == null)
            {
                EditorGUILayout.LabelField("Target is null.");
                return;
            } // if

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            // Layer Filters
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            _myTarget.volFilterFoldoutState = EditorGUILayout.Foldout(_myTarget.volFilterFoldoutState, "Layer Filters (" + _myTarget.LayerRules.Count + ")", true);
            bool buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();

            if (_myTarget.layerFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                for (int i = 0; i < _myTarget.LayerRules.Count; ++i)
                {
                    var layerRule = _myTarget.LayerRules[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorUtils.SaveLabelWidth();
//                    EditorUtils.SetLabelWidth("Layer:     ");
                    _myTarget.layerRuleSelected = EditorGUILayout.LayerField(_myTarget.layerRuleSelected);
                    layerRule.layerName = LayerMask.LayerToName(_myTarget.layerRuleSelected);

                    EditorUtils.SetLabelWidth("Rule:     ");
                    layerRule.filterRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", layerRule.filterRule);
                    EditorUtils.RestoreLabelWidth();

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                    if (buttonDeleteRule)
                    {
                        _myTarget.LayerRules.RemoveAt(i);
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach layerRule

                EditorGUILayout.EndVertical();
            } // if foldout is true
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (buttonAddRule)
            {
                _myTarget.LayerRules.Add(new LayerRuleData()
                { filterRule = (FilterRule)_myTarget.layerRuleSelected  });

                _myTarget.layerFilterFoldoutState = true;
            } // if

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        } // DrawLayerRulesGUI()

        public void DrawElevationRangeGUI()
        {
            EditorGUILayout.PropertyField(UseElevationRangeProp);
            if (UseElevationRangeProp.boolValue == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(ElevationRangeProp);
                EditorGUI.indentLevel--;
            } // if
        } // DrawElevationRangeGUI()

#if LANDSCAPE_BUILDER
        public void DrawStencilRulesGUI()
        {
            if (_myTarget == null || _stencilLayers.Count == 0) return;

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            // Stencil Filters
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            _myTarget.stencilFilterFoldoutState = EditorGUILayout.Foldout(_myTarget.stencilFilterFoldoutState, "Stencil Filters (" + _myTarget.StencilRules.Count + ")", true);
            bool buttonAddRule = GUILayout.Button("Add Filter", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();

            if (_myTarget.stencilFilterFoldoutState)
            {
                EditorUtils.BeginBoxStyleDark();

                EditorGUILayout.PropertyField(FilterClauseProp);
                if (_myTarget.FilterClause == FilterClause.All)
                {
                    EditorGUILayout.HelpBox("All stencil filters must pass their checks to be considered in this biome.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Any stencil filter can pass their check to be considered in this biome.", MessageType.Info);
                }

                for (int i = 0; i < _myTarget.StencilRules.Count; ++i)
                {
                    var stencilRule = _myTarget.StencilRules[i];

                    stencilRule.stencil = EditorGUILayout.ObjectField("Stencil: ", stencilRule.stencil, typeof(LBStencil), true) as LBStencil;

                    EditorGUILayout.BeginHorizontal();

                    if (stencilRule.stencil != null)
                    {
                        EditorUtils.SaveLabelWidth();
                        EditorUtils.SetLabelWidth("Layer:      ");

                        _layerNames.Clear();
                        foreach (var layer in _stencilLayers[stencilRule.stencil])
                        {
                            _layerNames.Add(layer.LayerName);
                        } // foreach
                        var layerIndex = _layerNames.IndexOf(stencilRule.layer.LayerName);
                        layerIndex = EditorGUILayout.Popup("Layer: ", layerIndex == -1 ? 0 : layerIndex, _layerNames.ToArray(), GUILayout.Width(200));
                        stencilRule.layer = _stencilLayers[stencilRule.stencil][layerIndex];

                        EditorUtils.SetLabelWidth("Rule:      ");
                        stencilRule.stencilRule = (FilterRule)EditorGUILayout.EnumPopup("Rule: ", stencilRule.stencilRule);

                        EditorUtils.SetLabelWidth("Threshhold:      ");
                        stencilRule.threshold = EditorGUILayout.FloatField("Threshold: ", stencilRule.threshold, GUILayout.Width(128));
                        EditorUtils.RestoreLabelWidth();
                    } // if

                    bool buttonDeleteRule = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(32));
                    if (buttonDeleteRule)
                    {
                        _myTarget.StencilRules.RemoveAt(i);
                    } // if

                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // foreach stencilRule

                EditorGUILayout.EndVertical();
            } // if foldout is true
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (buttonAddRule)
            {
                _myTarget.StencilRules.Add(new StencilRuleData()
                { stencilRule = (FilterRule)_myTarget.stencilRuleSelected, threshold = 0.5f });

                _myTarget.stencilFilterFoldoutState = true;
            } // if

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        } // DrawStencilRulesGUI()
#endif
    } // class BiomeTerrainEditorHelper
} // namespace Magique.SoulLink
