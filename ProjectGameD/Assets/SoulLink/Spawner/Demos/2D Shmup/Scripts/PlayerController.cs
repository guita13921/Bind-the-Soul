using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 2f;

        [SerializeField]
        private GameObject _playerSprite;

        [SerializeField]
        private List<Image> _lifeIcons = new List<Image>();

        [SerializeField]
        private Vector2 _minLimits = new Vector2(-8.5f, -4.0f);

        [SerializeField]
        private Vector2 _maxLimits = new Vector2(8.5f, 0.5f);

        [SerializeField]
        private Transform _bullet;

        [SerializeField]
        private float _fireRate = 0.5f;

        [SerializeField]
        private Transform _deathEffect;

        [SerializeField]
        private AudioClip _deathSound;

        [SerializeField]
        private float _deathEffectScale = 1f;

        [SerializeField]
        private float _respawnTime = 3f;

        [SerializeField]
        private AudioClip _respawnSound;

        private Vector3 _startPosition;

        private bool _isAlive = true;
        private bool _isInvincible = false;
        private float _fireTimer = 0f;

        private float _saveFireRate = 0f;
        private int _numLives = 3;

        private void Awake()
        {
            _startPosition = transform.position;

            UpdateLives();
        } // Awake()

        void Update()
        {
            if (!_isAlive) return;

            _fireTimer += Time.deltaTime;

			float hMove = Input.GetAxis("Horizontal");
			float vMove = Input.GetAxis("Vertical");

            var deltaX = hMove * Time.deltaTime * _speed;
            var deltaY = vMove * Time.deltaTime * _speed;

            if (transform.position.x + deltaX > _maxLimits.x || transform.position.x + deltaX < _minLimits.x)
                deltaX = 0f;
            if (transform.position.y + deltaY > _maxLimits.y || transform.position.y + deltaY < _minLimits.y)
                deltaY = 0f;
            transform.position = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0f);

            if (Input.GetKey(KeyCode.Space) && _fireTimer >= _fireRate)
            {
                // spawn a bullet
                SpawnData spawnData = new SpawnData() { spawnPrefab = _bullet, spawnPos = transform.position, spawnCategory = "Bullets" };
                SoulLinkSpawner.Instance.Spawn(spawnData);

                _fireTimer = 0f;
            } // if
        } // Update()

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isAlive || _isInvincible) return;

            if (other.CompareTag("PowerUp"))
            {
                var powerup = other.gameObject.GetComponent<PowerUp>();
                if (powerup.PowerUpTypeRef == PowerUp.PowerUpType.FireRate && _saveFireRate == 0f)
                {
                    _saveFireRate = _fireRate;
                    _fireRate /= 2f;
                    _fireTimer = 0f;

                    Invoke("ResetFireRate", 10f);

                    powerup.Collect();
                } // if
            }
            else if (other.CompareTag("Enemy") || other.CompareTag("EnemyMissile"))
            {
                _playerSprite.SetActive(false);

                SpawnData spawnData = new SpawnData() { spawnPrefab = _deathEffect, minScale = _deathEffectScale, maxScale = _deathEffectScale, spawnPos = transform.position, spawnCategory = "Effects" };
                SoulLinkSpawner.Instance.Spawn(spawnData);

                SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _deathSound, soundIs3D = false }, Vector3.zero);

                --_numLives;
                _isAlive = false;

                UpdateLives();
                ResetFireRate();

                if (_numLives == 0)
                {
                    GameManager.Instance.GameOver();
                }
                else
                {
                    Invoke("Respawn", _respawnTime);
                }
            }
        } // OnTriggerEnter2D()

        void UpdateLives()
        {
            for (int i = 0; i < _lifeIcons.Count; ++i)
            {
                _lifeIcons[i].enabled = ((i + 1) <= _numLives);
            } // for i
        } // UpdateLives()

        void ResetFireRate()
        {
            if (_saveFireRate == 0f) return;

            _fireRate = _saveFireRate;
            _saveFireRate = 0f;

            CancelInvoke();
        } // ResetFireRate()

        private void TurnOffInvincibility()
        {
            _isInvincible = false;
        } // TurnOffInvincibility()

        private void Respawn()
        {
            transform.position = _startPosition;
            _isAlive = true;
            _playerSprite.SetActive(true);

            if (_respawnSound != null)
            {
                SoulLinkGlobal.Instance.PlaySound(new SoundData() { audioClip = _respawnSound, soundIs3D = false }, Vector3.zero);
            } // if

            _isInvincible = true;
            Invoke("TurnOffInvincibility", 2f);
        } // Respawn()
    } // class PlayerController
} // namespace Magique.SoulLink
