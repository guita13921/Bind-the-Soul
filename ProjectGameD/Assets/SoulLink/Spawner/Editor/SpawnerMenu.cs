using UnityEditor;
using UnityEngine;
using System;

#if SOULLINK_USE_SKYMASTER
using Artngame.SKYMASTER;
#endif

#if HDRPTIMEOFDAY
using ProceduralWorlds.HDRPTOD;
#endif

#if SURVIVAL_TEMPLATE_PRO
using PolymindGames.WorldManagement;
#endif

#if SURVIVAL_ENGINE
using SurvivalEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

#if SOULLINK_USE_DS || SOULLINK_USE_QM
using PixelCrushers;
#endif

#if ENVIRO_3
using Enviro;
#endif

#if COZY_WEATHER
using DistantLands.Cozy;
#endif

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnerMenu : MonoBehaviour
    {
        private static SoulLinkSpawner _soulLinkSpawner;
        private static SoulLinkGlobal _soulLinkGlobal;
        private static SpawnAreaBounds _spawnAreaBoundsPrefab;

        [MenuItem("Tools/SoulLink/Spawner/Create Level Manager")]
        private static void CreateLevelManager()
        {
            GameObject go = new GameObject();
            go.name = "Level Manager";
            go.AddComponent<LevelManager>();
        } // CreateLevelManager()

        [MenuItem("Tools/SoulLink/Spawner/Setup Scene")]
        private static void SetupScene()
        {
            // Create a parent object to store all other components as children for better organization
            var global = Resources.FindObjectsOfTypeAll(typeof(SoulLinkGlobal));
            if (global.Length == 0)
            {
                GameObject go = new GameObject();
                go.name = "SoulLink";
                _soulLinkGlobal = go.AddComponent<SoulLinkGlobal>();
            }
            else 
            {
                _soulLinkGlobal = global[0] as SoulLinkGlobal;
            }

            var managers = Resources.FindObjectsOfTypeAll(typeof(SoulLinkSpawner));
            if (managers.Length == 0)
            {
                GameObject go = new GameObject();
                go.name = "SoulLink Spawner";
                _soulLinkSpawner = go.AddComponent<SoulLinkSpawner>();
                _soulLinkSpawner.transform.SetParent(_soulLinkGlobal.transform);
            }
            else
            {
                _soulLinkSpawner = managers[0] as SoulLinkSpawner;
            }

            SetupSkySystem();
            SetupPoolManager();
            SetupQuestSystem();
            SetupSpawnerSaver();
        } // SetupScene()

#if SOULLINK_USE_DS || SOULLINK_USE_QM
        [MenuItem("Tools/SoulLink/Spawner/Setup Save System Integration")]
        private static void SetupSaveSystem()
        {
            if (SetupSpawnerSaver())
            {
                bool regenIds = EditorUtility.DisplayDialog(
                    "Setup Save System",
                    "Do you want to regenerate unique Ids?",
                    "YES", "NO");

                // Add saver to all SpawnArea components in the scene
                var spawnAreas = FindObjectsOfType<SpawnArea>();
                foreach (var spawnArea in spawnAreas)
                {
                    if (!spawnArea.gameObject.GetComponent<SpawnArea_Saver>())
                    {
                        spawnArea.GenerateGuid();

                        spawnArea.gameObject.AddComponent<SpawnArea_Saver>();
                    } // if

                    if (string.IsNullOrEmpty(spawnArea.GetGuid()) || regenIds)
                    {
                        spawnArea.GenerateGuid();
                    } // if

                    var saver = spawnArea.gameObject.GetComponent<SpawnArea_Saver>();
                    if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                    {
                        saver.key = spawnArea.GetGuid();
                    } // if
                } // foreach

                // Add saver to all SpawnOnTrigger components in the scene
                var spawnOnTriggers = FindObjectsOfType<SpawnOnTrigger>();
                foreach (var spawnOnTrigger in spawnOnTriggers)
                {
                    if (!spawnOnTrigger.gameObject.GetComponent<SpawnOnTrigger_Saver>())
                    {
                        spawnOnTrigger.gameObject.AddComponent<SpawnOnTrigger_Saver>();
                    } // if

                    var saver = spawnOnTrigger.GetComponent<SpawnOnTrigger_Saver>();
                    if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                    {
                        saver.key = Guid.NewGuid().ToString();
                    } // if
                } // foreach

                SetupQuestSavers(regenIds);
                SetupGameTimeSaver();

                // Add saver to all LevelManager components in the scene
                var levelManagers = FindObjectsOfType<LevelManager>();
                foreach (var levelManager in levelManagers)
                {
                    if (!levelManager.gameObject.GetComponent<LevelManager_Saver>())
                    {
                        levelManager.GenerateGuid();

                        levelManager.gameObject.AddComponent<LevelManager_Saver>();
                    } // if

                    if (string.IsNullOrEmpty(levelManager.GetGuid()) || regenIds)
                    {
                        levelManager.GenerateGuid();
                    } // if

                    var saver = levelManager.gameObject.GetComponent<LevelManager_Saver>();
                    if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                    {
                        saver.key = levelManager.GetGuid();
                    } // if
                } // foreach

                // Add saver to all WaveManager components in the scene
                var waveManagers = FindObjectsOfType<WaveManager>();
                foreach (var waveManager in waveManagers)
                {
                    if (!waveManager.gameObject.GetComponent<WaveManager_Saver>())
                    {
                        waveManager.GenerateGuid();

                        waveManager.gameObject.AddComponent<WaveManager_Saver>();
                    } // if

                    if (string.IsNullOrEmpty(waveManager.GetGuid()) || regenIds)
                    {
                        waveManager.GenerateGuid();
                    } // if

                    var saver = waveManager.gameObject.GetComponent<WaveManager_Saver>();
                    if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                    {
                        saver.key = waveManager.GetGuid();
                    } // if
                } // foreach
            }
            else
            {
                if (EditorUtility.DisplayDialog(
                    "Error",
                    "There is no Save System in your scene.",
                    "OK"))
                {
                } // if
            }
        } // SetupSaveSystem()
#endif

        private static void SetupQuestSavers(bool regenIds = false)
        {
//#if SOULLINK_USE_DS || SOULLINK_USE_QM
#if SOULLINK_USE_QM
            var questManager = FindObjectOfType<BaseQuestManager>();
            if (questManager != null)
            {
                if (questManager.GetComponent<QuestManager_Saver>() == null)
                {
                    questManager.gameObject.AddComponent<QuestManager_Saver>();
                } // if

                var saver = questManager.gameObject.GetComponent<QuestManager_Saver>();
                if (string.IsNullOrEmpty(saver._internalKeyValue))
                {
                    saver.key = "QuestManager";
                } // if
            } // if
#endif

#if SOULLINK_USE_QM
            {
                // Add saver to all QMQuest components in the scene
                var quests = FindObjectsOfType<QMQuest>();
                foreach (var quest in quests)
                {
                    // Only add a Quest_Saver to components not under a QuestManager collection
                    if (quest.GetComponent<ISoulLinkQuestManager>() == null)
                    {
                        if (!quest.gameObject.GetComponent<Quest_Saver>())
                        {
                            quest.gameObject.AddComponent<Quest_Saver>();
                        } // if

                        var saver = quest.gameObject.GetComponent<Quest_Saver>();
                        if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                        {
                            saver.key = Guid.NewGuid().ToString();
                        } // if
                    } // if
                } // foreach
            }
#endif

#if SOULLINK_USE_DS
            {
                // Add saver to all DSQuest components in the scene
                var quests = FindObjectsOfType<DSQuest>();
                foreach (var quest in quests)
                {
                    // Only add a Quest_Saver to components not under a QuestManager collection
                    if (quest.GetComponent<ISoulLinkQuestManager>() == null)
                    {
                        if (!quest.gameObject.GetComponent<Quest_Saver>())
                        {
                            quest.gameObject.AddComponent<Quest_Saver>();
                        } // if

                        var saver = quest.gameObject.GetComponent<Quest_Saver>();
                        if (string.IsNullOrEmpty(saver._internalKeyValue) || regenIds)
                        {
                            saver.key = Guid.NewGuid().ToString();
                        } // if
                    }
                } // foreach
            }
#endif
        } // SetupQuestSavers()

        private static bool SetupSpawnerSaver()
        {
#if SOULLINK_USE_DS || SOULLINK_USE_QM
            if (IsSaveSystemPresent())
            {
                // Add saver component to SoulLinkSpawner
                var spawner = FindObjectOfType<SoulLinkSpawner>();
                if (spawner != null)
                {
                    if (!spawner.gameObject.GetComponent<SoulLinkSpawner_Saver>())
                    {
                        spawner.gameObject.AddComponent<SoulLinkSpawner_Saver>();
                    } // if
                } // if

                return true;
            }

            return false;
#else
			return false;
#endif
        } // SetupSpawnerSaver()

        /// <summary>
        /// Check for Pixel Crusher's Save System in the scene
        /// </summary>
        /// <returns></returns>
        private static bool IsSaveSystemPresent()
        {
#if SOULLINK_USE_DS || SOULLINK_USE_QM
            // Check for Pixel Crusher's Save System in the scene
            var saveSystem = Resources.FindObjectsOfTypeAll(typeof(SaveSystem));
            return (saveSystem.Length != 0);
#else
            return false;
#endif
        } // IsSaveSystemPresent()

        private static void SetupQuestSystem()
        {
#if SOULLINK_USE_DS
            {
                // Try to find a ISoulLinkQuestManager interface in the scene
                var questManager = Resources.FindObjectsOfTypeAll(typeof(DSQuestManager));
                if (questManager.Length == 0)
                {
                    var ds = Resources.FindObjectsOfTypeAll(typeof(PixelCrushers.DialogueSystem.DialogueSystemController));
                    if (ds.Length != 0)
                    {
                        GameObject questSystem = new GameObject();
                        questSystem.name = "Quest Manager";
                        questSystem.AddComponent<DSQuestManager>();
                        questSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SOULLINK_USE_QM
            {
                // Try to find a ISoulLinkQuestManager interface in the scene
                var questManager = Resources.FindObjectsOfTypeAll(typeof(QMQuestManager));
                if (questManager.Length == 0)
                {
                    var qm = Resources.FindObjectsOfTypeAll(typeof(PixelCrushers.QuestMachine.QuestMachineConfiguration));
                    if (qm.Length != 0)
                    {
                        GameObject questSystem = new GameObject();
                        questSystem.name = "Quest Manager";
                        questSystem.AddComponent<QMQuestManager>();
                        questSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif
            SetupQuestSavers();
        } // SetupQuestSystem()

        private static void SetupSkySystem()
        {
#if SOULLINK_USE_TOD
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(TODGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(TOD_Sky));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as TOD_Sky).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Time of Day Time";
                            timeSystem.AddComponent<TODGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                // There is no weather system for Time of Day
            }
#endif

#if UNISTORM_PRESENT
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(UniStormGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(UniStorm.UniStormSystem));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "UniStorm Time";
                        timeSystem.AddComponent<UniStormGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(UniStorm.UniStormSystem)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(UniStormWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "UniStorm Weather";
                        weatherSystem.AddComponent<UniStormWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SOULLINK_USE_AZURE
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(AzureGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.AzureSky.AzureTimeController));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as UnityEngine.AzureSky.AzureTimeController).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Azure Time";
                            timeSystem.AddComponent<AzureGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(UnityEngine.AzureSky.AzureTimeController)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(AzureWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Azure Weather";
                        weatherSystem.AddComponent<AzureWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if LANDSCAPE_BUILDER
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(LBGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(LandscapeBuilder.LBLighting));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as LandscapeBuilder.LBLighting).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Landscape Builder Time";
                            timeSystem.AddComponent<LBGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(LandscapeBuilder.LBLighting)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(LBWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Landscape Builder Weather";
                        weatherSystem.AddComponent<LBWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if WEATHER_MAKER_PRESENT
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(WeatherMakerGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(DigitalRuby.WeatherMaker.WeatherMakerScript));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as DigitalRuby.WeatherMaker.WeatherMakerScript).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Weather Maker Time";
                            timeSystem.AddComponent<WeatherMakerGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(DigitalRuby.WeatherMaker.WeatherMakerPrecipitationManagerScript)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(WeatherMakerWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Weather Maker Weather";
                        weatherSystem.AddComponent<WeatherMakerWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if ENVIRO_LW || ENVIRO_HD
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(EnviroGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(EnviroSkyMgr));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Enviro Time";
                        timeSystem.AddComponent<EnviroGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(EnviroSkyMgr)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(EnviroWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Enviro Weather";
                        weatherSystem.AddComponent<EnviroWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if ENVIRO_3
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(Enviro3GameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(EnviroManager));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Enviro 3 Time";
                        timeSystem.AddComponent<Enviro3GameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(EnviroManager)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(Enviro3WeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Enviro 3 Weather";
                        weatherSystem.AddComponent<Enviro3WeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if JUPITER
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(JupiterGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(Pinwheel.Jupiter.JDayNightCycle));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as Pinwheel.Jupiter.JDayNightCycle).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Jupiter Time";
                            timeSystem.AddComponent<JupiterGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                // There is no weather system for Jupiter
            }
#endif

#if GAIA_PRO_PRESENT
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(GaiaGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(Gaia.GaiaSceneLighting));
                    if (sky.Length != 0 && sky[0] != null)// && sky[0].activeInHierarchy)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Gaia Time";
                        timeSystem.AddComponent<GaiaGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(Gaia.ProceduralWorldsGlobalWeather)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(GaiaWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Gaia Weather";
                        weatherSystem.AddComponent<GaiaWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if HDRPTIMEOFDAY
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(GaiaGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(HDRPTimeOfDay));
                    if (sky.Length != 0 && sky[0] != null)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "HDRP Time";
                        timeSystem.AddComponent<GaiaGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SOULLINK_USE_TENKOKU
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(TenkokuGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(Tenkoku.Core.TenkokuModule));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Tenkoku Time";
                        timeSystem.AddComponent<TenkokuGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(Tenkoku.Core.TenkokuModule)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(TenkokuWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Tenkoku Weather";
                        weatherSystem.AddComponent<TenkokuWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SOULLINK_USE_EXPANSE
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(ExpanseGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(Expanse.DateTimeController));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Expanse Time";
                        timeSystem.AddComponent<ExpanseGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                // There is no weather system for Expanse
            }
#endif

#if SURVIVAL_ENGINE || FARMING_ENGINE
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(SurvivalEngineGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(TheGame));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as TheGame).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
#if SURVIVAL_ENGINE
                            timeSystem.name = "Survival Engine Time";
#else
                            timeSystem.name = "Farming Engine Time";
#endif
                            timeSystem.AddComponent<SurvivalEngineGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(WeatherSystem)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(SurvivalEngineWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
#if SURVIVAL_ENGINE
                        weatherSystem.name = "Survival Engine Weather";
#else
                        weatherSystem.name = "Farming Engine Weather";
#endif
                        weatherSystem.AddComponent<SurvivalEngineWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SURVIVAL_TEMPLATE_PRO
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(STPGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(WorldManager));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "STP Time";
                        timeSystem.AddComponent<STPGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                // There is no weather system for Survival Template Pro
            }
#endif

#if SOULLINK_USE_SKYMASTER
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(SkyMasterGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(SkyMasterManager));
                    if (sky.Length != 0)
                    {
                        var skyGO = (sky[0] as SkyMasterManager).gameObject;
                        if (skyGO != null)
                        {
                            GameObject timeSystem = new GameObject();
                            timeSystem.name = "Sky Master Time";
                            timeSystem.AddComponent<SkyMasterGameTime>();
                            timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                        } // if
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(SkyMasterManager)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(SkyMasterWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Sky Master Weather";
                        weatherSystem.AddComponent<SkyMasterWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if COZY_WEATHER
            {
                // Try to find a IGameTime interface in the scene
                var gametime = Resources.FindObjectsOfTypeAll(typeof(CozyGameTime));
                if (gametime.Length == 0)
                {
                    var sky = Resources.FindObjectsOfTypeAll(typeof(CozyWeather));
                    if (sky.Length != 0)
                    {
                        GameObject timeSystem = new GameObject();
                        timeSystem.name = "Cozy Time";
                        timeSystem.AddComponent<CozyGameTime>();
                        timeSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if

                if (Resources.FindObjectsOfTypeAll(typeof(CozyWeather)).Length != 0)
                {
                    if (Resources.FindObjectsOfTypeAll(typeof(CozyWeatherSystem)).Length == 0)
                    {
                        GameObject weatherSystem = new GameObject();
                        weatherSystem.name = "Cozy Weather";
                        weatherSystem.AddComponent<CozyWeatherSystem>();
                        weatherSystem.transform.SetParent(_soulLinkGlobal.transform);
                    } // if
                } // if
            }
#endif

#if SOULLINK_USE_DS || SOULLINK_USE_QM
            if (IsSaveSystemPresent())
            {
                SetupGameTimeSaver();
            } // if
#endif
        } // SetupSkySystem()

#if SOULLINK_USE_DS || SOULLINK_USE_QM
        private static void SetupGameTimeSaver()
        {
            var gameTime = FindObjectOfType<BaseGameTime>();
            if (gameTime != null)
            {
                if (gameTime.GetComponent<GameTime_Saver>() == null)
                {
                    gameTime.gameObject.AddComponent<GameTime_Saver>();
                } // if

                var saver = gameTime.gameObject.GetComponent<GameTime_Saver>();
                if (string.IsNullOrEmpty(saver._internalKeyValue))
                {
                    saver.key = "GameTime";
                } // if

            } // if
        } // SetupGameTimeSaver()
#endif

        private static void SetupPoolManager()
        {
#if SOULLINK_USE_POOLBOSS
            if (Resources.FindObjectsOfTypeAll(typeof(PoolManager_PoolBoss)).Length == 0)
            {
                GameObject poolManager = new GameObject();
                poolManager.name = "PoolManager";
                poolManager.AddComponent<PoolManager_PoolBoss>();
                poolManager.transform.SetParent(_soulLinkGlobal.transform);
            }
#else
            if (Resources.FindObjectsOfTypeAll(typeof(PoolManager_SoulLink)).Length == 0)
            {
                GameObject poolManager = new GameObject();
                poolManager.name = "PoolManager";
                poolManager.AddComponent<PoolManager_SoulLink>();
                poolManager.transform.SetParent(_soulLinkGlobal.transform);
            } // if
#endif
        } // SetupPoolManager()
    } // class SpawnerMenu
} // namespace Magique.SoulLink

