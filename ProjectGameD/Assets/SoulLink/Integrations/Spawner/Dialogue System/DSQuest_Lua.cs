#if SOULLINK_USE_DS
using UnityEngine;
using PixelCrushers.DialogueSystem;

// A set of SoulLinkQuestManager functions that can be registered with Dialogue System and called in Lua 

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class DSQuest_Lua : MonoBehaviour
    {
        protected static bool areFunctionsRegistered = false;
        private bool didIRegisterFunctions = false;

        private DSQuestManager _questManager;

        void OnEnable()
        {
            if (areFunctionsRegistered)
            {
                didIRegisterFunctions = false;
            }
            else
            {
                Lua.RegisterFunction("DSQuest_ActivateQuest", this, SymbolExtensions.GetMethodInfo(() => ActivateQuest(string.Empty)));
                Lua.RegisterFunction("DSQuest_DeactivateQuest", this, SymbolExtensions.GetMethodInfo(() => DeactivateQuest(string.Empty)));
            }

            if (_questManager == null)
            {
                _questManager = FindObjectOfType<DSQuestManager>();
            } // if
        } // OnEnable()

        void OnDisable()
        {
            // Note: If this script is on your Dialogue Manager & the Dialogue Manager is configured
            // as Don't Destroy On Load (on by default), don't unregister Lua functions.
            if (didIRegisterFunctions)
            {
                // Remove the functions from Lua:
                didIRegisterFunctions = false;
                areFunctionsRegistered = false;

                Lua.UnregisterFunction("DSQuest_ActivateQuest");
                Lua.UnregisterFunction("DSQuest_DeactivateQuest");
            }
        } // OnDisable()

        private void ActivateQuest(string questName)
        {
            if (_questManager == null) return;

            _questManager.ActivateQuest(questName);
        } // ActivateQuest()

        private void DeactivateQuest(string questName)
        {
            if (_questManager == null) return;

            _questManager.DeactivateQuest(questName);
        } // DeactivateQuest()
    } // DSQuest_Lua
} // namespace Magique.SoulLink
#endif