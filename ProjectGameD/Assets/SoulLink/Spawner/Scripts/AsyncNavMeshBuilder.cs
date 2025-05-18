#if SOULLINK_USE_AINAV
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

#if UNITY_2020_3_OR_NEWER && !SURVIVAL_ENGINE && !SURVIVAL_ENGINE_ONLINE && !FARMING_ENGINE
using Unity.AI.Navigation;
#endif

// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    // Build and update a localized navmesh from the sources marked by NavMeshSourceTag
    // Spawn AI that are queued from Spawner system
    [RequireComponent(typeof(BoxCollider))]
    [DefaultExecutionOrder(-102)]
    public class AsyncNavMeshBuilder : MonoBehaviour
    {
        NavMeshData _navMesh;
        AsyncOperation _asyncOp;
        NavMeshDataInstance _instance;
        List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

        // The size of the build bounds
        [SerializeField]
        private Vector3 _size = new Vector3(80.0f, 20.0f, 80.0f);

        private BoxCollider _boxCollider;
        private bool _navMeshBuilt = false;
        private bool _isBuilding = false;
        private Queue<SoulLinkTypes.SpawnData> _spawns = new Queue<SoulLinkTypes.SpawnData>();

        private List<AsyncNavMeshBuilder> _neighbors = new List<AsyncNavMeshBuilder>();

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.size = _size;
        } // Awake()

        void OnEnable()
        {
            // Construct and add navmesh
            _navMesh = new NavMeshData();
            _instance = NavMesh.AddNavMeshData(_navMesh);
        } // OnEnable()

        void OnDisable()
        {
            // Unload navmesh and clear handle
            _instance.Remove();
        } // OnDisable()

        private void Update()
        {
            if (_navMeshBuilt)
            {
                SpawnQueuedItems();

                return;
            } // if

            if (_asyncOp != null)
            {
                if (_asyncOp.progress == 1f)
                {
                    _navMeshBuilt = true;
                    _isBuilding = false;

                    EnableNavMeshLinks();
                    SpawnQueuedItems();
                }
                //Debug.Log("async progress = " + _asyncOp.progress);
            }
        } // Update()

        public void QueueSpawn(SoulLinkTypes.SpawnData spawnData)
        {
            lock (_spawns)
            {
                _spawns.Enqueue(spawnData);
            } // lock
        } // QueueSpawn()

        public void AddNeighbor(AsyncNavMeshBuilder neighbor)
        {
            if (!_neighbors.Contains(neighbor) && _neighbors.Count < 4)
            {
                _neighbors.Add(neighbor);
            } // if
        } // AddNeighbor()

        public void EnableNavMeshLinks()
        {
            NavMeshLink[] navMeshLinks = GetComponentsInChildren<NavMeshLink>();
            foreach (var link in navMeshLinks)
            {
                link.enabled = true;
            } // foreach
        } // EnableNavMeshLinks()

        void SpawnQueuedItems()
        {
            lock (_spawns)
            {
                while (_spawns.Count > 0)
                {
                    var spawnData = _spawns.Peek();
                    if (!SoulLinkSpawner.Instance.PopulationReached(spawnData))
                    {
                        SoulLinkSpawner.Instance.TrySpawn(spawnData);
                    } // if
                    _spawns.Dequeue();
                } // while
            } // lock
        } // SpawnQueuedItems()

        public void UpdateNavMesh()
        {
            if ((_navMeshBuilt || _isBuilding) && _asyncOp != null) return;
            _isBuilding = true;

            SoulLinkNavMeshSourceTag.CollectMeshes(ref _sources);
            SoulLinkNavMeshSourceTag.CollectModifierVolumes(LayerMask.GetMask("Default"), ref _sources);

            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            var bounds = new Bounds(transform.position, _size);

            defaultBuildSettings.agentSlope = SoulLinkSpawner.Instance.AgentSlope;
            defaultBuildSettings.agentHeight = SoulLinkSpawner.Instance.AgentHeight;
            defaultBuildSettings.agentClimb = SoulLinkSpawner.Instance.AgentStepHeight;
            defaultBuildSettings.agentRadius = 0.25f;

            _asyncOp = NavMeshBuilder.UpdateNavMeshDataAsync(_navMesh, defaultBuildSettings, _sources, bounds);
        } // UpdateNavMesh()

        public void SetSize(int width, int height)
        {
            _size = new Vector3(width, height, width);
            _boxCollider.size = _size;
        } // SetSize()

        void OnDrawGizmosSelected()
        {
            if (_navMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_navMesh.sourceBounds.center, _navMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            var bounds = new Bounds(transform.position, _size * 1.05f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, _size);
        } // OnDrawGizmosSelected()
    } // class AsyncNavMeshBuilder
} // namespace Magique.SoulLink
#else
using UnityEngine;

namespace Magique.SoulLink
{
    // Build and update a localized navmesh from the sources marked by NavMeshSourceTag
    // Spawn AI that are queued from Spawner system
    [RequireComponent(typeof(BoxCollider))]
    [DefaultExecutionOrder(-102)]
    public class AsyncNavMeshBuilder : MonoBehaviour
    {
    }
}
#endif