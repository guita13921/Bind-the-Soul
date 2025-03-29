using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AttackState : State
    {

        public CombatStanceState combatStanceState;

        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimator)
        {
            //Select one of our many attacks based on attack scores
            //if the selecteed attack is not able to be used because of bad angle or distance, select a ne attack 
            //if the attack is viable, stop our movement and attack our target
            //set our recovery timer to the attacks recovery time
            // return the combat stance state

            //HandleRotationToTarget(enemyManager);

            Vector3 targetDirection = enemyManager.curretTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            if (enemyManager.isPerformingAction) return combatStanceState;

            if (currentAttack != null)
            {
                //IF we are too close to the enemy to perform current attack, get a new attack

                if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
                {
                    return this;
                }
                else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
                {
                    // if ur enemy  is within our attack viewable angle, we attack
                    if (viewableAngle <= currentAttack.maximumAttackAngle &&
                        viewableAngle >= currentAttack.minimumAttackAngle)
                    {
                        if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                        {
                            enemyAnimator.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                            enemyAnimator.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                            enemyAnimator.PlayTargetAnimation(currentAttack.actionAnimation, true);
                            enemyManager.isPerformingAction = true;
                            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                            currentAttack = null;
                            return combatStanceState;
                        }
                    }
                }
            }
            else
            {
                GetNewAttack(enemyManager);
            }

            return combatStanceState;
        }

        private void GetNewAttack(EnemyManager enemyManager)
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
                        if (currentAttack != null) return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > rendomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }

            }
        }

        private void HandleRotationToTarget(EnemyManager enemyManager)
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
            //Rotate with pathfinding
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
    }
}
