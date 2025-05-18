using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// (c)2020-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public static class EditorUtils
    {
        static public GUIStyle guiStyle_Regular_NoWrap;
        static public GUIStyle boxStyleLight;
        static public GUIStyle boxStyleLightGray;
        static public GUIStyle boxStyleDark;
        static public GUIStyle boxStyleDarkGray;
        static public GUIStyle bgndStyle;
        static public GUIStyle bgndStyleGray;
        static public GUIStyle bgndStyleGreenTrans;
        static public GUIStyle windowStyle;
        static public GUIStyle windowStyleGray;
        static public GUIStyle guiStyle_SmallBold;
        static public GUIStyle guiStyle_LargeBold;
        static public GUIStyle guiStyle_LargeBoldReg;
        static public GUIStyle guiStyle_MediumBold;
        static public GUIStyle guiStyle_Help;
        static public GUIStyle guiStyle_Error;
        static public GUIStyle guiStyle_RedText;
        static public GUIStyle guiStyle_GreenText;
        static public GUIStyle guiStyle_Yellow;
        static public GUIStyle guiStyle_Red;
        static public GUIStyle guiStyle_Green;
        static public GUIStyle guiStyle_Wrap;
        static public GUIStyle guiStyle_CenteredText;
        static public GUIStyle tabStyle;

        static public Color defaultColor;
        static public Color colorDarkGray = new Color(0.2f, 0.2f, 0.2f, 1f);
        static private bool initialized = false;
        static public Texture2D soullinkLogo = null;
        static public Texture2D spawnerText = null;
        static public Texture2D aiText = null;
        static private float _saveLableWidth;

        static public GUIStyle foldoutStyle;

        static public string SoulLinkGreenThemeSetting = "SoulLinkGreenTheme";
        static public string SoulLinkSpawnerVersionStr = "Version 1.3.8";
        static public string SoulLinkAIVersionStr = "Version 0.9.16a";

        static public int SMALL_BTN_SIZE = 25;
        static public int MEDIUM_BTN_SIZE = 75;

        public static void Init()
        {
            if (!initialized)
            {
                soullinkLogo = (Texture2D)Resources.Load("SoulLink-Logo", typeof(Texture2D));
                aiText = (Texture2D)Resources.Load("SoulLink-Logo-TextOnly", typeof(Texture2D));
                spawnerText = (Texture2D)Resources.Load("SoulLink-Spawner-TextOnly", typeof(Texture2D));

                defaultColor = GUI.color;

                windowStyle = new GUIStyle();
                windowStyle.normal.background = (Texture2D)Resources.Load("SoulLinkWindow", typeof(Texture2D));
                windowStyle.margin = new RectOffset(4, 4, 4, 4);

                windowStyleGray = new GUIStyle();
                windowStyleGray.normal.background = (Texture2D)Resources.Load("SoulLinkWindow-Gray", typeof(Texture2D));
                windowStyleGray.margin = new RectOffset(4, 4, 4, 4);

                boxStyleLight = new GUIStyle();
                boxStyleLight.normal.background = (Texture2D)Resources.Load("LightBgnd-Green", typeof(Texture2D));
                boxStyleLight.margin = new RectOffset(8, 8, 8, 8);

                boxStyleLightGray = new GUIStyle();
                boxStyleLightGray.normal.background = (Texture2D)Resources.Load("LightBgnd-Gray", typeof(Texture2D));
                boxStyleLightGray.margin = new RectOffset(8, 8, 8, 8);

                boxStyleDark = new GUIStyle();
                boxStyleDark.normal.background = (Texture2D)Resources.Load("DarkBgnd-DkGreen", typeof(Texture2D));
                boxStyleDark.margin = new RectOffset(8, 8, 8, 8);

                boxStyleDarkGray = new GUIStyle();
                boxStyleDarkGray.normal.background = (Texture2D)Resources.Load("DarkBgnd-Gray", typeof(Texture2D));
                boxStyleDarkGray.margin = new RectOffset(8, 8, 8, 8);

                bgndStyle = new GUIStyle();
                bgndStyle.normal.background = (Texture2D)Resources.Load("DarkBgnd-Green", typeof(Texture2D));
                bgndStyle.margin = new RectOffset(0, 0, 0, 0);

                bgndStyleGray = new GUIStyle();
                bgndStyleGray.normal.background = (Texture2D)Resources.Load("DarkBgnd-Gray", typeof(Texture2D));
                bgndStyleGray.margin = new RectOffset(0, 0, 0, 0);

                guiStyle_Regular_NoWrap = new GUIStyle();
                guiStyle_Regular_NoWrap.fontSize = 12;
                guiStyle_Regular_NoWrap.fontStyle = FontStyle.Normal;
                guiStyle_Regular_NoWrap.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_Regular_NoWrap.alignment = TextAnchor.MiddleLeft;

                guiStyle_LargeBold = new GUIStyle();
                guiStyle_LargeBold.fontSize = 16;
                guiStyle_LargeBold.fontStyle = FontStyle.Bold;
                guiStyle_LargeBold.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_LargeBold.wordWrap = true;

                guiStyle_MediumBold = new GUIStyle();
                guiStyle_MediumBold.fontSize = 14;
                guiStyle_MediumBold.fontStyle = FontStyle.Bold;
                guiStyle_MediumBold.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_MediumBold.wordWrap = true;

                guiStyle_SmallBold = new GUIStyle();
                guiStyle_SmallBold.fontSize = 12;
                guiStyle_SmallBold.fontStyle = FontStyle.Bold;
                guiStyle_SmallBold.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_SmallBold.wordWrap = true;

                guiStyle_Error = new GUIStyle();
                guiStyle_Error.fontSize = 13;
                guiStyle_Error.fontStyle = FontStyle.Italic | FontStyle.Bold;
                guiStyle_Error.normal.textColor = Color.red;
                guiStyle_Error.wordWrap = true;

                guiStyle_Help = new GUIStyle();
                guiStyle_Help.fontSize = 13;
                guiStyle_Help.fontStyle = FontStyle.Italic;
                guiStyle_Help.normal.textColor = Color.yellow;
                guiStyle_Help.wordWrap = true;

                guiStyle_RedText = new GUIStyle();
                guiStyle_RedText.fontSize = 13;
                guiStyle_RedText.fontStyle = FontStyle.Bold;
                guiStyle_RedText.normal.textColor = new Color(0.5f, 0.25f, 0.25f);
                guiStyle_RedText.wordWrap = true;

                guiStyle_GreenText = new GUIStyle();
                guiStyle_GreenText.fontSize = 13;
                guiStyle_GreenText.normal.textColor = Color.green;
                guiStyle_GreenText.wordWrap = true;

                guiStyle_Wrap = new GUIStyle();
                guiStyle_Wrap.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_Wrap.fontSize = 13;
                guiStyle_Wrap.wordWrap = true;

                guiStyle_CenteredText = new GUIStyle();
                guiStyle_CenteredText.alignment = TextAnchor.MiddleCenter;
                guiStyle_CenteredText.fontSize = 14;
                guiStyle_CenteredText.fontStyle = FontStyle.Bold;
                guiStyle_CenteredText.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
                guiStyle_CenteredText.wordWrap = true;

                guiStyle_Yellow = new GUIStyle();
                guiStyle_Yellow.fontSize = 12;
                guiStyle_Yellow.fontStyle = FontStyle.Normal;
                guiStyle_Yellow.normal.textColor = Color.yellow;
                guiStyle_Yellow.alignment = TextAnchor.MiddleLeft;

                guiStyle_Red = new GUIStyle();
                guiStyle_Red.fontSize = 12;
                guiStyle_Red.fontStyle = FontStyle.Normal;
                guiStyle_Red.normal.textColor = Color.red;
                guiStyle_Red.alignment = TextAnchor.MiddleLeft;

                guiStyle_Green = new GUIStyle();
                guiStyle_Green.fontSize = 12;
                guiStyle_Green.fontStyle = FontStyle.Normal;
                guiStyle_Green.normal.textColor = Color.green;
                guiStyle_Green.alignment = TextAnchor.MiddleLeft;

                tabStyle = new GUIStyle();
                tabStyle.alignment = TextAnchor.MiddleCenter;
                tabStyle.fontSize = 14;
                tabStyle.border = new RectOffset(18, 18, 20, 4);

                Texture2D tabNormal = (Texture2D)Resources.Load("Tab_Normal");
                Texture2D tabSelected = (Texture2D)Resources.Load("Tab_Selected");

                tabStyle.fixedHeight = 40f;
                tabStyle.normal.background = tabNormal;
                tabStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

                tabStyle.onNormal.background = tabSelected;
                tabStyle.onNormal.textColor = Color.black;

                tabStyle.onFocused.background = tabSelected;
                tabStyle.onFocused.textColor = Color.black;

                foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.fontStyle = FontStyle.Bold;

                initialized = true;
            } // if
        } // Init()

        public static bool IsUsingGreenTheme()
        {
            return EditorPrefs.GetBool(SoulLinkGreenThemeSetting, true);
        } // IsUsingGreenTheme()

        public static void DrawHeader(string headerText)
        {
            BeginBoxStyleDark();
            BeginBoxStyleLight();
            EditorGUILayout.LabelField(string.Format("- - - - - {0} - - - - -", headerText), guiStyle_CenteredText, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        } // DrawHeader()

        public static void BeginWindowStyle()
        {
            if (IsUsingGreenTheme())
                EditorGUILayout.BeginVertical(windowStyle);
            else
                EditorGUILayout.BeginVertical(windowStyleGray);
        } // BeginWindowStyleGray()

        public static Rect BeginBoxStyleDark()
        {
            if (IsUsingGreenTheme())
                return EditorGUILayout.BeginVertical(boxStyleDark);
            else
                return EditorGUILayout.BeginVertical(boxStyleDarkGray);
        } // BeginBoxStyleDark()

        public static void BeginBgndStyleDark()
        {
            if (IsUsingGreenTheme())
                EditorGUILayout.BeginVertical(bgndStyle);
            else
                EditorGUILayout.BeginVertical(bgndStyleGray);
        } // BeginBgndStyleDark()

        public static void BeginBoxStyleLight()
        {
            if (IsUsingGreenTheme())
                EditorGUILayout.BeginVertical(boxStyleLight);
            else
                EditorGUILayout.BeginVertical(boxStyleLightGray);
        } // BeginBoxStyleLight()

        public static void BeginBgndStyle()
        {
            if (IsUsingGreenTheme())
                EditorGUILayout.BeginVertical(bgndStyle);
            else
                EditorGUILayout.BeginVertical(bgndStyleGray);
        } // BeginBgndStyle()

        public static void SaveLabelWidth()
        {
            _saveLableWidth = EditorGUIUtility.labelWidth;
        } // SaveLabelWidth()

        public static void SetLabelWidth(string labelString)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(labelString));
            EditorGUIUtility.labelWidth = textDimensions.x;
        } // SetLabelWidth()

        public static void SetLabelWidth(float width)
        {
            EditorGUIUtility.labelWidth = width;
        } // SetLabelWidth()

        public static void RestoreLabelWidth()
        {
            EditorGUIUtility.labelWidth = _saveLableWidth;
        } // RestoreLabelWidth()

        public static float GetLabelWidth(string labelString)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(labelString));
            return textDimensions.x;
        } // GetLabelWidth()

        public static void LabelField(string labelString, GUIStyle guiStyle = null)
        {
            if (string.IsNullOrEmpty(labelString)) return;
            EditorGUILayout.LabelField(labelString, guiStyle == null ? guiStyle_Regular_NoWrap : guiStyle, GUILayout.Width(GetLabelWidth(labelString)));
        } // LabelField()

        public static int IntField(string labelString, int intValue, int width = 50, GUIStyle guiStyle = null)
        {
            LabelField(labelString, guiStyle);
            return EditorGUILayout.IntField(intValue, GUILayout.Width(width));
        } // IntField()

        public static bool Toggle(string labelString, bool boolValue, int width = 50, GUIStyle guiStyle = null)
        {
            LabelField(labelString, guiStyle);
            return EditorGUILayout.Toggle(boolValue, GUILayout.Width(width));
        } // Toggle()

        public static float FloatField(string labelString, float floatValue, int width = 50, GUIStyle guiStyle = null)
        {
            LabelField(labelString, guiStyle);
            return EditorGUILayout.FloatField(floatValue, GUILayout.Width(width));
        } // FloatField()

        public static void HorizontalLine(Color color, float height, Vector2 margin)
        {
            GUILayout.Space(margin.x);

            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);

            GUILayout.Space(margin.y);
        } // HorizontalLine()

        static public void DrawSoulLinkLogo()
        {
            var preferredSize = 550;
            var scaleFactor = EditorGUIUtility.currentViewWidth / preferredSize;
            GUILayout.Label(soullinkLogo, GUILayout.Width(150 * scaleFactor), GUILayout.Height(75 * scaleFactor));
        } // DrawSoulLinkLogo()

        static public void DrawSpawnerText(bool compact = false)
        {
            if (!compact)
            {
                EditorGUILayout.BeginVertical();
                var preferredSize = 550;
                var scaleFactor = EditorGUIUtility.currentViewWidth / preferredSize;
                GUILayout.Label(spawnerText, GUILayout.Width(250 * scaleFactor), GUILayout.Height(82.5f * scaleFactor));
                EditorGUILayout.LabelField(SoulLinkSpawnerVersionStr);
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                var preferredSize = 900;
                var scaleFactor = EditorGUIUtility.currentViewWidth / preferredSize;
                GUILayout.Label(spawnerText, GUILayout.Width(250 * scaleFactor), GUILayout.Height(32f * scaleFactor));
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(SoulLinkSpawnerVersionStr);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        } // DrawSpawnerText()

        static public void DrawAIText(bool compact = false)
        {
            if (!compact)
            {
                EditorGUILayout.BeginVertical();
                var preferredSize = 550;
                var scaleFactor = EditorGUIUtility.currentViewWidth / preferredSize;
                GUILayout.Label(aiText, GUILayout.Width(250 * scaleFactor), GUILayout.Height(82.5f * scaleFactor));
                EditorGUILayout.LabelField(SoulLinkAIVersionStr);
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                var preferredSize = 750;
                var scaleFactor = EditorGUIUtility.currentViewWidth / preferredSize;
                GUILayout.Label(aiText, GUILayout.Width(250 * scaleFactor), GUILayout.Height(32f * scaleFactor));
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(SoulLinkAIVersionStr);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        } // DrawAIText()

        /// <summary>
        /// Editor helper function to edit an AudioClipInfo object and return true if the currently edited item should be removed
        /// </summary>
        /// <param name="audioClipInfo"></param>
        /// <returns></returns>
        static public bool EditAudioClipInfo(AudioClipInfo audioClipInfo, bool allowRemoval = true)
        {
            if (audioClipInfo == null) return false;
            EditorGUILayout.BeginHorizontal();
#if SOULLINK_USE_MASTERAUDIO
                audioClipInfo.clip = EditorGUILayout.TextField("Clip", audioClipInfo.clip);
#else
            audioClipInfo.clip = (AudioClip)EditorGUILayout.ObjectField("Clip", audioClipInfo.clip, typeof(AudioClip), false);
#endif
            if (allowRemoval)
            {
                bool buttonRemove = GUILayout.Button("-", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorUtils.SMALL_BTN_SIZE));
                EditorGUILayout.EndHorizontal();

                if (buttonRemove)
                {
                    return true;
                } // if
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }

            audioClipInfo.volume = EditorGUILayout.Slider("Volume", audioClipInfo.volume, 0f, 1f);
            audioClipInfo.is3D = EditorGUILayout.Toggle("Is 3D", audioClipInfo.is3D);
            audioClipInfo.minRange = EditorGUILayout.FloatField("Min Range", audioClipInfo.minRange);
            audioClipInfo.maxRange = EditorGUILayout.FloatField("Max Range", audioClipInfo.maxRange);

            return false;
        } // EditAudioClipInfo()

        static public void SetColor(Color color)
        {
            GUI.color = color;
        } // SetColor()

        static public void DefaultColor()
        {
            GUI.color = defaultColor;
        } // DefaultColor()

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        } // MakeTex()

        public static List<T> GetAssetsWithScript<T> (string path) where T : MonoBehaviour
        {
            T tmp;
            string assetPath;
            GameObject asset;
            List<T> assetList = new List<T>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });

            for (int i = 0; i < guids.Length; ++i)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                tmp = asset.GetComponent<T>();
                if (tmp != null)
                {
                    assetList.Add(tmp);
                }
            } // for i

            return assetList;
        } // GetAssetsWithScript()

        public static List<T> GetListFromEnum<T>()
        {
            List<T> enumList = new List<T>();
            System.Array enums = System.Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                enumList.Add(e);
            } // foreach

            return enumList;
        } // GetListFromEnum()

        public static void RemoveScriptingDefine(string value)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            defines = defines.Replace(value, "");
            defines = defines.Replace(";;", ";");

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        } // RemoveScriptingDefine()

        public static void AddScriptingDefine(string value)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (string.IsNullOrEmpty(defines))
            {
                defines = value;
            }
            else if (!defines.EndsWith(";"))
            {
                defines += ";" + value;
            }
            else
            {
                defines += value;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        } // AddScriptingDefine()

    } // class EditorUtils
} // namespace Magique.SoulLink