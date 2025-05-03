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

        [Header("A.I Setting")]
        public float detectionRadius = 20f;
        //The Higher, and lower
        public float minimumDetectionAngle;
        public float maximumDetectionAngle;
        public float currentRecoveryTime = 0;
        public float currentStunningTime;
        public float stunningTime = 5f;
        public float rotationSpeed = 15f;
        public float maximumAttackRange = 2f;

        [Header("A.I Combat Setting")]
        public bool allowAiToPerformCombo;
        public float comboLikelyHood;
        public bool isPhaseShifting;
        public bool isStunning;

        [Header("Spawn Settings")]
        [SerializeField] private GameObject spawnVFXPrefab; // 👈 Assign your spawn VFX prefab here


        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStat = GetComponent<EnemyStat>();
            enemyRigidBody = GetComponent<Rigidbody>();
            backStabCollider = GetComponentInChildren<BackStabCollider>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();

            navMeshAgent.enabled = false; // 👈 Keep enemy inactive at first
        }

        private void Start()
        {
            enemyRigidBody.isKinematic = false;

            // 👇 Start the delayed activation coroutin
            StartCoroutine(DelayedActivateEnemy());
        }

        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();
            HandleStunningTimer();

            isPhaseShifting = enemyAnimationManager.animator.GetBool("isPhaseShifting");
            isRotatingWithRootMotion = enemyAnimationManager.animator.GetBool("isRotatingWithRootMotion");
            isInterActing = enemyAnimationManager.animator.GetBool("isInteracting");
            isInvulnerable = enemyAnimationManager.animator.GetBool("IsInvulnerable");
            isFiringSpell = enemyAnimationManager.animator.GetBool("isFiringSpell");
            canRotate = enemyAnimationManager.animator.GetBool("canRotate");
            CanDoCombo = enemyAnimationManager.animator.GetBool("canDoCombo");

            enemyAnimationManager.animator.SetBool("isDead", enemyStat.isDead);
            enemyAnimationManager.animator.SetBool("IsBlocking", isBlocking);
            enemyAnimationManager.animator.SetBool("isStunning", isStunning);
        }

        private void LateUpdate()
        {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        private IEnumerator DelayedActivateEnemy()
        {
            // Disable detection during spawn delay
            detectionRadius = 0f;

            // Play VFX immediately
            if (spawnVFXPrefab != null)
            {
                Instantiate(spawnVFXPrefab, transform.position, Quaternion.identity);
            }

            // Wait before activating enemy
            yield return new WaitForSeconds(2f);

            // Enable enemy AI components
            navMeshAgent.enabled = true;

            // Restore detection radius
            detectionRadius = 20f;
        }

        private void HandleStateMachine()
        {
            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStat, enemyAnimationManager);

                if (nextState != null && !enemyStat.isDead)
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

        private void HandleStunningTimer()
        {
            if (currentStunningTime > 0)
            {
                currentStunningTime -= Time.deltaTime;
            }
            else
            {
                isStunning = false;
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