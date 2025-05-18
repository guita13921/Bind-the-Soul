#if SOULLINK_USE_POOLBOSS
using UnityEditor;
using UnityEngine;

// (c)2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(PoolManager_PoolBoss), true)]
    public class PoolManager_PoolBossEditor : Editor
    {
        private PoolManager_PoolBoss _myTarget;

        private void OnEnable()
        {
            _myTarget = (PoolManager_PoolBoss)target;

            // Clear all database Spawner_ items since they should not exist at edit time anymore
            SpawnRegion[] regions = Resources.FindObjectsOfTypeAll<SpawnRegion>() as SpawnRegion[];
            foreach (var region in regions)
            {
                _myTarget.ClearPoolItems(region.Database);
            } // foreach

            var spawner = FindObjectOfType<SoulLinkSpawner>();
            if (spawner != null)
            {
                _myTarget.ClearPoolItems(spawner.Database);
            } // if
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying && SoulLinkSpawner.Instance != null && (SoulLinkSpawner.Instance.Initializing || SoulLinkSpawner.Quitting)) return;

            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()
    } // PoolManager_PoolBossEditor
} // namespace Magique.SoulLink
#endif
