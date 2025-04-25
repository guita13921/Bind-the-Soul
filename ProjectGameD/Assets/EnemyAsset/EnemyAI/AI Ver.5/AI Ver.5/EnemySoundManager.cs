using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemySoundManager : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] deathSounds;
        [SerializeField] private AudioClip[] attackSounds;
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip[] shieldHitSounds;
        [SerializeField] private AudioClip[] shieldBreakSounds;
        [SerializeField] private AudioClip[] PariedSounds;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayDeathSound()
        {
            PlayRandomSound(deathSounds);
        }

        public void PlayAttackSound()
        {
            PlayRandomSound(attackSounds);
        }

        public void PlayHitSound()
        {
            PlayRandomSound(hitSounds);
        }

        public void PlayShieldHitSounds()
        {
            PlayRandomSound(shieldHitSounds);
        }

        public void PlayShielBreakSounds()
        {
            PlayRandomSound(shieldBreakSounds);
        }

        public void PlayPariedSounds()
        {
            PlayRandomSound(PariedSounds);
        }

        private void PlayRandomSound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0)
            {
                int index = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[index]);
            }
        }
    }


}