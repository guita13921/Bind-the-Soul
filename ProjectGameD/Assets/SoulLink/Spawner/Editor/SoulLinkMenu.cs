using UnityEditor;
using UnityEngine;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SoulLinkMenu : MonoBehaviour
    {
        private const string MenuName = "Tools/SoulLink/Green Editor Theme";

        [MenuItem("Tools/SoulLink/Integrations")]
        private static void SetupIntegrations()
        {
            IntegrationsWindow.ShowEditor();
        } // SetupIntegrations()

        public static bool IsEnabled
        {
            get { return EditorPrefs.GetBool(EditorUtils.SoulLinkGreenThemeSetting, true); }
            set { EditorPrefs.SetBool(EditorUtils.SoulLinkGreenThemeSetting, value); }
        } // IsEnabled()

        [MenuItem(MenuName)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        } // ToggleAction()

        [MenuItem(MenuName, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MenuName, IsEnabled);
            return true;
        } // ToggleActionValidate()
    } // class SoulLinkMenu
} // namespace Magique.SoulLink

