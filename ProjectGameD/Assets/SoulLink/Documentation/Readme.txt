README

SoulLink Spawner is a stand-alone asset designed for comprehensive procedural spawning. SoulLink Spawner is also included in the upcoming SoulLink Artificial Intelligence System to be released later this year.

(c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

Version 1.3.8

Integrations
This package includes all available integrations for SoulLink Spawner. They will not cause compiler errors, but some 
integrations require activation from the SoulLink Integrations window. Please see the Integration Guide and Quick Start Guide for 
details.

Getting Started
Go to the SoulLink/Documentation folder and start with the SoulLink Installation Guide followed by the Integration Guide and finally, 
the SoulLink Spawner - Quick Start Guide. When you have gone through those documents, refer to the SoulLink Spawner - Manual for 
more detailed information.

Support
For support, please direct all question to the SoulLink channel on Discord. Or e-mail support@magiqueproductions.com.

Versions
(1.3.8)
-Fixed issues with Herd spawning in biomes where herds would no longer spawn after roaming the world for a short period of time.
-Added check to prevent herd spawning if the minimum herd population can't be achieved

(1.3.7)
-Removed non-asset gizmos that were accidentally included

(1.3.6)
-Fixed issue with WaveItemRepeatConditional not spawning conditional prefabs
-Fixed compiler error for Dialogue System, Enviro 3, and UniStorm integrations

(1.3.5)
-Fix for Spawnable_EmeraldAI compiler error

(1.3.4)
-Internal editor improvements
-Restructured folders to pave the way for assembly definition support
-Removed SoulLinkTypes and moved any Spawner related structures into SoulLinkSpawnerTypes
-Changed all Reset methods to ResetToDefault in order to fix conflict with MonoBehaviour Reset method
-Add Allow Despawn In Combat flag to Spawnable and inherited classes.
-Fixed various issues with Malbers Animal Controller integration

(1.3.3)
-Fixed compiler issue for Survival Template Pro integration

(1.3.2)
-Fixed issue with SpawnArea incorrectly spawning outside the proper min/max radii

(1.3.1)
-Added COZY: Stylized Weather 2 integration

(1.3.0)
-Updated minimum Unity version to 2021.3.27f1
-Added SoulLink component menu with all commonly used SoulLink components
-Added 2D spawning option to SoulLinkSpawner component
-Added FlexCollider script for dual purpose 2D/3D collision
-Added 2D spawning by Biome and by Area
-Added Cluster Range option to SoulLinkSpawner
-Added Cluster Range to SpawnArea
-Added Use Spawn Circles option so this feature can be turned off if desired
-Added Min Altitude for Spawn In Air
-Added advanced wave spawner components
-Added Spawnable_MAC component for Malbers Animal Controller integration
-Added confirmation dialog when deleting a spawn or spawn area.
-Added up/down buttons to inspectors in order to move various items up or down in their respective lists
-Added new Variants spawn type
-Added Survival Template Pro time of day integration
-Added support for Gaia Pro HDRP Time of Day and stand-alone HDRP Time of Day
-Added support for Sky Master Ultimate
-Added Enviro 3 auto integration support
-Added Enabled property to SpawnArea to allow an area to be disabled at runtime, which despawns active AI and removes placeholders
-Added elevation range option to biomes
-Added Use Local Spawn Data option to SpawnArea so that all spawn information can be defined locally instead of in the main SoulLinkSpawner component
-Added Spawnable_SurvivalEngine component and can now automatically add to spawns
-Added ChangeArea method to SpawnArea so that an area can be changed at runtime to a different named area.
-Added PlayerSpawner and PlayerSpawn scripts
-Added Global Snow integration
-Added Season filters to spawns
-Added the ability to utilize spawned quest items and check for quest completion on despawn
-Added Volume Filters to Biomes
-Added Ignore Total Spawns flag to spawn settings so marked spawns do not count toward total spawns
-Added new Spline spawning method for SpawnArea and WaveSpawner
-Added Dreamteck Splines integration
-Added Copy/Paste feature for spawns
-Added Paused member to SoulLinkSpawner in order to pause/unpasue all spawners
-Added Category feature to allow spawns to be grouped together and have their own Max Spawns.
-Added ISoulLinkGuid interface and modified components to use this interface instead of their own version
-Added Game Time timestamp to debug logs
-Removed edit time PoolManager auto-modifications by SoulLinkSpawner.
-Removed obsolete convert spawn bounds menu option
-Removed Fading feature and replaced with customizable Spawn Action system with Fader and Scaler as ready options available
-Removed dependency on Unity's AI Navigation package for general use. Allows it to be enabled only as needed for runtime nav mesh generation.
-Modified PoolManager_SoulLink so that inactive spawns are child objects instead of under the scene root.
-Modified SpawnArea so that if SuppressBiomeSpawns is modified at runtime that it will dynamically change its behavior.
-Updated QuestManager so that quests can be defined on QuestManager itself instead of multiple scene objects
-Changed Disable toggles for spawns, areas, and biomes to show a checkmark if enabled and blank if disabled.
-Made editor improvements for easier usage
-Fixed Azure sky build failure issue
-Fixed SpawnArea reset so that any remnant AI are despawned when the trigger is reset.
-Fixed issue with SpawnArea that would not ever complete spawning if the spawn interval was high and the player eliminated AI before max spawns was reached.
-Fixed issue where SpawnArea and WaveSpawner colliders were continuously turning on/off
-Fixed issue where QMQuest could get null reference exception when there is no Quest Journal in the scene
-Fixed lag issue where profiler spikes would be seen when spawning/despawning instances
-Fixed issue with QueryTerrainTextures scene view where output was truncated if texture names were very short

NOTES: 
You should open any existing scenes and click on the PoolManager object to trigger a proper refresh of the data. You will no longer see the objects that spawner will create object pools for except those custom categories you edit manually yourself. So, don't panic if it looks empty when editing your scene.

The Max Spawns field on the main SoulLink Spawner component is now removed. When you first use this version, it will auto-add a Default category and all your existing spawns will be in that category. Each category has its own Max Spawns value and it defaults to 50. When using the SpawnerDebugCanvas, the total spawns listed will be the total spawns for all categories combined.

This version will break existing prefabs that used to use the built-in fading feature. For existing prefabs, you will need to add the SpawnAction_Fader component to the prefab and then assign that script in the Spawnable's Spawn Action field. If you are also using the SpawnableSettingsObject to import spawnable settings, that will be broken as well. You'll have to update your settings object to specify the correct SpawnAction script. You will not be able to automatically assign the SpawnAction_Fader properties though in this version.

(1.2.18)
Fixed issue with auto integration of Enviro3 that was accidentally reverted.

(1.2.17)
Fixed issue with running out of placeholders when using Spawn Outside Player FOV feature with spawns that Ignore Total Spawns

(1.2.16)
Fixed auto integration with Enviro 3

(1.2.15)
Fixed compiler error with ActivateQuest method for DSQuestManager

(1.2.14)
Fixed Pixel Crusher save system integration for Malbers Animal Controller

(1.2.13)
Added 'Spawn Interval' property to SpawnArea
Added 'Auto Trigger Reset' property to SpawnArea
Implemented ActivateQuest method in DSQuestManager
Updated Enviro 3 integration so it is auto detected
Fixed lag issue where profiler spikes would be seen when spawning/despawning instances
Fixed issue with BaseQuest.ContainsPrefab returning true when no entries are defined
Fixed issue with Biome definitions not working in non-Polaris scenes when Polaris is installed
Fixed issue with AI scaling being too big after loading from a saved game
Fixed issue with Invector FSM AI integration that was checking for in combat status for non-combat AI

(1.2.12)
Added Enviro 3 support
Added Malbers Animal Controller support
Fixed AzureGameTime SetMinute method to properly set the current minute.
Updated support for Invector FSM AI Version 1.1.8b

(1.2.11)
-Fixed issue where SoulLinkGlobal.Instance.PlaySound method would change the SoulLinkGlobal object's position in world space.
-Fixed runtime error for population debug logging if spawns are spawning outside player FOV

(1.2.10)
-Fixed issue with SpawnArea that may fail to spawn the correct quantity of spawns, especially when the area has only a single spawn.
-Fixed issue with SpawnerDebugCanvas that was not properly rendered with different aspect ratios
-Fixed issue with QMQuest and DSQuest not retaining their serialized data sometimes on project reload

(1.2.9)
-Fixed QMQuestManager IsQuestActive method, which would always return true if any quest was active.

(1.2.8)
-Fixed issue with AI not despawning when the quest filters are no longer applicable
-Fixed issue with SoulLinkGameTime not properly updating to the correct season

(1.2.7)
-Added Marked Spawns As Persistent field to SpawnArea
-Fixed Quest_Saver issue not properly saving entry complete data

(1.2.6)
-Fixed issue with Spawn Outside Player FOV not working when scene view is visible

(1.2.5)
-Fixed issue where spawns would diminish and eventually vanish completely after a length of time in the game.

(1.2.4)
-Added DisableAgentOnDespawn option to Spawnable to accommodate Opsive Behavior Designer entities.

(1.2.3)
-Fixed issue with herds not distributing their spawns properly by probability and blocking cpu during herd spawning
-Fixed issue with demo database Afternoon time having incorrect end time
-Fixed issue with time not syncing immediately at startup

(1.2.2)
-Fixed random number seed value, which was always using 0
-Fixed issue where an invalid navmesh position was still allowing spawning
-Added initial support for GKC AI
-Added Reusable flag to spawns to prevent re-use of a spawn
-Fixed issue where Add Herd button was not available if you removed all herd assets from your project.

(1.2.1)
-Changed SpawnArea bounds system to use a more flexible object that can be scaled and rotated in the scene view
-Added a Convert Spawn Area Bounds tools menu option for fixing up old areas to use the new bounds system
-Added auto conversion code to fix old spawn bounds to use the new system whenever the SpawnArea is viewed in the inspector
-Changed spawn radius color for Spawn Points to blue to avoid confusion with spawn trigger radius
-Added Spawn Boss option to SpawnArea
-Added option to regenerate unique Ids when running the Setup Save System Integration option from the Tools menu 
-Fixed issue with SoulLinkGlobal PlaySound method that would sometimes fail to play 2D sounds
-Fixed issue with Spawnable using bad Guid reference in OnDeath handler

(1.2.0)
-Added Suppression Radius field to SpawnArea
-Fixed issue with FailSpawn methods trying to reference a null object when shutting down game
-Moved GetRandomPos function to Utility class so it can be accessed across all SoulLink systems
-Added Spawn and Despawn methods to SoulLinkGlobal for custom use scenarios
-Improved visibility of scene view query terrain textures information
-Added Delay Enable Agent field to Spawnable and EnableAgent to ISpawnable interface. Implemented override in Spawnable_EmeraldAI.
-Added season-based peak temperatures to the weather system for assets that don't support temperature.
-Added SpawnOnTrigger script. 
-Added SpawnOnTrigger_Saver component
-Added SoulLinkSpawner_Saver to support saving and restoring active spawns.
-Changed all Spawnable classes to partial classes
-Added ISpawnableSerializer interface
-Changed Spawnable's reference to SpawnArea to a Guid instead of a direct reference.
-Added SpawnArea_Saver component for DS and QM integrations
-Added SetSecondsToReset, SetTotalResets, and SetSecondsElapsed methods to SpawnArea
-Added GetSecondsAdded to IGameTime and implemented in BaseGameTime
-Added UpdateSpawnAreas method to SoulLinkSpawner so area reset times can be updated after crossing scenes.
-Added automatic QuestManager setup from Setup Scene menu item if QM is detected in the scene
-Changed SpawnArea to be multi-object editable in the inspector
-Fixed null reference exception when Spawn Type is set to Herd before assigning the herd object
-Added parameter to SoulLinkSpawner Spawn method to prevent random rotation on spawn.
-Added spawnRot parameter to SpawnData
-Fixed EnviroGameTime so it can properly update hour and minute from GameTime_Saver
-Fixed SoulLinkGameTime to properly update season
-Various editor improvements

(1.1.0)
-Refactored quest scripts to accommodate Quest Machine
-Added Quest Machine integration
-Made Quest Saver and Game Time Saver components common to both Quest Machine and Dialogue System
-Added Quest Filters for spawning by Biome and by Area
-Removed Is Active property from Quest components. This is now solely based on active status in the third party quest system
-Fixed issue with spawn probability being ignored
-Fixed additional issue with copying Gizmos files
-Added support for Polaris mesh terrain biomes from splats
-Modified QueryTerrainTextures script to allow for scene view mode
-Added Spawn in Air feature
-Moved QueryTerrainTextures scripts out of Bonus and into the main Scripts folder. Removed Bonus folder.
-Added Farming Engine support
-Added additional debug logging for spawn failure conditions related to current time of day and current biomes
-Added Proximity Spawns
-Various editor improvements
-Fixed issue with PoolManager_SoulLink trying to create an object pool for a missing prefab
-Improved spawn generation so populations are more representative of the spawn probabilities
-Added Pause and UnPause to IGameTime interface.
-Added Day, Month, and Year integration to game time systems
-Added date to the debug canvas
-Fixed editor for Landscape Builder Stencil filters so the field labels are not cut off
-Fixed issue where Invector FSM AI could despawn when in combat
-Fixed warning for gizmo file conflict
-Fixed SoulLinkGlobal script so MasterAudio integration will not generate errors
-Added Hide/Show toggle button for Area spawns
-Made Spawnable component multi-object editable
-Added AssignPlayer script for scenes where the player spawns at runtime
-Spawn AI instances under their associated SpawnArea gameObject
-Fixed issue where AI could still despawn when AllowDespawn is false 
-Fixed issue where SpawnArea could trigger even if weather filters were not valid, thus causing spawns to never generate
-Made some editor improvements with indents and margin adjustments
-Fixed issue with LBGameTime null reference exception
-Verified that Spawner is compatible with SECTR Stream
-Updated QueryTerrainTextures script to work with Polaris mesh terrain splats
-Exposed the Spawn Circles property of SoulLinkSpawner
-Added user definable default population cap
-Added Hide All/Show All Button for Spawns by Biome and Spawns by Area
-Fixed issue where the Show/Hide status of biomes was not retained if component loses focus.
-Added SpawnValidator Probability failure reason
-Exposed Day Length In Minutes on BaseGameTime
-Added new SpawnArea reset capabilities based on duration and maximum repeat values
-Added Perform Fading option for Spawnable. This prevents any fade in/fade out operations on spawning AI.
-Exposed texture filters in Spawns by Area
-Added Proximity Filters to spawns. Added new ProximityFilter failure reason for spawn validator.
-Added version information to SoulLinkGlobal inspector
-Added Elevation to QueryTerrainTextures script
-Fixed issues with Spawn Outside Player FOV for despawning and when exiting play mode
-Added inspector errors for setting Spawn Radius smaller then Minimum Spawn Range
-Added Seasons and Season filters for spawning
-Added precheck on the spawner setup and issue warning if nothing will spawn.
-Added fix to apply hidden flags to biome components attached to SoulLinkSpawner, which can become visible after upgrade from 1.01.
-Added Manual trigger flag for Spawn Area so spawning can be triggered through code.
-Added new spawning methods for SpawnArea. You can now use Spawn Radius, Spawn Bounds, or Spawn Points.
-Fixed issue with GaiaGameTime where setup scene would add object even when Gaia Lighting was disabled
-Added error log if 'Build Nav Mesh After Spawning' is set to true on SpawnArea, but 'Generate Nav Mesh' is off
-Added basic A* integration to insure best spawn point is used when spawn position is deemed invalid
-Added 'Auto Add Spawnable Interface' option to SoulLinkSpawner
-Added more interface methods to ISpawnable
-Added SpawnableSettingsObject scriptable object and allow it to be used as a default for auto added Spawnable components
-Added Help Box for information regarding Population Cap to make it more clear.
-Added Import and Export buttons for Spawnable so they can be saved to SpawnableSettingsObject scriptable object.
-Added Elimination reset type to SpawnArea
-Fixed issue where user's custom OnDeath handler for Spawnable_EmeraldAI would prevent internal OnDeath handler from being assigned.
-Fixed issue where OnDeath would not be called if Despawn Delay was set to 0
-Added additional debug logging to show why an AI is despawning.
-No longer showing texture filters option if there are no texture layers in the scene
-Added TimedDespawn script

(1.0.1)
-Fixed issue where setup script fails to copy gizmo folder
-Added optional Demos packages for SRP pipelines (URP and HDRP)
-Moved SoulLinkGameTime prefab and scripts into Demos

(1.0.0)
First release
