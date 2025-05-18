using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Magique.SoulLink
{
    public static class NavMeshAgentExtensionMethods
    {
        public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
        {
            if (navMeshAgent.pathPending ||
                navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
                navMeshAgent.path.corners.Length == 0)
                return -1f;

            float remainingDistance = 0f;
            if (navMeshAgent.hasPath && navMeshAgent.remainingDistance != Mathf.Infinity)
            {
                remainingDistance = navMeshAgent.remainingDistance;
            }
            else
            {
                for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
                {
                    remainingDistance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
                } // for i
            }

            return remainingDistance;
        } // GetPathRemainingDistance
    } // class NavMeshAgentExtensionMethods
} // namespace Magique.SoulLink