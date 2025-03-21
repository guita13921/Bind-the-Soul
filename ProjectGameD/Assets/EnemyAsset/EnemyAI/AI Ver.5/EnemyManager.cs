using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyManager : MonoBehaviour
    {
        EnemyLocomotionManager enemyLocomotionManager;
        public bool isPerformingAction;

        [Header("A.I Setting")]
        public float detectionRadius = 20f;

        //The Higher, and lower
        public float minimumDetectionAngle;
        public float maximumDetectionAngle;

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            HandleCurrentAction();
        }

        private void HandleCurrentAction()
        {
            if (enemyLocomotionManager.curretTarget == null)
            {
                enemyLocomotionManager.HandleDetection();
            }
            else
            {
                enemyLocomotionManager.HandleMoveToTarget();
            }


        }

        private void OnDrawGizmosSelected()
        {
            // Draw the detection radius as a wire sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw field of view lines
            Vector3 fovLine1 = Quaternion.AngleAxis(maximumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(minimumDetectionAngle, transform.up) * transform.forward * detectionRadius;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
        }

    }
}