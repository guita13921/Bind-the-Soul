using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class DialogueData
    {
        public GameObject sprite;
        public TMPro.TextMeshProUGUI dialogueTextObject;
        public string dialogueText;
        public AudioClip dialogueAudioClip;
        public float delayNext = 1f;
    } // class DialogueData
} // namespace Magique.SoulLink
