using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] EnemyLocomotionManager enemyLocomotionManager;
        [SerializeField] EnemyAnimatorManager enemyAnimationManager;
        EnemyStat enemyStat;

        public State currentState;
        public CharacterStat curretTarget;

        public bool isPerformingAction;

        [Header("A.I Setting")]
        public float detectionRadius = 20f;
        //The Higher, and lower
        public float minimumDetectionAngle;
        public float maximumDetectionAngle;

        public float currentRecoveryTime = 0;

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStat = GetComponent<EnemyStat>();
        }

        private void Update()
        {
            HandleRecoveryTimer();
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
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

        #region  Attacks

        private void AttackTarget()
        {
            /*
            if (isPerformingAction)
            {
                return;
            }

            if (currentAttack == null)
            {
                GetNewAttack();
            }
            else
            {
                isPerformingAction = true;
                currentRecoveryTime = currentAttack.recoveryTime;
                enemyAnimationManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                currentAttack = null;
            }
            */
        }

        private void GetNewAttack()
        {
            /*
            Vector3 targetDirection = enemyLocomotionManager.curretTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.curretTarget.transform.position, transform.position);

            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                    && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int rendomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                    && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (currentAttack != null) return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > rendomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }

            }
            */

        }

        #endregion

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