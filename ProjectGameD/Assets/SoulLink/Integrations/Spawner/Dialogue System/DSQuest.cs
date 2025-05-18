#if SOULLINK_USE_DS

using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Dialogue System Quest")]
    public class DSQuest : BaseQuest
    {
        [QuestPopup]
        [SerializeField]
        protected string _questName;

        /// <summary>
        /// Return the quest name
        /// </summary>
        /// <returns></returns>
        override public string GetQuestName()
        {
            return _questName;
        } // GetQuestName()

        /// <summary>
        /// The quest is considered active if the isActive toggle is checked and the quest is active in the DS database
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            if (_questManager == null) return false;

            return _questManager.IsQuestActive(_questName);
        } // IsActive()
    } // class DSQuest
} // namespace Magique.SoulLink

#endif