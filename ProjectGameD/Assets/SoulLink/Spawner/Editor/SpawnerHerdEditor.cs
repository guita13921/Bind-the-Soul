using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SpawnerHerd), true)]
    public class SpawnerHerdEditor : Editor
    {
        private SpawnerHerdEditorHelper _herd = null;
        private Texture2D _herdLogo;
        private void OnEnable()
        {
            if (_herd == null)
            {
                _herd = new SpawnerHerdEditorHelper();
            } // if

            _herd.AssignTarget((SpawnerHerd)target);

            _herdLogo = (Texture2D)Resources.Load("Herd-Logo", typeof(Texture2D));
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorGUILayout.BeginHorizontal();
            EditorUtils.DrawSoulLinkLogo();
            GUILayout.Label(_herdLogo, GUILayout.Height(100));
            EditorGUILayout.EndHorizontal();

            _herd.DrawGeneralInfo();
            _herd.DrawSpawnInfoItemsGUI();

            EditorGUILayout.EndVertical();

            _herd.UpdateObject();
        } // OnInspectorGUI()
    } // class SpawnerHerdEditor
} // namespace Magique.SoulLink


