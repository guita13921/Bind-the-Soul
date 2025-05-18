using System.Collections.Generic;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PowerUp : MonoBehaviour, ISpawnEvents
    {
        public enum PowerUpType
        {
            Speed,
            FireRate,
            Missile
        }

        public PowerUpType PowerUpTypeRef
        { get { return _powerupType; } }

        [SerializeField]
        private PowerUpType _powerupType = PowerUpType.FireRate;

        [SerializeField]
        private List<AudioClip> _powerUpSounds = new List<AudioClip>();

        [SerializeField]
        private float _powerUpSoundVolume = 1f;

        [SerializeField]
        private int _points = 25;

        public void Collect()
        {
            foreach (var sound in _powerUpSounds)
            {
                SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = sound, soundIs3D = false, soundVolume = _powerUpSoundVolume }, Vector3.zero);
            } // foreach

            GameManager.Instance.AddPoints(_points, true, transform);

            GetComponent<ISpawnable>().ForceDespawn();
        } // Collect()

        public void OnSpawned()
        {
            GetComponent<Spawnable>().SetSpawnCategory("Powerup");
        } // OnSpawned()

        public void OnDespawned()
        {
            // nothing
        } // OnDespawned()
    } // class PowerUp
} // namespace Magique.SoulLink
