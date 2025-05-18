using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [CustomEditor(typeof(SoulLinkGameTime), true)]
    public class SoulLinkGameTimeEditor : Editor
    {
        private void OnEnable()
        {
        } // OnEnable()

        public override void OnInspectorGUI()
        {
            EditorUtils.Init();

            EditorUtils.BeginBgndStyle();

            EditorUtils.DrawSoulLinkLogo();

            DrawDefaultInspector();

            EditorGUILayout.EndVertical();
        } // OnInspectorGUI()
    } // SoulLinkGameTimeEditor
} // namespace Magique.SoulLink
