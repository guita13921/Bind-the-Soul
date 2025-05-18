using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Magique.SoulLink.ShaderUtils;

// (c)2021-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WanderAI : Spawnable
    {
        public enum BehaviorType
        {
            Stationary,
            Wander, 
            Waypoints
        }

        [SerializeField]
        private BehaviorType _behaviorType = BehaviorType.Wander;

        [SerializeField]
        private float _wanderRadius = 5f;

        [SerializeField]
        private float _minWanderRange = 1f;

        [SerializeField]
        private float _wanderWaitMin = 3f;

        [SerializeField]
        private float _wanderWaitMax = 8f;

        [SerializeField]
        private List<Transform> _waypoints;

        [SerializeField]
        private bool _isPlayer = false;

        private Vector3 _homePosition;
        private NavMeshAgent _agent;

        private const int VALIDATION_RETRIES = 3;

        override public void Awake()
        {
            base.Awake();

            _agent = GetComponent<NavMeshAgent>();
        } // Awake()

        override public void SpawnActionComplete()
        {
            base.SpawnActionComplete();

            StartBoid();
        } // FadeInComplete()

        public override void Start()
        {
            if (_isPlayer)
            {
                SetVisible(_renderers, _materials);
                StartBoid();
            }
            else
            {
                base.Start();
            }
        } // Start()

        void StartBoid()
        {
            EnableNavMeshAgent();

            // Set home position to starting point
            _homePosition = transform.position;

            if (_behaviorType == BehaviorType.Wander)
            {
                StartCoroutine(Wander());
            } 
            else if (_behaviorType == BehaviorType.Waypoints)
            {
                StartCoroutine(Waypoints());
            } // if
        } // StartBoid();

        IEnumerator Wander()
        {
            while (true && _agent.enabled && _agent.isOnNavMesh)
            {
                yield return new WaitForSeconds(Random.Range(_wanderWaitMin, _wanderWaitMax));
                var destPos = GetRandomPos(_minWanderRange, _wanderRadius, _homePosition);

                bool isValid;
                destPos = ValidateSpawnPos(destPos, out isValid);
                if (isValid && _agent.isOnNavMesh)
                {
                    // Set NavMeshAgent destination
                    NavMeshPath path = new NavMeshPath();
                    _agent.CalculatePath(destPos, path);
                    _agent.path = path;
                    if (_agent.hasPath)
                    {
                        _agent.SetDestination(destPos);
                    } // if
                } // if

                yield return new WaitForSeconds(1f);

                // Wait until destination is reached
                while (_agent.hasPath && _agent.remainingDistance > float.Epsilon)
                {
                    yield return new WaitForEndOfFrame();
                } // while
            } // while
        } // Wander()

        IEnumerator Waypoints()
        {
            int wayPointIndex = 0;
            while (true)
            {
                // Set NavMeshAgent destination
                NavMeshPath path = new NavMeshPath();
                _agent.CalculatePath(_waypoints[wayPointIndex].position, path);
                _agent.path = path;
                _agent.SetDestination(_waypoints[wayPointIndex].position);

                yield return new WaitForSeconds(1f);

                // Wait until destination is reached
                while (_agent.remainingDistance > float.Epsilon && _agent.hasPath)
                {
                    yield return new WaitForEndOfFrame();
                } // while

                wayPointIndex++;
                if (wayPointIndex == _waypoints.Count) wayPointIndex = 0;

                yield return new WaitForSeconds(Random.Range(_wanderWaitMin, _wanderWaitMax));
            } // while
        } // Waypoints()

        Vector3 ValidateSpawnPos(Vector3 spawnPos, out bool isValid)
        {
            int tries = 0;
            var origSpawnPos = spawnPos;
            isValid = false;
            while (!isValid && tries < VALIDATION_RETRIES)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPos, out hit, 2.0f, NavMesh.AllAreas))
                {
                    isValid = true;
                    return spawnPos;
                } // if

                SoulLinkGlobal.DebugLog(gameObject, "Failed to spawn. Could not find valid spawn position at " + spawnPos);

                // Failed, so try to find nearby spot to spawn somewhere within 2f radius
                spawnPos = GetRandomPos(2f, 2f, origSpawnPos);

                ++tries;
            } // while

            // Failed. Just return 0 and mark as invalid
            isValid = false;
            return Vector3.zero;
        } // ValidateSpawnPos()

        public Vector3 GetRandomPos(float minRange, float maxRange, Vector3 startPos)
        {
            float x = Random.Range(minRange, maxRange);
            if (Random.Range(0, 100) < 50) x *= -1f;
            float z = Random.Range(minRange, maxRange);
            if (Random.Range(0, 100) < 50) z *= -1f;

            x += startPos.x;
            z += startPos.z;

            float y = Utility.GetAdjustedYPos(x, startPos.y, z, SoulLinkSpawner.Instance != null ? SoulLinkSpawner.Instance.RaycastDistance : 1000f);

            return new Vector3(x, y, z);
        } // GetRandomPos()
    } // class WanderAI
} // namespace Magique.SoulLink