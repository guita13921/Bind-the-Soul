using System.Collections;
using UnityEngine;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SpawnPlaceholder : MonoBehaviour
    {
        public SpawnData SpawnData
        { get; set; }

        public bool Spawned
        { get { return _spawned; } }

        private bool _spawned = false;

        private float _tolerance = 0.3f;
        private float _minOcclusion;
        private float _maxOcclusion;

        private void Awake()
        {
            _minOcclusion = 0.0f - _tolerance;
            _maxOcclusion = 1.0F + _tolerance;
        } // Awake()

        /// <summary>
        /// Start checking for out of range and not in FOV
        /// </summary>
        private void OnEnable()
        {
            if (SpawnData == null) return;

            StartCoroutine(CheckOutOfRange());

            if (SpawnData.spawnPrefab != null)
            {
                StartCoroutine(CheckInFOV());
            }
            else
            {
                SoulLinkGlobal.DebugLog(gameObject, "Error: Prefab is null. Nothing will spawn.");
            }
        } // OnEnable()

        private void OnDisable()
        {
            StopAllCoroutines();
        } // OnDisable()

        /// <summary>
        /// Check for out of range every seconds to see if we need to deactivate the placeholder
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckOutOfRange()
        {
            while (!_spawned)
            {
                yield return new WaitForSeconds(1f);

                var dist = Vector3.Distance(transform.position, SoulLinkSpawner.Instance.PlayerTransform.position);
                if (Vector3.Distance(transform.position, SoulLinkSpawner.Instance.PlayerTransform.position) > (SoulLinkSpawner.Instance.SpawnRadius + 32f))
                {
                    SoulLinkGlobal.DebugLog(gameObject, "Out of Range. Distance = " + dist + " prefab = " + SpawnData.spawnPrefab.name);

                    // Release back into the spawn placeholder pool
                    SoulLinkSpawner.Instance.ReleasePlaceholder(this);
                } // if
                else
                {
                    CheckInFOV();
                }
            } // while

            yield return null;
        } // ChecOutOfRange()

        /// <summary>
        /// Reset placeholder so it can be used again
        /// </summary>
        public void ResetToDefault()
        {
            _spawned = false;
        } // ResetToDefault();

        /// <summary>
        /// Check if the placeholder is not longer in the camera's FOV so we can spawn the instance when not in view.
        /// Special thanks to Volker Dietz for the camera check code.
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckInFOV()
        {
            while (!_spawned)
            {
                if (Camera.main != null)
                {
                    Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position, Camera.MonoOrStereoscopicEye.Mono);
                    bool onScreen = screenPoint.z > 0 && screenPoint.x > _minOcclusion && screenPoint.x < _maxOcclusion &&
                        screenPoint.y > _minOcclusion && screenPoint.y < _maxOcclusion;
                    if (!onScreen && Vector3.Distance(SoulLinkSpawner.Instance.PlayerTransform.position, transform.position) >= SoulLinkSpawner.Instance.MinimumSpawnRange)
                    {
                        SpawnInstance();
                    } // if
                } // if

                yield return new WaitForSeconds(0.1f);
            } // while
        } // CheckInFOV()

        /// <summary>
        /// Spawn the actual prefab instance
        /// </summary>
        private void SpawnInstance()
        {
            if (_spawned || SoulLinkSpawner.Instance == null) return;
            if (!Application.isPlaying || SoulLinkSpawner.Quitting) return;

            _spawned = true;

            SoulLinkSpawner.Instance.Spawn(SpawnData);

            // Release back into the spawn placeholder pool
            SoulLinkSpawner.Instance.ReleasePlaceholder(this);
        } // SpawnInstance()
    } // class SpawnPlaceholder
} // namespace Magique.SoulLink
