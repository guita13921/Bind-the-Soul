#if MAPMAGIC2
using MapMagic.Core;
#endif
#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// QueryTerrainTextures
// A script to query the texture influences at the object's current location. This is useful for debugging biome issues in your scene.
// Attach to your player or mobile camera and watch the inspector values at runtime to see what the current texture influences are.
// Or attach to an object in your scene and move your mouse over your terrain in the scene view to see the texture influences.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Query Terrain Texture")]
    [ExecuteInEditMode]
    public class QueryTerrainTextures : MonoBehaviour
    {
        public bool SceneViewMode
        { get { return _sceneViewMode; } }

        [Tooltip("Enable scene view mode in order to see texture influences by moving the mouse over your terrain in the scene view.")]
        [SerializeField]
        private bool _sceneViewMode = false;

        private bool _useMapMagic = false;
        private bool _usePolaris = false;
        public List<float> textureValues = new List<float>();
        public float elevation = 0f;

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

        private void Awake()
        {
#if MAPMAGIC2
            _useMapMagic = (FindObjectOfType<MapMagicObject>() != null);
#endif

#if GRIFFIN_2021
            _terrains = FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif
        } // Awake()

        void Update()
        {
            if (!Application.isPlaying || _sceneViewMode) return;

            if (_usePolaris)
            {
#if GRIFFIN_2021
                GetPolarisTerrainTextures(transform.position);
#endif
            }
            else
            {
                GetTerrainTextures(Utility.TerrainContaining(transform.position, _useMapMagic), transform.position);
            }

            if (!_sceneViewMode)
            {
                elevation = transform.position.y;
            }
        } // Update()

        public void GetTerrainTextures(Terrain terrain, Vector3 pos)
        {
            textureValues.Clear();
            if (terrain == null) return;

            float[,,] aMap = Utility.CheckTextures(terrain, pos);
            for (int i = 0; i < aMap.Length; ++i)
            {
                textureValues.Add(aMap[0, 0, i]);
            } // for i
        } // GetTerrainTextures()

#if GRIFFIN_2021
        void GetPolarisTerrainTextures(Vector3 pos)
        {
            textureValues.Clear();

            // Verify terrains are still valid, else get the references again
            foreach (var tile in _terrains)
            {
                if (tile == null)
                {
                    _terrains = FindObjectsOfType<GStylizedTerrain>();
                    break;
                } // if
            } // foreach

            // Get active terrain based on world position
            GStylizedTerrain activeTerrain = Utility.PolarisTerrainContaining(_terrains, pos);

            if (activeTerrain == null)
            {
                Debug.LogError("No Active Terrain at position = " + pos);
                return;
            } // if

            var mapCount = activeTerrain.TerrainData.Shading.SplatControlMapCount;

            int res = activeTerrain.TerrainData.Shading.SplatControlResolution;
            float width = activeTerrain.TerrainData.Geometry.Width;
            float length = activeTerrain.TerrainData.Geometry.Length;

            // Convert x/z coordinates into 0,0 based
            float x = pos.x - activeTerrain.transform.position.x;
            float z = pos.z - activeTerrain.transform.position.z;

            for (int i = 0; i < mapCount; ++i)
            {
                var color = activeTerrain.TerrainData.Shading.GetSplatControl(i).GetPixel((int)(x * (res / width)), (int)(z * (res / length)));

                textureValues.Add(color.r);
                textureValues.Add(color.g);
                textureValues.Add(color.b);
                textureValues.Add(color.a);
            }
        } // GetPolarisTerrainTextures()
#endif

#if UNITY_EDITOR
        void OnEnable()
        {
            SceneView.duringSceneGui += OnScene;
        } // OnEnable()

        void OnScene(SceneView scene)
        {
            if (!_sceneViewMode) return;

            Event e = Event.current;
            if (e.type == EventType.MouseMove)
            {
                Vector3 mousePos = e.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
                mousePos.x *= ppp;

                Vector3 pos = Vector3.zero;
                Ray ray = scene.camera.ScreenPointToRay(mousePos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.DrawLine(transform.position, hit.point);
                    pos = hit.point;
                    elevation = pos.y;
                } // if

                if (_usePolaris)
                {
#if GRIFFIN_2021
                GetPolarisTerrainTextures(pos);
#endif
                }
                else
                {
                    GetTerrainTextures(Utility.TerrainContaining(pos, _useMapMagic), pos);
                }

                SceneView.RepaintAll();
            } // if
        } // OnScene()
#endif
    } // class QueryTerrainTextures
} // namespace Magique.SoulLink