#if SOULLINK_USE_AINAV
using System.Collections.Generic;
using UnityEngine;

// SpawnNavChecker
// This component is used to check an area in the game world to determine if a navmesh has been built before spawning an AI
// in that location. If the navmesh has not been built then this will initiate that process asynchronously. When the build
// process is complete, the AI will be spawned from the AsyncNavMeshBuilder component.
//
// (c)2021-2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    [RequireComponent(typeof(SphereCollider))]
    public class SpawnNavChecker : MonoBehaviour
    {
        private SphereCollider _sphereCollider;
        private List<SoulLinkTypes.SpawnData> _spawns = new List<SoulLinkTypes.SpawnData>();

        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
        } // Awake()

        public void SetRadius(float value)
        {
            _sphereCollider.radius = value;
        } // SetRadius()

        public void AddSpawn(SoulLinkTypes.SpawnData spawnData)
        {
            lock (_spawns)
            {
                _spawns.Add(spawnData);
            } // lock
        } // AddSpawn()

        private void OnTriggerEnter(Collider other)
        {
            // Check if the collision is against a AsyncNavMeshBuilder component. If so then enable it so it can start building navmesh
            if (!other.CompareTag(SoulLinkSpawnerTypes.NAVMESHBUILDER_TAG)) return;

            var asyncNavMeshBuilder = other.GetComponent<AsyncNavMeshBuilder>();
            if (asyncNavMeshBuilder != null)
            {
                if (!asyncNavMeshBuilder.enabled)
                {
                    asyncNavMeshBuilder.gameObject.GetComponent<AsyncNavMeshBuilder>().enabled = true;
                } // if

                lock (_spawns)
                {
                    foreach (var spawn in _spawns)
                    {
                        asyncNavMeshBuilder.QueueSpawn(spawn);
                    } // foreach

                    _spawns.Clear();
                } // lock

                asyncNavMeshBuilder.UpdateNavMesh();
            } // if
        } // OnTriggerEnter()
    } // class SpawnNavChecker
} // namespace Magique.SoulLink
#else
using UnityEngine;

namespace Magique.SoulLink
{
    [RequireComponent(typeof(SphereCollider))]
    public class SpawnNavChecker : MonoBehaviour
    {
    }
}
#endif