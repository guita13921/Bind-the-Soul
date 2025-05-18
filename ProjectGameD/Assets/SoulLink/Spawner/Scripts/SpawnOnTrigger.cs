using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Spawn On Trigger")]
    public class SpawnOnTrigger : MonoBehaviour
    {
        [SerializeField]
        private Transform _spawnPrefab;

        [SerializeField]
        private string _triggerTag = "Player";

        public float TriggerRange
        { get { return _triggerRange; } }

        [SerializeField]
        private float _triggerRange = 4f;

        [SerializeField]
        private float _spawnDelay = 0f;

        [SerializeField]
        private Transform _spawnPos;

        [SerializeField]
        private bool _validateSpawnPos = true;

        [SerializeField]
        private int _populationCap = 10;
		
        [Tooltip("If the spawn can be reused from an object pool then check this.")]
        [SerializeField]
        private bool _reusable = true;

        [SerializeField]
        private UnityEvent _onSpawnEventHandler;

        private FlexCollider _collider;

        public bool IsTriggered
        { get { return _isTriggered; } }

        private bool _isTriggered = false;

        /// <summary>
        /// Set the proper collider parameters and get a reference to the scene's PoolManager if it exists
        /// </summary>
        private void Awake()
        {
            // Prepare components as needed
            if (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D)
            {
                _collider = gameObject.AddComponent<FlexCollider>();
                _collider.CreateCollider();
                _collider.Radius = _triggerRange;
                _collider.IsTrigger = true;
            }
            else
            {
                _collider = gameObject.AddComponent<FlexCollider>();
                _collider.CreateCollider(false);
                _collider.Radius = _triggerRange;
                _collider.IsTrigger = true;
            }

            var poolManager = FindObjectOfType<PoolManager>();
            if (poolManager != null)
            {
                poolManager.AddPoolItem(_spawnPrefab, _populationCap, SoulLinkSpawner.Instance.Database);
            } // if
        } // Awake()

        /// <summary>
        /// Set the current triggered status
        /// </summary>
        /// <param name="value"></param>
        public void SetIsTriggered(bool value)
        {
            _isTriggered = value;

            _collider.Enabled = !_isTriggered;
        } // SetIsTriggered()

        /// <summary>
        /// If the collider is triggered, call the user defined event handler and spawn the prefab
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_triggerTag)) return;

            _isTriggered = true;
            _collider.Enabled = false; // only triggers once and then disables

            StartCoroutine(DelaySpawn());
        } // OnTriggerEnter()


        /// <summary>
        /// If the collider is triggered, call the user defined event handler and spawn the prefab
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(_triggerTag)) return;

            _isTriggered = true;
            _collider.Enabled = false; // only triggers once and then disables

            StartCoroutine(DelaySpawn());
        } // OnTriggerEnter2D()

        IEnumerator DelaySpawn()
        {
            yield return new WaitForSeconds(_spawnDelay);

            if (_onSpawnEventHandler != null)
            {
                _onSpawnEventHandler.Invoke();
            } // if

            // Barebones spawn data
            SpawnData spawnData = new SpawnData()
            {
                spawnPrefab = _spawnPrefab,
                spawnCategory = DEFAULT_SPAWN_CATEGORY_NAME, // TODO: allow this to be set in the trigger component
                spawnPos = _spawnPos != null ? _spawnPos.transform.position : transform.position,
                spawnRot = transform.rotation
            };
            SoulLinkSpawner.Instance.TrySpawn(spawnData);//, false, false, _validateSpawnPos);
        } // DelaySpawn()
    } // class SpawnOnTrigger
} // namespace Magique.SoulLink

