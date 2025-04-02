using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class EnemyManager : CharacterManager
    {
        [SerializeField] EnemyLocomotionManager enemyLocomotionManager;
        [SerializeField] EnemyAnimatorManager enemyAnimationManager;
        [SerializeField] EnemyWeaponSlotManager enemyWeaponSlotManager;
        EnemyStat enemyStat;

        [Header("State")]
        public State currentState;
        public CharacterStats curretTarget;
        public NavMeshAgent navMeshAgent;
        public Rigidbody enemyRigidBody;

        [Header("Enemy Flags")]
        public bool isPerformingAction;
        public bool isInterActing;

        public bool hasShield;
        public float rotationSpeed = 15f;
        public float maximumAttackRange = 2f;

        [Header("A.I Setting")]
        public float detectionRadius = 20f;
        //The Higher, and lower
        public float minimumDetectionAngle;
        public float maximumDetectionAngle;
        public float currentRecoveryTime = 0;

        [Header("A.I Combat Setting")]
        public bool allowAiToPerformCombo;
        public float comboLikelyHood;
        public bool isPhaseShifting;

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStat = GetComponent<EnemyStat>();
            enemyRigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
            navMeshAgent.enabled = false;
        }

        private void Start()
        {
            hasShield = enemyWeaponSlotManager.LoadShield();
            enemyRigidBody.isKinematic = false;
        }

        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();

            isPhaseShifting = enemyAnimationManager.animator.GetBool("isPhaseShifting");
            isRotatingWithRootMotion = enemyAnimationManager.animator.GetBool("isRotatingWithRootMotion");
            isInterActing = enemyAnimationManager.animator.GetBool("isInteracting");
            isInvulnerable = enemyAnimationManager.animator.GetBool("isInvulnerable");
            canRotate = enemyAnimationManager.animator.GetBool("canRotate");
            CanDoCombo = enemyAnimationManager.animator.GetBool("canDoCombo");
            enemyAnimationManager.animator.SetBool("isDead", enemyStat.isDead);
            enemyAnimationManager.animator.SetBool("isBlocking", isBlocking);

        }

        private void LateUpdate()
        {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        private void HandleStateMachine()
        {
            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStat, enemyAnimationManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }

        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
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