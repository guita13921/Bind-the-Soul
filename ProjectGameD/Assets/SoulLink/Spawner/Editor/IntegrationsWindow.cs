using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class IntegrationsWindow : EditorWindow
    {
        public static IntegrationsWindow instance;

        private string[] _integrations = { "Azure Sky", "Time of Day", "Core GameKit", "Dialogue System", "Game Kit Controller", "Realistic FPS Prefab", "Final IK", "Tenkoku",
                                            "Expanse", "Master Audio", "Quest Machine", "Malbers Animal Controller", "Sky Master Ultimate", "Unity AI Navigation", "Global Snow",
                                            "Opsive Version 2", "3D Game Kit"};
        private string[] _readOnlyIntegrations = { "MapMagic 2", "Enviro LW", "Enviro HD", "Weather Maker", "UniStorm", "Landscape Builder", "Jupiter", 
                                                   "Emerald AI", "Playmaker", "Opsive Third Person Controller",  "Opsive UCC Shooter", "Opsive UCC Melee",
                                                    "Gaia Pro", "Survival Engine", "Invector FSM AI", "Polaris", "Survival Template Pro", "Dreamteck Splines",
                                                    "Enviro 3", "Cozy Sylized Weather 2" };
        private string[] _defines = { "SOULLINK_USE_AZURE", "SOULLINK_USE_TOD", "SOULLINK_USE_POOLBOSS", "SOULLINK_USE_DS", "SOULLINK_USE_GKC", "SOULLINK_USE_RFPS", "SOULLINK_USE_FIK", 
                                      "SOULLINK_USE_TENKOKU", "SOULLINK_USE_EXPANSE", "SOULLINK_USE_MASTERAUDIO", "SOULLINK_USE_QM", "SOULLINK_USE_MAC", "SOULLINK_USE_SKYMASTER",
                                      "SOULLINK_USE_AINAV", "SOULLINK_USE_GLOBALSNOW", "OPSIVE_V2", "SOULLINK_USE_3DGAMEKIT" };
        private string[] _readOnlyDefines = { "MAPMAGIC2", "ENVIRO_LW", "ENVIRO_HD", "WEATHER_MAKER_PRESENT", "UNISTORM_PRESENT", 
                                              "LANDSCAPE_BUILDER", "JUPITER", "EMERALD_AI_PRESENT", "PLAYMAKER", "THIRD_PERSON_CONTROLLER", 
                                              "ULTIMATE_CHARACTER_CONTROLLER_SHOOTER", "ULTIMATE_CHARACTER_CONTROLLER_MELEE", "GAIA_PRO_PRESENT", "SURVIVAL_ENGINE",
                                              "INVECTOR_AI_TEMPLATE", "GRIFFIN_2021", "SURVIVAL_TEMPLATE_PRO", "DREAMTECK_SPLINES", "ENVIRO_3", "COZY_WEATHER" };
        private Dictionary<string, string> _integrationDefines = new Dictionary<string, string>();
        private Dictionary<string, string> _readOnlyIntegrationDefines = new Dictionary<string, string>();
        private Dictionary<string, bool> _integrationStatus = new Dictionary<string, bool>();
        private Dictionary<string, bool> _readOnlyIntegrationStatus = new Dictionary<string, bool>();

        public static void ShowEditor()
        {
            instance = (IntegrationsWindow)GetWindow(typeof(IntegrationsWindow));
            instance.titleContent = new GUIContent("SoulLink Integrations");

            // calculate a proper size for the window based on how many entries we have
            var winHeight = ((instance._integrations.Length + instance._readOnlyIntegrations.Length) * 22f) + 32f; // 32f extra is for Apply button
            instance.position = new Rect(300, 150, 350f, winHeight);
        } // ShowEditor()

        private void OnEnable()
        {
            InitIntegrations();
            GetIntegrationStatuses();
        } // OnEnable()

        private void OnGUI()
        {
            EditorUtils.Init();

            EditorGUILayout.BeginVertical(EditorUtils.boxStyleLight);
            EditorUtils.DrawSoulLinkLogo();

            // Show each integration status with toggle
            DrawIntegrationStatus();

            bool buttonApply= GUILayout.Button("Apply", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(128));
            if (buttonApply)
            {
                UpdateIntegrationStatuses();
            } // if

            EditorGUILayout.EndVertical();
        } // OnGUI()

        void InitIntegrations()
        {
            // Changeable
            if (_integrationDefines.Count == 0)
            {
                for (int i = 0; i < _integrations.Length; ++i)
                {
                    _integrationDefines[_integrations[i]] = _defines[i];
                } // for i
            } // if

            // Read Only
            if (_readOnlyIntegrationDefines.Count == 0)
            {
                for (int i = 0; i < _readOnlyIntegrations.Length; ++i)
                {
                    _readOnlyIntegrationDefines[_readOnlyIntegrations[i]] = _readOnlyDefines[i];
                } // for i
            } // if
        } // InitIntegrations()

        string _currentDefines = "";

        void GetIntegrationStatuses()
        {
            _currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Check current status of the integration and set status to true or false
            for (int i = 0; i < _integrations.Length; ++i)
            {
                _integrationStatus[_integrations[i]] = GetIntegrationStatus(_integrationDefines[_integrations[i]]);
            } // for i

            // Check current status of the read only integration and set status to true or false
            for (int i = 0; i < _readOnlyIntegrations.Length; ++i)
            {
                _readOnlyIntegrationStatus[_readOnlyIntegrations[i]] = GetIntegrationStatus(_readOnlyIntegrationDefines[_readOnlyIntegrations[i]]);
            } // for i
        } // GetIntegrationStatuses()

        bool GetIntegrationStatus(string define)
        {
            return _currentDefines.Contains(define);
        } // GetIntegrationStatus()

        void DrawIntegrationStatus()
        {
            Dictionary<string, bool> newIntegrationStatus = new Dictionary<string, bool>();

            foreach (var integrationStatus in _integrationStatus)
            {
                newIntegrationStatus[integrationStatus.Key] = EditorGUILayout.Toggle(integrationStatus.Key, integrationStatus.Value);
            } // foreach

            foreach (var integrationStatus in _readOnlyIntegrationStatus)
            {
                if (integrationStatus.Value)
                {
                    EditorGUILayout.LabelField("  " + integrationStatus.Key + " is present", EditorUtils.guiStyle_GreenText);
                }
                else
                {
                    EditorGUILayout.LabelField("  " + integrationStatus.Key + " is not available", EditorUtils.guiStyle_RedText);
                }
            } // foreach

            // Update any changed items
            foreach (var integrationStatus in newIntegrationStatus)
            {
                _integrationStatus[integrationStatus.Key] = integrationStatus.Value;
            } // foreach
        } // DrawIntegrationStatus()

        void UpdateIntegrationStatuses()
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Remove all SoulLink define symbols unless they are read only
            for (int i = 0; i < _defines.Length; ++i)
            {
                defines = defines.Replace(_defines[i], "");
            } // for i
            defines = defines.Replace(";;", ";");

            // Add in concatenated list of integration defines
            string newDefines = "";
            foreach (var integrationStatus in _integrationStatus)
            {
                for (int i = 0; i < _integrations.Length; ++i)
                {
                    if (_integrations[i] == integrationStatus.Key)
                    {
                        if (integrationStatus.Value)
                        {
                            newDefines += _integrationDefines[integrationStatus.Key] + ";";
                        } // if
                    } // if
                } // for i
            } // foreach

            if (!string.IsNullOrEmpty(newDefines))
            {
                if (string.IsNullOrEmpty(defines))
                {
                    defines = newDefines;
                }
                else if (!defines.EndsWith(";"))
                {
                    defines += ";" + newDefines;
                }
                else
                {
                    defines += newDefines;
                }
            } // if

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);

            instance.Close();
        } // UpdateIntegrationStatuses()
    } // class IntegrationsWindow
} // namespace Magique.SoulLink
