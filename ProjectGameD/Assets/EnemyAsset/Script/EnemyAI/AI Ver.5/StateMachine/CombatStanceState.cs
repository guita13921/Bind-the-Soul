using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CombatStanceState : State
    {

        public AttackState attackState;
        public EnemyAttackAction[] enemyAttacks;
        public PursueTargetState pursueTargetState;

        protected bool randomDestinationSet = false;
        protected float vertcalMovementValue = 0;
        protected float HorizontalMovementValue = 0;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager)
        {
            //Check for attack range
            //potentially circle player or walk around them
            //if in attack range return Attack State
            //if we are in a cool down after attack, return this state and continue circling player
            //if the player runs out of range return the pursuetarget state
            float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, enemyManager.transform.position);
            enemyAnimatorManager.animator.SetFloat("Vertical", vertcalMovementValue, 0.2f, Time.deltaTime);
            enemyAnimatorManager.animator.SetFloat("Horizontal", HorizontalMovementValue, 0.2f, Time.deltaTime);

            if (enemyManager.isInterActing)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0);
                enemyAnimatorManager.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if (enemyManager.hasShield && enemyManager.isBlocking == false)
            {
                enemyAnimatorManager.PlayTargetAnimation("StartBlock", false);
                enemyManager.isBlocking = true;
            }

            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                return pursueTargetState;
            }

            /*
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemyAnimatorManager);
            }
            */


            HandleRotationToTarget(enemyManager);

            if (enemyManager.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                randomDestinationSet = false;
                attackState.hasPerformAttack = false;
                return attackState;
            }
            else
            {
                GetNewAttack(enemyManager);
            }

            return this;
        }

        protected void HandleRotationToTarget(EnemyManager enemyManager)
        {
            //Rotate manually
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.curretTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.curretTarget.transform.position);
                enemyManager.enemyRigidBody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }

        protected void DecideCirclingAction(EnemyAnimatorManager enemyAnimatorManager)
        {
            //Circle with only forward vertical movement
            //Circle with running
            WalkAroundTarget(enemyAnimatorManager);
        }

        protected void WalkAroundTarget(EnemyAnimatorManager enemyAnimatorManager)
        {
            vertcalMovementValue = 0.5f;
            HorizontalMovementValue = vertcalMovementValue = Random.Range(-1, 1);

            if (vertcalMovementValue <= 1 && vertcalMovementValue >= 0)
            {
                vertcalMovementValue = 0.5f;
            }
            else if (vertcalMovementValue >= -1 && vertcalMovementValue < 0)
            {
                vertcalMovementValue = -0.5f;
            }
        }

        protected virtual void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection = enemyManager.curretTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, transform.position);

            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
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

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                    && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (attackState.currentAttack != null) return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > rendomValue)
                        {
                            attackState.currentAttack = enemyAttackAction;
                        }
                    }
                }

            }
        }
    }

}