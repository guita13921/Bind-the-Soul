using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioClip[] attackSwordSounds;
    [SerializeField] private AudioClip[] attackHammerSounds;
    [SerializeField] private AudioClip[] attackDaggerSounds;
    [SerializeField] private AudioClip[] DamageSounds;
    [SerializeField] private AudioClip[] shieldHitSounds;

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

    public void PlayAttackSWordSound()
    {
        PlayRandomSound(attackSwordSounds);
    }
    public void PlayAttackHammerSound()
    {
        PlayRandomSound(attackHammerSounds);
    }
    public void PlayAttackDaggerSound()
    {
        PlayRandomSound(attackDaggerSounds);
    }

    public void PlayDamageSound()
    {
        PlayRandomSound(DamageSounds);
    }

    public void PlayShieldHitSounds()
    {
        PlayRandomSound(shieldHitSounds);
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

