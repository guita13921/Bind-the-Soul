using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Tooltip("Assign the player prefab to spawn")]
        [SerializeField]
        private Transform _playerPrefab;

        [Tooltip("Set one or more spawn points where the player is allowed to spawn")]
        [SerializeField]
        private List<Transform> _spawnPoints = new List<Transform>();

        [Tooltip("Set the duration in seconds to wait before the initial spawn and any respawns.")]
        [SerializeField]
        private float _spawnDelay = 0f;

        [Tooltip("Assign a camera that will render the scene before the player spawns.")]
        [SerializeField]
        private Camera _initialCamera;

        [Tooltip("Check this if you want to pick a random spawn point from the available positions.")]
        [SerializeField]
        private bool _randomizeSpawnPoint = false;

        public int spawnPointIndex
        {
            get { return _spawnPointIndex; }
            set { _spawnPointIndex = value; }
        }
        [Tooltip("Set the index of the spawn point to use for the player spawn.")]
        [SerializeField]
        private int _spawnPointIndex = 0;

        private Transform _playerInstance = null;
        private int _currSpawnPoint = 0;

        static public PlayerSpawner Instance;

        private void Awake()
        {
            // Must have only one PlayerSpawner in the scene
            if (Instance != null)
            {
                DestroyImmediate(this);
            } // if

            Instance = this;
        } // Awake()

        void Start()
        {
            if (_initialCamera != null && _spawnPoints.Count != 0)
            {
                Respawn();
            } // if
        } // Start()

        /// <summary>
        /// Wait the specified delay time then respawn the player
        /// </summary>
        /// <returns></returns>
        IEnumerator DelaySpawn()
        {
            yield return new WaitForSeconds(_spawnDelay);

            if (_spawnPoints.Count != 0)
            {
                if (_playerInstance != null)
                {
                    DestroyImmediate(_playerInstance.gameObject);
                } // if

                _playerInstance = Instantiate(_playerPrefab);
                _playerInstance.gameObject.SetActive(false);

                _playerInstance.position = _spawnPoints[_currSpawnPoint].position;
                _initialCamera.gameObject.SetActive(false);
                _playerInstance.gameObject.SetActive(true);
            } // if
        } // DelaySpawn()

        /// <summary>
        /// Respawn the player in the scene
        /// </summary>
        public void Respawn()
        {
            _currSpawnPoint = _randomizeSpawnPoint ? Random.Range(0, _spawnPoints.Count) : _spawnPointIndex;
            if (_currSpawnPoint >= _spawnPoints.Count)
            {
                Debug.LogError("Attempting to spawn to non-existent spawn point.");
                return;
            } // if

            _initialCamera.transform.position = new Vector3(_spawnPoints[_currSpawnPoint].position.x, _spawnPoints[_currSpawnPoint].position.y + 1f, _spawnPoints[_currSpawnPoint].position.z);

            StartCoroutine(DelaySpawn());
        } // Respawn()
    } // class PlayerSpawner
} // namespace Magique.SoulLink
