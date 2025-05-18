using System.Collections;
using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Enemy : MonoBehaviour, ISpawnEvents
    {
        [SerializeField]
        private GameObject _sprite;

        [SerializeField]
        private Transform _bullet;

        [SerializeField]
        private float _fireRate = 3f;

        [SerializeField]
        private int _hitsToKill = 1;

        [SerializeField]
        private AudioClip _spawnSound;

        [SerializeField]
        private Transform _impactEffect;

        [SerializeField]
        private AudioClip _impactSound;

        [SerializeField]
        private float _impactVolume = 1f;

        [SerializeField]
        private Transform _deathEffect;

        [SerializeField]
        private AudioClip _deathSound;

        [SerializeField]
        private float _deathVolume = 1f;

        [SerializeField]
        private float _deathEffectScale = 1f;

        [SerializeField]
        private int _points = 50;

        private bool _isAlive = true;
        private int _hitsTaken = 0;
        private bool _canFire = false;

        public void OnSpawned()
        {
            _isAlive = true;
            _hitsTaken = 0;

            if (_spawnSound != null)
            {
                SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _spawnSound, soundIs3D = false, soundVolume = 1f }, Vector3.zero);
            } // if

            StartCoroutine(FireBullet());
        } // OnSpawned()

        private void OnBecameVisible()
        {
            _canFire = true;
        } // OnBecameVisible()

        private void OnBecameInvisible()
        {
            _canFire = false;
        } // OnBecameInvisible()

        IEnumerator FireBullet()
        {
            while (_isAlive)
            {
                yield return new WaitForSeconds(_fireRate);

                if (_canFire && _bullet != null)
                {
                    // spawn a bullet
                    SpawnData spawnData = new SpawnData() { spawnPrefab = _bullet, spawnPos = transform.position, spawnCategory = "Bullets" };
                    SoulLinkSpawner.Instance.Spawn(spawnData);
                } // if
            } // while
        } // FireBullet()

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isAlive) return;

            if (other.CompareTag("PlayerMissile"))
            {

                ++_hitsTaken;
                _isAlive = (_hitsTaken < _hitsToKill);

                if (!_isAlive)
                {
                    SpawnData spawnData = new SpawnData() { spawnPrefab = _deathEffect, minScale = _deathEffectScale, maxScale = _deathEffectScale, spawnPos = transform.position, 
                        spawnCategory = "Effects" };
                    SoulLinkSpawner.Instance.Spawn(spawnData);

                    SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _deathSound, soundIs3D = false, soundVolume = _deathVolume }, Vector3.zero);

                    StopCoroutine(FireBullet());
                    GetComponent<Spawnable>().OnDeath();

                    GameManager.Instance.AddPoints(_points, true, transform);
                } // if

                if (_impactEffect != null)
                {
                    if (_impactSound != null)
                    {
                        SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _impactSound, soundIs3D = false, soundVolume = _impactVolume }, Vector3.zero);
                    } // if

                    SpawnData spawnData = new SpawnData() { spawnPrefab = _impactEffect, spawnPos = transform.position, spawnCategory = "Effects" };
                    SoulLinkSpawner.Instance.Spawn(spawnData);
                } // if
            } // if
        } // OnTriggerEnter2D()

        public void OnDespawned()
        {
            // nothing
        } // OnDespawned()
    } // class Enemy
} // namespace Magique.SoulLink