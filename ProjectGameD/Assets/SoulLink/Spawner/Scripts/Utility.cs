using System;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using static Magique.SoulLink.SoulLinkSpawnerTypes;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

#if MAPMAGIC2
using MapMagic.Core;
using MapMagic.Terrains;
#endif

#if GRIFFIN_2021
using Pinwheel.Griffin;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Utility : MonoBehaviour
    {
        static private SoulLinkSpawner _spawner;

        static private bool _useMapMagic = false;
        static private bool _usePolaris = false;

        static public List<string> TimeOfDayNames
        { get { return _timeOfDayNames; } }
        static private List<string> _timeOfDayNames;

        static public List<string> SpawnCategoryNames
        { get { return _spawnCategoryNames; } }
        static private List<string> _spawnCategoryNames;

        static public List<string> WeatherConditionNames
        { get { return _weatherConditionNames;  } }
        static private List<string> _weatherConditionNames = new List<string>();

        static public List<string> TextureNames
        { get { return _textureNames; } }
        static private List<string> _textureNames = new List<string>();

#if MAPMAGIC2
        static private MapMagicObject _mapMagicObject;
        static private DirectMatricesHolder[] _matricesHolders;
#endif

#if GRIFFIN_2021
        private GStylizedTerrain[] _terrains;
#endif

        static public List<string> QuestNames
        { get { return _questNames; } }

        static public BaseQuestManager QuestManager
        { get { return _questManager; } }
        static private BaseQuestManager _questManager;
        static private BaseQuest[] _quests;
        static private List<string> _questNames = new List<string>();

        static public void FindSpawnerComponents()
        {
            if (_spawner == null)
            {
                _spawner = FindObjectOfType<SoulLinkSpawner>();
            }

#if MAPMAGIC2
            _mapMagicObject = FindObjectOfType<MapMagicObject>();
            _matricesHolders = FindObjectsOfType<DirectMatricesHolder>();
            _useMapMagic = (_mapMagicObject != null && _matricesHolders.Length > 0);
#endif

#if GRIFFIN_2021
            _terrains = FindObjectsOfType<GStylizedTerrain>();
            _usePolaris = (_terrains != null && _terrains.Length > 0);
#endif

            // Find the quest manager if it exists in the scene
            _questManager = FindObjectOfType<BaseQuestManager>();
        } // FindSpawnerComponents()

        /// <summary>
        /// Initialize or re-initialize the list of time of day definitions
        /// </summary>
        static public void InitTODNames()
        {
            if (_spawner == null || _spawner.Database == null) return;

            if (_timeOfDayNames == null)
            {
                _timeOfDayNames = new List<string>();
            }
            else 
            {
                _timeOfDayNames.Clear();
            }

            foreach (var item in _spawner.Database.timeOfDayData)
            {
                if (item.timeOfDayName != "")
                {
                    _timeOfDayNames.Add(item.timeOfDayName);
                } // if
            } // foreach
        } // InitTODNames()

        /// <summary>
        /// Initialize or re-initialize the list of spawn categories
        /// </summary>
        static public void InitSpawnCategoryNames()
        {
            if (_spawner == null || _spawner.Database == null) return;

            if (_spawnCategoryNames == null)
            {
                _spawnCategoryNames = new List<string>();
            }
            else
            {
                _spawnCategoryNames.Clear();
            }

            if (_spawner.Database.spawnCategories.Count == 0)
            {
                _spawner.Database.Initialize();
            } // if

            foreach (var item in _spawner.Database.spawnCategories)
            {
                if (item.categoryName != "")
                {
                    _spawnCategoryNames.Add(item.categoryName);
                } // if
            } // foreach
        } // InitSpawnCategoryNames()

        /// <summary>
        /// Returns true if the defined time of day encompasses the entire day
        /// </summary>
        /// <param name="timeOfDayName"></param>
        /// <returns></returns>
        static public bool IsTimeOfDayAllDay(string timeOfDayName)
        {
            if (_spawner == null || _spawner.Database == null) return false;

            var tod = _spawner.Database.timeOfDayData.Find(e => e.timeOfDayName == timeOfDayName);
            if (tod != null)
            {
                return tod.startTime == 0f && tod.endTime == 23.99f;
            } // if

            return false;
        } // IsTimeOfDayAllDay()

        static public void InitWeatherConditionNames()
        {
            _weatherConditionNames.AddRange(Enum.GetNames(typeof(WeatherCondition)));
        } // InitWeatherConditionNames()

        static public  void InitTextureNames()
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

        static public void InitQuestData()
        {
            _questNames.Clear();

            if (_questManager != null)
            {
                _quests = FindObjectsOfType<BaseQuest>();
                if (_quests != null)
                {
                    foreach (var quest in _quests)
                    {
                        _questNames.Add(quest.GetQuestName());
                    } // foreach
                } // if
            } // if
        } // InitQuestData()

        /// <summary>
        /// ConvertPosition
        /// </summary>
        /// <param name="activeTerrain"></param>
        /// <param name="checkPosition"></param>
        /// <returns></returns>
        static Vector2 ConvertPosition(Terrain activeTerrain, Vector3 checkPosition)
        {
            Vector3 terrainPosition = checkPosition - activeTerrain.transform.position;

            Vector3 mapPosition = new Vector3
            (terrainPosition.x / activeTerrain.terrainData.size.x, 0,
            terrainPosition.z / activeTerrain.terrainData.size.z);

            float xCoord = mapPosition.x * activeTerrain.terrainData.alphamapWidth;
            float zCoord = mapPosition.z * activeTerrain.terrainData.alphamapHeight;

            return new Vector2(xCoord, zCoord);
        } // ConvertPosition()

        /// <summary>
        /// CheckTextures
        /// </summary>
        /// <param name="activeTerrain"></param>
        /// <param name="checkPos"></param>
        /// <returns></returns>
        static public float[,,] CheckTextures(Terrain activeTerrain, Vector3 checkPos)
        {
            var pos = ConvertPosition(activeTerrain, checkPos);
            return activeTerrain.terrainData.GetAlphamaps((int)pos.x, (int)pos.y, 1, 1);
        } // CheckTextures()

        /// <summary>
        /// Get the terrain containing the spawn position for Unity terrain
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="useMapMagic"></param>
        /// <returns></returns>
        static public Terrain TerrainContaining(Vector3 spawnPos, bool useMapMagic = false)
        {
            if (useMapMagic)
            {
#if MAPMAGIC2
                if (_mapMagicObject == null)
                {
                    _mapMagicObject = FindObjectOfType<MapMagicObject>();
                } // if

                var tile = _mapMagicObject.tiles.FindByWorldPosition(spawnPos.x, spawnPos.z);
                if (tile != null && tile.ActiveTerrain != null)
                {
                    return tile.ActiveTerrain;
                } // if

                return null;
#else
                return null;
#endif
            }
            else
            {
                var terrains = Terrain.activeTerrains;
                foreach (var tile in terrains)
                {
                    Bounds terrainWorldBounds = new Bounds(tile.terrainData.bounds.center + tile.transform.position, tile.terrainData.bounds.size);
                    if (terrainWorldBounds.extents.y < 2f) // edge case where terrain is nearly flat
                    {
                        spawnPos.y = 0;
                    } // if

                    if (terrainWorldBounds.Contains(spawnPos))
                    {
                        return tile;
                    } // if
                } // foreach

                return null;
            }
        } // TerrainContaining()

#if GRIFFIN_2021
        /// <summary>
        /// Check texture influences for Polaris mesh terrain
        /// </summary>
        /// <param name="activeTerrain"></param>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        static public float[,,] CheckPolarisTextures(GStylizedTerrain activeTerrain, Vector3 spawnPos)
        {
            var mapCount = activeTerrain.TerrainData.Shading.SplatControlMapCount;

            int res = activeTerrain.TerrainData.Shading.SplatControlResolution;
            float width = activeTerrain.TerrainData.Geometry.Width;
            float length = activeTerrain.TerrainData.Geometry.Length;

            // Convert x/z coordinates into 0,0 based
            float x = spawnPos.x - activeTerrain.transform.position.x;
            float z = spawnPos.z - activeTerrain.transform.position.z;

            float[,,] alphaMap = new float[1, 1, mapCount * 4];
            for (int i = 0; i < mapCount; ++i)
            {
                var color = activeTerrain.TerrainData.Shading.GetSplatControl(i).GetPixel((int)(x * (res / width)), (int)(z * (res / length)));

                alphaMap[0, 0, 0 + (i * 4)] = color.r;
                alphaMap[0, 0, 1 + (i * 4)] = color.g;
                alphaMap[0, 0, 2 + (i * 4)] = color.b;
                alphaMap[0, 0, 3 + (i * 4)] = color.a;
            } // for i

            return alphaMap;
        } // CheckPolarisTextures()

        /// <summary>
        /// Get the terrain containing the spawn position for Polaris mesh terrain
        /// </summary>
        /// <param name="terrains"></param>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        static public GStylizedTerrain PolarisTerrainContaining(GStylizedTerrain[] terrains, Vector3 spawnPos)
        {
            foreach (var tile in terrains)
            {
                spawnPos.y = tile.Bounds.center.y;

                if (tile.Bounds.Contains(spawnPos))
                {
                    return tile;
                } // if
            } // foreach

            return null;
        } // PolarisTerrainContaining()
#endif

        static private Vector2 _terrainMinMax = Vector2.zero;

        /// <summary>
        /// GetTerrainMinMax
        /// </summary>
        /// <returns></returns>
        static public Vector2 GetTerrainMinMax()
        {
            if (_terrainMinMax == Vector2.zero)
            {
                float min = float.MaxValue;
                float max = float.MinValue;
                var terrains = Terrain.activeTerrains;
                foreach (var tile in terrains)
                {
                    Bounds terrainWorldBounds = new Bounds(tile.terrainData.bounds.center + tile.transform.position, tile.terrainData.bounds.size);
                    min = Mathf.Min(min, terrainWorldBounds.min.y);
                    max = Mathf.Max(max, terrainWorldBounds.max.y);
                } // foreach

                _terrainMinMax = new Vector2(min, max);
            } // if

            return _terrainMinMax;
        } // GetTerrainMinMax()

#if MAPMAGIC2
        /// <summary>
        /// GetTerrainMinMax
        /// </summary>
        /// <returns></returns>
        static public Vector2 GetTerrainMinMax(TerrainTile terrainTile)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            var mainTerrain = terrainTile.GetTerrain(false);
            if (mainTerrain != null)
            {
                Bounds terrainWorldBounds = new Bounds(mainTerrain.terrainData.bounds.center + terrainTile.transform.position, mainTerrain.terrainData.bounds.size);
                min = Mathf.Min(min, terrainWorldBounds.min.y);
                max = Mathf.Max(max, terrainWorldBounds.max.y);

                _terrainMinMax = new Vector2(min, max);
            } // if

            return _terrainMinMax;
        } // GetTerrainMinMax()
#endif

        /// <summary>
        /// GetSlopeAngle
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="ignoreLayers"></param>
        /// <param name="radius"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        static public float GetSlopeAngle(Vector3 origin, LayerMask ignoreLayers, float radius = 0.25f, float distance = 1.25f)
        {
            RaycastHit hit;
            var pos = origin + new Vector3(0f, 1f, 0f);
            if (Physics.SphereCast(pos, radius, Vector3.down, out hit, distance, ~ignoreLayers.value))
            {
                // Angle of our slope (between these two vectors). 
                // A hit normal is at a 90 degree angle from the surface that is collided with (at the point of collision).
                // e.g. On a flat surface, both vectors are facing straight up, so the angle is 0.
                return Vector3.Angle(hit.normal, Vector3.up);
            } // if

            return 0f;
        } // GetSlopeAngle()

        /// <summary>
        /// Generate a random position from the specified origin. Options for 2D position.
        /// </summary>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="origin"></param>
        /// <param name="spawnMode"></param>
        /// <param name="spawnAxes"></param>
        /// <returns></returns>
        static public Vector3 GetRandomPos(float minRange, float maxRange, Vector3 origin, SpawnMode spawnMode = SpawnMode.SpawnIn3D, SpawnAxes spawnAxes = SpawnAxes.XY)
        {
            if (spawnMode == SpawnMode.SpawnIn3D)
            {
                float deg = Random.Range(0f, 360f); // randomize position along radius
                var rotation = Quaternion.AngleAxis(deg, Vector3.up);
                var forward = rotation * Vector3.forward;

                return origin + (forward * (minRange + Random.Range(0f, maxRange - minRange)));
            }
            else
            {
                float deg = Random.Range(0f, 360f); // randomize position along radius
                var rotation = Quaternion.AngleAxis(deg, Vector3.forward);
                var forward = rotation * Vector2.up;
                var newPos = origin + (forward * (minRange + Random.Range(0f, maxRange - minRange)));
                float a = 0f, b = 0f;

                switch (spawnAxes)
                {
                    case SpawnAxes.XY:
                        a = newPos.x;
                        b = newPos.y;
                        return new Vector3(a, b, newPos.z);
                    case SpawnAxes.XZ:
                        a = newPos.x;
                        b += newPos.z;
                        return new Vector3(a, newPos.y, b);
                    case SpawnAxes.YZ:
                        a = newPos.y;
                        b = newPos.z;
                        return new Vector3(newPos.x, a, b);
                } // switch
            }

            return Vector3.zero;
        } // GetRandomPos()

        static public Vector2 GetRandomPosInBounds(Bounds bounds, SpawnMode spawnMode = SpawnMode.SpawnIn3D, 
            SpawnAxes spawnAxes = SpawnAxes.XY)
        {
            if (spawnMode == SpawnMode.SpawnIn3D)
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float z = Random.Range(bounds.min.z, bounds.max.z);

                return new Vector2(x, z);
            }
            else
            {
                float a = 0f;
                float b = 0f;

                switch (spawnAxes)
                {
                    case SpawnAxes.XY:
                        a = Random.Range(bounds.min.x, bounds.max.x);
                        b = Random.Range(bounds.min.y, bounds.max.y);
                        break;
                    case SpawnAxes.XZ:
                        a = Random.Range(bounds.min.x, bounds.max.x);
                        b = Random.Range(bounds.min.z, bounds.max.z);
                        break;
                    case SpawnAxes.YZ:
                        a = Random.Range(bounds.min.y, bounds.max.y);
                        b = Random.Range(bounds.min.z, bounds.max.z);
                        break;
                } // switch

                return new Vector2(a, b);
            }
        } // GetRandomPosInBounds()

        /// <summary>
        /// Calculates the y position for the spawn by checking against walkable meshes that may be placed on the terrain.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        static public float GetAdjustedYPos(float x, float y, float z, float range = 30f)
        {
            //if (_spawnMode == SpawnMode.SpawnIn2D) return y;

            LayerMask navMeshIgnoreLayers = new LayerMask();
            if (SoulLinkSpawner.Instance != null)
            {
                navMeshIgnoreLayers = SoulLinkSpawner.Instance.NavMeshIgnoreLayers;
            } // if

            RaycastHit hit;
            // Does the ray intersect on any objects excluding the ignore raycast and triggers layer
            var origin = new Vector3(x, y + range, z);
            if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, ~navMeshIgnoreLayers.value))
            {
                if (SoulLinkGlobal.Instance.DebugMode)
                {
                    Debug.DrawRay(origin, Vector3.down * hit.distance, Color.yellow);
                } // if
                return hit.point.y;
            } // if

            return y;
        } // GetAdjustedYPos()

        /// <summary>
        /// Perform a basic check on nav mesh position to see if it's a valid position for Ai to move to
        /// </summary>
        /// <param name="navPos"></param>
        /// <returns></returns>
        static public Vector3 ValidateNavMeshPosition(Vector3 navPos, out bool isValid)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(navPos, out hit, 2.0f, NavMesh.AllAreas);
            isValid = hit.hit;

            return hit.position;
        } // ValidateNavMeshPosition()

        /// <summary>
        /// A function for converting real time seconds passed to a value expressed in Game Time. Uses SoulLinkManager if AI system is present, else
        /// uses SoulLinkSpawner
        /// </summary>
        /// <param name="realtimeSeconds"></param>
        /// <returns></returns>
        static public float GameTimeElapsed(float realtimeSeconds)
        {
            BaseGameTime gameTime = null;
//#if SOULLINK_AI
//            gameTime = SoulLinkManager.Instance.GameTime;
//#elif SOULLINK_SPAWNER
            gameTime = SoulLinkSpawner.Instance.GameTime;
//#endif
            var dayInSeconds = gameTime.DayLengthInMinutes * 60f;
            return (realtimeSeconds / dayInSeconds) / 60f; // calculated hours elapsed
        } // GameTimeElapsed()

        /// <summary>
        /// BuildTimeString
        /// </summary>
        /// <param name="timeValue"></param>
        /// <returns></returns>
        static public string BuildTimeString(float timeValue)
        {
            DateTime dt = new DateTime(2000, 1, 1, 13, 0, 0);
            var dtStr = dt.ToString(CultureInfo.CurrentCulture);
            bool is24HourTime = dtStr.Contains("13");

            float timeFrac = timeValue - (int)timeValue;
            int timeHour = (int)timeValue;

            if (is24HourTime)
            {
                return string.Format("{0}:{1:00}", timeHour, (int)((timeFrac * 60) + .01f));
            }

            // Convert from 24 hour float to 12 hour am/pm string with start and end time
            string ampmTag;
            if (timeValue >= 12)
            {
                ampmTag = " PM";
                if (timeValue >= 13)
                {
                    timeHour -= 12;
                } // if
            }
            else
            {
                if (timeHour == 0)
                {
                    timeHour = 12;
                } // if
                ampmTag = " AM";
            }

            return string.Format("{0}:{1:00}{2}", timeHour, (int)((timeFrac * 60) + .01f), ampmTag);
        } // BuildTimeString()

        /// <summary>
        /// Builds a date string from the current day, month, year data
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        static public string BuildDateString(IGameTime gameTime)
        {
            DateTime dt = new DateTime(gameTime.GetYear(), gameTime.GetMonth(), gameTime.GetDay());
            var dateOnly = dt.ToString().Split(' ');
            return dateOnly[0].ToString(CultureInfo.CurrentCulture);
        } // BuildDateString()

        /// <summary>
        /// Add a spawn action component to an game object by specifying the script name. Extend this as needed for your own custom actions.
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="gameObject"></param>
        static public void AddSpawnActionComponentByName(string scriptName, GameObject gameObject)
        {
            if (scriptName == "") return;

            BaseSpawnAction spawnAction = null;
            switch (scriptName)
            {
                case "SpawnAction_Fader":
                    if (gameObject.GetComponent<SpawnAction_Fader>() == null)
                    {
                        spawnAction = gameObject.AddComponent<SpawnAction_Fader>();
                    } // if
                    break;
                case "SpawnAction_Scaler":
                    if (gameObject.GetComponent<SpawnAction_Scaler>() == null)
                    {
                        spawnAction = gameObject.AddComponent<SpawnAction_Scaler>();
                    } // if
                    break;
                default:
                    Debug.LogError("Error: Cannot add unknown Spawn Action by the name of " + scriptName);
                    break;
            } // switch

            if (spawnAction != null && gameObject.GetComponent<ISpawnable>() != null)
            {
                gameObject.GetComponent<ISpawnable>().SetSpawnAction(spawnAction);
            } // if
        } // AddComponentByName()

        /// <summary>
        /// Import settings from a SpawnableSettingsObject
        /// </summary>
        /// <param name="spawnableSettings"></param>
        static public void ImportSpawnableSettings(ISpawnable spawnable, SpawnableSettingsObject spawnableSettings)
        {
            if (spawnableSettings.spawnAction != "")
            {
                AddSpawnActionComponentByName(spawnableSettings.spawnAction, spawnable.GetGameObject());
            } // if

            spawnable.SetRequiresNavMesh(spawnableSettings.requiresNavMesh);
            spawnable.SetIsPersistent(spawnableSettings.isPersistent);
//            spawnable.SetTexColorName(spawnableSettings.texColorName);
//            spawnable.SetPerformFading(spawnableSettings.performFading);
//            spawnable.SetFadeInLength(spawnableSettings.fadeInLength);
//            spawnable.SetFadeOutLength(spawnableSettings.fadeOutLength);
            spawnable.SetDespawnDelay(spawnableSettings.despawnDelay);
            spawnable.SetDespawnCheckInterval(spawnableSettings.despawnCheckInterval);
            spawnable.SetDespawnRangeBuffer(spawnableSettings.despawnRangeBuffer);
            spawnable.SetIgnoreTotalSpawns(spawnableSettings.ignoreTotalSpawns);
        } // ImportSpawnableSettings()

        /// <summary>
        /// Export settings to a new SpawnableSettingsObject
        /// </summary>
        static public void ExportSpawnableSettings(ISpawnable spawnable)
        {
#if UNITY_EDITOR
            // Have user enter a path and name for the new settings object
            string path = EditorUtility.SaveFilePanel("New Spawnable Settings Asset", Application.dataPath, "SpawnableSettings", "asset");
            if (path.Length != 0)
            {
                // strip off prefix path data so it starts with Assets. If Assets is not in the path then throw and error.
                int pos = path.IndexOf("Assets");
                if (pos == -1)
                {
                    if (EditorUtility.DisplayDialog(
                        "Error",
                        "Invalid path. Asset must be created within the Assets folder of your project.",
                        "OK"))
                    {
                    } // if
                }
                else
                {
                    var projectPath = path.Substring(pos, path.Length - pos);

                    // Create new SpawnableSettingsObject asset
                    SpawnableSettingsObject spawnableSettings = ScriptableObject.CreateInstance<SpawnableSettingsObject>();

                    spawnableSettings.requiresNavMesh = spawnable.GetRequiresNavMesh();
                    spawnableSettings.isPersistent = spawnable.GetIsPersistent();
//                    spawnableSettings.texColorName = spawnable.GetTexColorName();
//                    spawnableSettings.performFading = spawnable.GetPerformFading();
//                    spawnableSettings.fadeInLength = spawnable.GetFadeInLength();
//                    spawnableSettings.fadeOutLength = spawnable.GetFadeOutLength();
                    spawnableSettings.despawnDelay = spawnable.GetDespawnDelay();
                    spawnableSettings.despawnCheckInterval = spawnable.GetDespawnCheckInterval();
                    spawnableSettings.despawnRangeBuffer = spawnable.GetDespawnRangeBuffer();

                    AssetDatabase.CreateAsset(spawnableSettings, projectPath);

                    GUIUtility.ExitGUI();
                }
            } // if
#endif
        } // ExportSpawnableSettings()


        /// <summary>
        /// Move an item up in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public static void MoveItemUp<T>(IList<T> list, int index)
        {
            if (index == 0) return; // already as high as it can go

            Swap(list, index, index - 1);
        } // MoveItemUp()

        /// <summary>
        /// Move an item down in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public static void MoveItemDown<T>(IList<T> list, int index)
        {
            if (index == list.Count - 1) return; // already as low as it can go

            Swap(list, index, index + 1);
        } // MoveItemDown()

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        /// <summary>
        /// Given a list of type T, this method will shuffle the items randomly and return the shuffled list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(List<T> list)
        {
            var tempList = new List<T>();

            while (list.Count > 0)
            {
                int i = Random.Range(0, list.Count);
                tempList.Add(list[i]);
                list.RemoveAt(i);
            } // while

            return tempList;
        } // Shuffle

        /// <summary>
        /// Accurate floating point comparison function
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsEqual(float a, float b)
        {
            if (a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
            {
                return true;
            }
            else
            {
                return false;
            }
        } // IsEqual()

#if UNITY_EDITOR
        public static int GetNumberMotionsInBlendTree(Animator animator, int layer, string stateName)
        {
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            foreach (var state in animatorController.layers[layer].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    BlendTree blendTree = state.state.motion as BlendTree;
                    return blendTree.children.Length;
                } // if
            } // foreach

            return 0;
        } // GetNumberMotionsInBlendTree()
#endif

        public static Transform GetTransform(GameObject parent, string xformName)
        {
            string[] xformNames = { xformName };
            return GetTransform(parent, xformNames);
        } // GetTransform()

        public static Transform GetTransform(GameObject parent, string[] xformNames)
        {
            // Try to find the specified transform name
            Transform[] transforms = parent.GetComponentsInChildren<Transform>();
            for (int i = transforms.Length - 1; i > -1; --i)
            {
                var xform = transforms[i];
                foreach (var itemName in xformNames)
                {
                    if (xform.name.ToUpper().Contains(itemName))
                    {
                        return xform;
                    } // if
                } // foreach
            } // foreach

            return null;
        } // GetTransform()

        /// <summary>
        /// Calculate the bounds of the specified game object by encapsulating all child renderer bounds
        /// </summary>
        /// <param name="someObject"></param>
        /// <returns></returns>
        static public Bounds CalculateBounds(GameObject someObject)
        {
            Bounds bounds = new Bounds(someObject.transform.position, Vector3.zero);
            UnityEngine.Object[] renderers = someObject.GetComponentsInChildren(typeof(Renderer));
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            } // foreach

            return bounds;
        } // CalculateBounds()

        /// <summary>
        /// Return the super parent of the specified transform
        /// </summary>
        /// <param name="someObject"></param>
        /// <returns></returns>
        static public Transform GetSuperParent(Transform someObject)
        {
            if (someObject.parent == null) return someObject;

            return GetSuperParent(someObject.parent);
        } // GetSuperParent()
    } // class Utility
} // namespace Magique.SoulLink