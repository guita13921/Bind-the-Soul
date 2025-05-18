#if SOULLINK_USE_AINAV
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_2020_3_OR_NEWER && !SURVIVAL_ENGINE && !SURVIVAL_ENGINE_ONLINE
using Unity.AI.Navigation; 
#endif

// NavMeshLinkActivator
//
// This component is placed on the terrain wherever nav mesh links are needed. When an AI triggers this, a pre-cached NavMeshLink is
// placed in this position so that the AI can cross nav mesh areas.
//
// Copyright 2021 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [RequireComponent(typeof(SphereCollider))]
    public class NavMeshLinkActivator : MonoBehaviour
    {
        [SerializeField]
        private float _range = 16f;

        private NavMeshLink _navMeshLink;
        private float _angleX = 0f;
        private float _angleZ = 0f;
        private Rigidbody _rigidbody;

        // A list of AI that have triggered this
        private List<GameObject> _triggers = new List<GameObject>();

        private bool _triggered = false;

        private void Awake()
        {
            // Get the sphere collider and set the properties as required
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = _range;

/*            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.Sleep();*/
        } // Awake()

        private void Start()
        {
            StartCoroutine(CheckTriggerState());
        } // Start()

        public void SetAngles(float angleX, float angleZ)
        {
            _angleX = angleX;
            _angleZ = angleZ;
        } // SetAngles()

        private void OnTriggerEnter(Collider other)
        {
            if (!SoulLinkSpawner.Instance.IsNavMeshLinkActivatorTag(other.tag)) return;

            if (!_triggers.Contains(other.gameObject))
            {
                lock (_triggers)
                {
                    _triggers.Add(other.gameObject);
                } // lock
            } // if

            if (!_triggered)
            {
                _navMeshLink = SoulLinkSpawner.Instance.FetchNavMeshLink().GetComponent<NavMeshLink>();
                _navMeshLink.transform.position = transform.position;
                _navMeshLink.transform.localRotation = Quaternion.Euler(_angleX, 0f, _angleZ);
                _navMeshLink.gameObject.SetActive(true);
                _navMeshLink.autoUpdate = !_navMeshLink.autoUpdate;

                _triggered = true;
            } // if
        } // OnCollisionEnter()

        private void OnTriggerExit(Collider other)
        {
            if (!SoulLinkSpawner.Instance.IsNavMeshLinkActivatorTag(other.tag)) return;

            lock (_triggers)
            {
                _triggers.Remove(other.gameObject);
            } // lock
        } // OnCollisionExit()

        IEnumerator CheckTriggerState()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (!_triggered) continue;

                lock (_triggers)
                {
                    for (int i = 0; i < _triggers.Count; ++i)
                    {
                        if (_triggers[i] == null || !_triggers[i].activeSelf)
                        {
                            _triggers.RemoveAt(i);
                        } // if
                    } // for i
                } // lock

                if (_triggers.Count == 0 && _navMeshLink != null)
                {
                    _navMeshLink.gameObject.SetActive(false);
                    SoulLinkSpawner.Instance.ReleaseNavMeshLink(_navMeshLink.transform);

                    _navMeshLink = null;
                    _triggered = false;
                } // if
            } // while
        } // CheckTriggerState()
    } // class NavMeshLinkActivator
} // namespace Magique.SoulLink
#else
using UnityEngine;

namespace Magique.SoulLink
{
    [RequireComponent(typeof(SphereCollider))]
    public class NavMeshLinkActivator : MonoBehaviour
    { }
}
#endif