using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class WaveTrigger : MonoBehaviour
    {
        private FlexCollider _collider;

        private bool _isTriggered = false;
        private bool _isValid = true;
        private bool _is3D = true;

        private WaveSpawner _waveSpawner = null;
        private WaveItemData _waveItemData = null;

        private void Awake()
        {
            _is3D = (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D);
        } // Awake()

        /// <summary>
        /// Set the associated wve spawner and wave spanw data for this trigger
        /// </summary>
        /// <param name="spawner"></param>
        /// <param name="waveItem"></param>
        public void SetWaveSpawner(WaveSpawner spawner, WaveItemData waveItem)
        {
            _waveSpawner = spawner;
            _waveItemData = waveItem;
        } // SetWaveSpawner()

        /// <summary>
        /// Reset the trigger so it can be fired again
        /// </summary>
        public void ResetTrigger()
        {
            _isTriggered = false;
        } // ResetTrigger()

        public void EnableCollider()
        {
            if (_waveItemData.spawningTriggerMethod == SpawningTriggerMethod.Auto)
            {
                PerformTrigger();
                return;
            } // if

            if (_collider == null) return;
            _collider.Enabled = true;
        } // EnableCollider()

        public void DisableCollider()
        {
            if (_collider == null) return;
            _collider.Enabled = false;
        } // DisableCollider()

        /// <summary>
        /// Create the trigger object for this wave
        /// </summary>
        public void CreateTrigger()
        {
            if (_waveItemData == null) return;

            if (_waveItemData.spawningTriggerMethod != SpawningTriggerMethod.Manual && _waveItemData.spawningTriggerMethod != SpawningTriggerMethod.Auto)
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

                _collider.Radius = _waveItemData.spawningTriggerMethod == SpawningTriggerMethod.GlobalRadius ?
                    SoulLinkSpawner.Instance.SpawnRadius : _waveItemData.overrideTriggerRadius;
                _collider.IsTrigger = true;
                _collider.Enabled = false;
            } // if
        } // CreateTrigger()

        /// <summary>
        /// OnTriggerEnter
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || !_isValid) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                PerformTrigger();
            } // if
        } // OnTriggerEnter()

        /// <summary>
        /// OnTriggerEnter2D
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered || !_isValid) return;

            if (other.CompareTag(SoulLinkSpawner.Instance.PlayerTag))
            {
                PerformTrigger();
            } // if
        } // OnTriggerEnter2D()

        /// <summary>
        /// Trigger spawning
        /// </summary>
        public void PerformTrigger()
        {
            _isTriggered = true;

            if (_waveSpawner != null && _waveItemData != null)
            {
                // Call user's custom triggered event handler
                _waveSpawner.SendEventMessage(WaveSpawner.OnTriggeredMessageName, _waveItemData);

                _waveSpawner.StartSpawning(_waveItemData);
            } // if
        } // PerformTrigger()
    } // class WaveTrigger()
} // namespace Magique.SoulLink
