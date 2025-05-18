using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class CopySpawnWindow : EditorWindow
    {
        public static CopySpawnWindow instance;

        static private bool _copyPrefabs = false;
        static private bool _copyFilters = true;

        static private SpawnInfoData _copySource = null;
        static private SpawnInfoData _copyTarget = null;

        public static void ShowEditor(SpawnInfoData copySource, SpawnInfoData copyTarget)
        {
            _copySource = copySource;
            _copyTarget = copyTarget;

            instance = (CopySpawnWindow)GetWindow(typeof(CopySpawnWindow));
            instance.titleContent = new GUIContent("Copy Spawn");
            instance.maxSize = new Vector2(300f, 100f);
            instance.minSize = new Vector2(300f, 100f);
            instance.maximized = true;
            instance.ShowModal();
        } // ShowEditor()

        private void OnEnable()
        {
        } // OnEnable()

        private void OnGUI()
        {
            EditorUtils.Init();

            EditorGUILayout.BeginVertical(EditorUtils.boxStyleLight);

            string copyName = "";
            switch (_copySource.spawnType)
            {
                case SoulLinkSpawnerTypes.SpawnType.Prefab:
                    copyName = _copySource.prefab != null ? _copySource.prefab.name : "";
                    copyName += " (Prefab)";
                    break;
                case SoulLinkSpawnerTypes.SpawnType.Herd:
                    copyName = _copySource.herd != null ? _copySource.herd.name : "";
                    copyName += " (Herd)";
                    break;
                case SoulLinkSpawnerTypes.SpawnType.Variants:
                    copyName = _copySource.variantsName;
                    copyName += " (Variants)";
                    break;
            } // switch

            EditorGUILayout.LabelField("Copy from " + copyName, EditorUtils.guiStyle_MediumBold);

            // Show copy options
            _copyPrefabs = EditorGUILayout.Toggle("Copy Prefab References", _copyPrefabs);
            _copyFilters = EditorGUILayout.Toggle("Copy Filters", _copyFilters);

            EditorGUILayout.BeginHorizontal();
            bool buttonOK = GUILayout.Button("OK", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            bool buttonCancel = GUILayout.Button("CANCEL", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            EditorGUILayout.EndHorizontal();

            if (buttonOK)
            {
                CopySpawnData();
            }
            else if (buttonCancel)
            {
                instance.Close();
            }

            EditorGUILayout.EndVertical();
        } // OnGUI()

        void CopySpawnData()
        {
            _copyTarget.CopyFrom(_copySource, _copyPrefabs, _copyFilters);
            instance.Close();
        } // CopySpawnData()
    } // class CopySpawnWindow
} // namespace Magique.SoulLink
