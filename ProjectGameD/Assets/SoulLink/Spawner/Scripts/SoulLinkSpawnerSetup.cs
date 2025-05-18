#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [InitializeOnLoad]
    public class SoulLinkSpawnerSetup : MonoBehaviour
    {
        public const string NAVMESHBUILDER_TAG = "NavMeshBuilder";
        public const string TERRAIN_LAYER = "Terrain";
        public const string SOULLINK_SPAWNER_DEFINE = "SOULLINK_SPAWNER";
        public const string NAVMESHAREA_WATER = "Water";
        public const string NAVMESHAREA_CITY = "City";

        static SoulLinkSpawnerSetup()
        {
            // Create a necessary tag for dynamic navmesh generation feature
            CreateTag(NAVMESHBUILDER_TAG);

            // Create necessary NavMesh areas for demo scenes
            CreateNavMeshArea(NAVMESHAREA_WATER);
            CreateNavMeshArea(NAVMESHAREA_CITY);

            // Add define for SOULLINK_SPAWNER so scripts can determine if it is installed
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!defines.Contains(SOULLINK_SPAWNER_DEFINE))
            {
                if (string.IsNullOrEmpty(defines))
                {
                    defines = SOULLINK_SPAWNER_DEFINE;
                }
                else if (!defines.EndsWith(";"))
                {
                    defines += ";" + SOULLINK_SPAWNER_DEFINE;
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            } // if
        } // SoulLinkSpawnerSetup()

        /// <summary>
        /// Create new tag
        /// </summary>
        /// <param name="tag"></param>
        public static void CreateTag(string tag)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            if (asset != null)
            { 
                var so = new SerializedObject(asset);
                var tags = so.FindProperty("tags");
                var numTags = tags.arraySize;

                // do not create duplicates
                for (int i = 0; i < numTags; i++)
                {
                    var existingTag = tags.GetArrayElementAtIndex(i);
                    if (existingTag.stringValue == tag) return;
                } // for i

                tags.InsertArrayElementAtIndex(numTags);
                tags.GetArrayElementAtIndex(numTags).stringValue = tag;
                so.ApplyModifiedProperties();
                so.Update();
            } // if
        } // CreateTag()

        /// <summary>
        /// Create a new nav mesh area
        /// </summary>
        /// <param name="area"></param>
        public static void CreateNavMeshArea(string area)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/NavMeshAreas.asset");
            if (asset != null)
            {
                var so = new SerializedObject(asset);
                var areas = so.FindProperty("areas");
                var numAreas = areas.arraySize;
                int newAreaIndex = -1;

                // do not create duplicates
                for (int i = 0; i < numAreas; i++)
                {
                    var existingArea = areas.GetArrayElementAtIndex(i);
                    var areaName = existingArea.FindPropertyRelative("name");
                    if (areaName.stringValue == area) return;
                    if (string.IsNullOrEmpty(areaName.stringValue) && newAreaIndex == -1)
                    {
                        newAreaIndex = i;
                    } // if
                } // for i

                if (newAreaIndex == -1) return;

                areas.GetArrayElementAtIndex(newAreaIndex).FindPropertyRelative("name").stringValue = area;
                areas.GetArrayElementAtIndex(newAreaIndex).FindPropertyRelative("cost").floatValue = 1f;
                so.ApplyModifiedProperties();
                so.Update();
            } // if
        } // CreateNavMeshArea()
    } // class SoulLinkSpawnerSetup()
} // namespace Magique.SoulLink
#endif
