using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PursueTargetState : State
    {
        //Chase the target
        //If within attack range, switch to combat stance stage
        //If target is out of range, return this state and continue to chase target\

        public CombatStanceState combatStanceState;
        public RotateTowardTargetState rotateTowardTargetState;


        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager)
        {
            Vector3 targetDirection = enemyManager.curretTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

            if (enemyManager.isStunning) return this;
            HandleRotationToTarget(enemyManager);
            if (enemyManager.isInterActing || enemyManager.isStunning) return this;

            if (enemyManager.hasShield && enemyManager.isBlocking == false)
            {
                enemyAnimatorManager.PlayTargetAnimation("StartBlock", false);
                enemyManager.isBlocking = true;
            }

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }


            if (distanceFromTarget <= enemyManager.maximumAttackRange)
            {
                return combatStanceState;
            }
            else
            {
                return this;
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
