using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class HeadLookController : MonoBehaviour
    {

        EnemyManager enemyManager;

        [Header("Head Look Settings")]
        public Transform target;         // Target to look at
        public Transform head;           // The head bone or reference forward direction
        public float sensitivity = 1.0f; // Control how sharp the blend is
        public float maxAngle = 90f;     // Maximum angle to map to blend range

        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
            target = FindObjectOfType<PlayerManager>().gameObject.transform;
        }


        void Update()
        {
            Vector3 directionToTarget = target.position - head.position;
            Vector3 localDirection = head.InverseTransformDirection(directionToTarget.normalized);

            // Project onto XZ and Y axes (forward = Z+)
            float horizontal = Mathf.Clamp(localDirection.x, -1f, 1f);
            float vertical = Mathf.Clamp(localDirection.y, -1f, 1f);

            // Optional: remap from angle if you want more precision
            float angle = Vector3.Angle(head.forward, directionToTarget);
            if (angle > maxAngle)
            {
                horizontal = 0f;
                vertical = 0f;
            }

            // Apply smoothing or sensitivity
            horizontal *= sensitivity;
            vertical *= sensitivity;

            // Set Animator parameters
            animator.SetFloat("HeadLookX", horizontal);
            animator.SetFloat("HeadLookY", vertical);
        }
    }

}
