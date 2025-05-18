using UnityEngine;
using UnityEngine.Audio;

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [System.Serializable]
    public class SoundData
    {
        public AudioMixerGroup outputAudioMixerGroup = null;
        public bool soundIs3D = true;
        public float soundVolume = 1f;
        public float soundMinRange = 1f;
        public float soundMaxRange = 30f;

        [Tooltip("Use this field if you want to play through Unity audio system.")]
        public AudioClip audioClip;

        [Tooltip("Use this field if you want to play through Master Audio system.")]
        public string audioClipName;
    } // class SoundData
} // namespace Magique.SoulLink
