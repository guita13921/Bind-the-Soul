using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Magique.SoulLink.SoulLinkSpawnerTypes;

// (c)2020-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class RangedActivate : MonoBehaviour
    {
        [SerializeField]
        private float _activateRange = 30f;

        [SerializeField]
        private List<Transform> _activateObjects = new List<Transform>();

        private FlexCollider _collider;
        private bool _isTriggered = false;

        private void Awake()
        {
            // Prepare components as needed
            if (SoulLinkSpawner.Instance.SpawnMode == SpawnMode.SpawnIn3D)
            {
                _collider = gameObject.AddComponent<FlexCollider>();
                _collider.CreateCollider();
                _collider.Radius = _activateRange;
                _collider.IsTrigger = true;
            }
            else
            {
                _collider = gameObject.AddComponent<FlexCollider>();
                _collider.CreateCollider(false);
                _collider.Radius = _activateRange;
                _collider.IsTrigger = true;
            }

            _collider.Enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Start all objects as inactive
            foreach (var obj in _activateObjects)
            {
                obj.gameObject.SetActive(false);
            } // foreach
        } // Awake()

        /// <summary>
        /// Startup code for the ranged activate script
        /// </summary>
        private void Start()
        {
            StartCoroutine(DelayEnable());
        } // Start()

        /// <summary>
        /// Wait for one second and then activate the collider
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayEnable()
        {
            yield return new WaitForSeconds(1f);

            _collider.Enabled = true;
        } // DelayEnable()

        /// <summary>
        /// If the collider is triggered, activate the objects that are in the activateObjects list
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || !other.CompareTag(SoulLinkSpawner.Instance.PlayerTag)) return;

            _isTriggered = true;

            foreach (var obj in _activateObjects)
            {
                obj.gameObject.SetActive(true);
            } // foreach
        } // OnTriggerEnter()

        /// <summary>
        /// If the player exits the collider range then deactivate all the objects in the activateObjects list
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!_isTriggered || !other.CompareTag(SoulLinkSpawner.Instance.PlayerTag)) return;

            _isTriggered = false;

            foreach (var obj in _activateObjects)
            {
                obj.gameObject.SetActive(false);
            } // foreach
        } // OnTriggerExit()

        /// <summary>
        /// If the collider is triggered, activate the objects that are in the activateObjects list
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered || !other.CompareTag(SoulLinkSpawner.Instance.PlayerTag)) return;

            _isTriggered = true;

            foreach (var obj in _activateObjects)
            {
                obj.gameObject.SetActive(true);
            } // foreach
        } // OntriggerEnter2D()

        /// <summary>
        /// If the player exits the collider range then deactivate all the objects in the activateObjects list
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_isTriggered || !other.CompareTag(SoulLinkSpawner.Instance.PlayerTag)) return;

            _isTriggered = false;

            foreach (var obj in _activateObjects)
            {
                obj.gameObject.SetActive(false);
            } // foreach
        } // OnTriggerExit2D()
    } // class RangedActivate
} // namespace Magique.SoulLink
