using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if MAPMAGIC2
using MapMagic.Core;
#endif
#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(QueryTerrainTextures), true)]
    public class QueryTerrainTexturesEditor : Editor
    {
        private QueryTerrainTextures _myTarget;
        private List<string> _textureNames = new List<string>();

        private bool _useMapMagic = false;
        private bool _usePolaris = false;

        private int _maxTextureNameWidth = -1;

        private SerializedObject _mySerializedObject;
        private SerializedProperty SceneViewModeProp;

        const float MIN_AREA_WIDTH = 175f;

#if MAPMAGIC2
        private MapMagicObject _mapMagicObject;
#endif

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

        private void OnEnable()
        {
            _myTarget = (QueryTerrainTextures)target;
            _mySerializedObject = new SerializedObject(_myTarget);

            SceneViewModeProp = _mySerializedObject.FindProperty("_sceneViewMode");

#if MAPMAGIC2
            _mapMagicObject = FindObjectOfType<MapMagicObject>();
            _useMapMagic = (_mapMagicObject != null);
#endif

#if GRIFFIN_2021
            _terrains = FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif

            InitTextureNames();
        } // OnEnable()

        private void InitTextureNames()
        {
            _textureNames.Clear();
            if (_useMapMagic)
            {
#if MAPMAGIC2
                if (_mapMagicObject != null && _mapMagicObject.tiles != null)
                {
                    var tile = _mapMagicObject.tiles.FindByWorldPosition(_mapMagicObject.transform.position.x + 1f, _mapMagicObject.transform.position.z + 1f);
                    if (tile != null)
                    {
                        foreach (var layer in tile.ActiveTerrain.terrainData.terrainLayers)
                        {
                            _textureNames.Add(layer.name);
                        } // foreach layer
                    } // if
                } // if
#endif
            }
            else if (_usePolaris)
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
                var targetTerrain = Terrain.activeTerrain;
                if (targetTerrain != null)
                {
                    foreach (var layer in targetTerrain.terrainData.terrainLayers)
                    {
                        _textureNames.Add(layer.name);
                    } // foreach layer
                } // if
            }
        } // InitTextureNames()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            _mySerializedObject.Update();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            EditorGUILayout.LabelField("Elevation: ", _myTarget.elevation.ToString("F2"));
            EditorGUILayout.PropertyField(SceneViewModeProp);
            DrawTextureInfluences();

            EditorGUILayout.EndVertical();

            _mySerializedObject.ApplyModifiedProperties();
        } // OnInspectorGUI()

        void DrawTextureInfluences()
        {
            if (!Application.isPlaying) return;

            if (_myTarget.SceneViewMode)
            {
                EditorGUILayout.HelpBox("Uncheck Scene View Mode to see texture influences at runtime.", MessageType.Info);
                return;
            } // if

            // For each texture, show the texture name followed by the influence value
            for (int i = 0; i <  _myTarget.textureValues.Count; ++i)
            {
                if (_myTarget.textureValues[i] > 0f)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString() + ": ", GUILayout.Width(32));
                    EditorGUILayout.LabelField(_textureNames[i]);
                    EditorGUILayout.Space(3f);
                    EditorGUILayout.LabelField(string.Format("{0:N2}", _myTarget.textureValues[i]));
                    EditorGUILayout.EndHorizontal();
                    EditorUtils.HorizontalLine(Color.black, 2, Vector2.one);
                } // if
            } // foreach
        } // DrawTextureInfluences()

        void OnSceneGUI()
        {
            if (!_myTarget.SceneViewMode) return;

            // calculate the largest size used by a texture layer
            int maxContentWidth = -1;
            foreach (var textureName in _textureNames)
            {
                var width = (int)EditorUtils.guiStyle_MediumBold.CalcSize(new GUIContent(textureName)).x;
                maxContentWidth = Mathf.Max(width, maxContentWidth);
                _maxTextureNameWidth = Mathf.Max(_maxTextureNameWidth, textureName.Length);
            } // foreach

            var valueWidth = (int)GUI.skin.label.CalcSize(new GUIContent("   0.00")).x;

            Handles.BeginGUI();

            var areaWidth = (maxContentWidth + valueWidth) >= MIN_AREA_WIDTH ? (maxContentWidth + valueWidth) : MIN_AREA_WIDTH;
            GUILayout.BeginArea(new Rect(20, 20, areaWidth, _textureNames.Count * 32));

            var rect = EditorGUILayout.BeginVertical();
            GUI.color = new Color(0f, 1f, 0f, 0.75f);
            GUI.Box(rect, GUIContent.none, EditorUtils.boxStyleDark);

            GUI.color = Color.white;

            GUILayout.Label("Elevation: " + _myTarget.elevation.ToString("F2"), EditorUtils.guiStyle_MediumBold);

            for (int i = 0; i < _myTarget.textureValues.Count; ++i)
            {
                if (_myTarget.textureValues[i] > 0f)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(_textureNames[i].PadRight(_maxTextureNameWidth) + "  " + string.Format("{0:N2}", _myTarget.textureValues[i]), EditorUtils.guiStyle_MediumBold);
                    GUILayout.EndHorizontal();
                } // if
            } // for i

            EditorGUILayout.EndVertical();

            GUILayout.EndArea();

            Handles.EndGUI();
        } // OnSceneGUI()
    } // class QueryTerrainTexturesEditor
} // namespace Magique.SoulLink


