using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [AddComponentMenu("SoulLink/Activate Wave Trigger")]
    public class ActivateWaveTrigger : MonoBehaviour
    {
        [SerializeField]
        private WaveSpawner _waveSpawner;

        [SerializeField]
        private string _waveItemName;

        [SerializeField]
        private float _triggerRadius = 1f;

        private FlexCollider _collider;
        private bool _isTriggered = false;

        /// <summary>
        /// Perform setup for the collider
        /// </summary>
        private void Awake()
        {
            CreateCollider();
        } // Awake()

        /// <summary>
        /// Start checking for the wave to start
        /// </summary>
        private void Start()
        {
            StartCoroutine(DelayColliderEnable());
        } // Start()

        /// <summary>
        /// Wait until the wave has started before enabling the collider
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayColliderEnable()
        {
            if (_waveSpawner != null)
            {
                while (!_waveSpawner.WaveStarted)
                {
                    yield return new WaitForEndOfFrame();
                } // while

                _collider.Enabled = true;
            } // if
        } // DelayColliderEnable()

        /// <summary>
        /// Create the collider this activator
        /// </summary>
        public void CreateCollider()
        {
            _collider = gameObject.AddComponent<FlexCollider>();

            if (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D)
            {
                _collider.CreateCollider();
            }
            else
            {
                _collider.CreateCollider(false);
            }

            _collider.Radius = _triggerRadius;
            _collider.IsTrigger = true;
            _collider.Enabled = false;
        } // CreateCollider()

        /// <summary>
        /// Perform the wave item trigger when the player enters 
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || _waveSpawner == null || string.IsNullOrEmpty(_waveItemName) || !_waveSpawner.WaveStarted) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                _isTriggered = true;
                _waveSpawner.PerformWaveTrigger(_waveItemName);
            } // if
        } // OnTriggerEnter()

        /// <summary>
        /// Perform the wave item trigger when the player enters 
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered || _waveSpawner == null || string.IsNullOrEmpty(_waveItemName) || !_waveSpawner.WaveStarted) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                _isTriggered = true;
                _waveSpawner.PerformWaveTrigger(_waveItemName);
            } // if
        } // OnTriggerEnter2D()
    } // class ActivateWaveTrigger
} // namespace Magique.SoulLink
