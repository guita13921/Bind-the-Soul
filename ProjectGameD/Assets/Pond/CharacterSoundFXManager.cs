using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class CharacterSoundFXManager : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        AudioSource audioSource;
        EnemyWeaponSlotManager enemyWeaponSlotManager;
        //ATTACKING GRUNTS

        //TAKEING DAMAGE GRUNTS
        [Header("Taking Damage Sounds")]
        public AudioClip[] takingDamageSounds;
        private List<AudioClip> potentiaDamageSounds;
        private AudioClip LastDamageSoundPlayed;

        [Header("Shield Hit Sounds")]
        public AudioClip[] shieldHitSounds;
        private List<AudioClip> potentiaShieldHitSounds;
        private AudioClip LastShieldHitSoundPlayed;

        [Header("Shield Break Sounds")]
        public AudioClip[] shieldBreakSounds;
        private List<AudioClip> potentiaShieldBreakSounds;
        private AudioClip LastShieldBreakSoundPlayed;

        [Header("weapon Whooshes")]
        private List<AudioClip> potentiaWeaponWhooshes;
        private AudioClip LastWeaponWhooshesSoundPlayed;

        [Header("Dead Sounds")]
        public AudioClip deadSound;

        [Header("Dead Sounds")]
        public AudioClip pariedSounds;


        //FOOT STEP SOUND

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
        }

        public virtual void PlayRandomDamageSoundFX()
        {
            potentiaDamageSounds = new List<AudioClip>();

            foreach (var damageSound in takingDamageSounds)
            {
                if (damageSound != LastDamageSoundPlayed)
                {
                    potentiaDamageSounds.Add(damageSound);
                }
            }

            int randomValue = Random.Range(0, potentiaDamageSounds.Count);
            LastDamageSoundPlayed = takingDamageSounds[randomValue];
            audioSource.PlayOneShot(takingDamageSounds[randomValue], 0.4f);
        }

        public virtual void PlayRandomShielHitSoundFX()
        {
            potentiaShieldHitSounds = new List<AudioClip>();

            foreach (var shieldHit in shieldHitSounds)
            {
                if (shieldHit != LastShieldHitSoundPlayed)
                {
                    potentiaShieldHitSounds.Add(shieldHit);
                }
            }

            int randomValue = Random.Range(0, potentiaShieldHitSounds.Count);
            LastShieldHitSoundPlayed = shieldHitSounds[randomValue];
            audioSource.PlayOneShot(shieldHitSounds[randomValue], 0.4f);
        }

        public virtual void PlayRandomShielBreakSoundFX()
        {
            potentiaShieldBreakSounds = new List<AudioClip>();

            foreach (var shieldBreak in shieldBreakSounds)
            {
                if (shieldBreak != LastShieldBreakSoundPlayed)
                {
                    potentiaShieldBreakSounds.Add(shieldBreak);
                }
            }

            int randomValue = Random.Range(0, potentiaShieldBreakSounds.Count);
            LastShieldBreakSoundPlayed = shieldBreakSounds[randomValue];
            audioSource.PlayOneShot(shieldBreakSounds[randomValue], 0.4f);
        }

        public virtual void PlayRandomWeaponWhooshesSoundFX()
        {
            potentiaWeaponWhooshes = new List<AudioClip>();

            if (weaponSlotManager != null) //Player
            {
                foreach (var whooshSound in weaponSlotManager.attackingWeapon.weaponWhooshes)
                {
                    if (whooshSound != LastWeaponWhooshesSoundPlayed)
                    {
                        potentiaWeaponWhooshes.Add(whooshSound);
                    }

                    int randomValue = Random.Range(0, potentiaWeaponWhooshes.Count);
                    LastWeaponWhooshesSoundPlayed = weaponSlotManager.attackingWeapon.weaponWhooshes[randomValue];
                    audioSource.PlayOneShot(weaponSlotManager.attackingWeapon.weaponWhooshes[randomValue], 0.4f);
                }
            }
            else //Enemy
            {
                foreach (var whooshSound in enemyWeaponSlotManager.rightHandWeapon.weaponWhooshes)
                {
                    if (whooshSound != LastWeaponWhooshesSoundPlayed)
                    {
                        potentiaWeaponWhooshes.Add(whooshSound);
                    }

                    int randomValue = Random.Range(0, potentiaWeaponWhooshes.Count);
                    LastWeaponWhooshesSoundPlayed = enemyWeaponSlotManager.rightHandWeapon.weaponWhooshes[randomValue];
                    audioSource.PlayOneShot(enemyWeaponSlotManager.rightHandWeapon.weaponWhooshes[randomValue], 0.4f);
                }
            }
        }

        public virtual void PlayDeathSound()
        {
            audioSource.PlayOneShot(deadSound, 0.4f);
        }

        public virtual void PlayPariedSounds()
        {
            audioSource.PlayOneShot(pariedSounds, 0.4f);
        }
    }
}