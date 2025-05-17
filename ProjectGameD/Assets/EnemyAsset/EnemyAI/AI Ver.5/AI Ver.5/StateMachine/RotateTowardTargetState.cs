using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class RotateTowardTargetState : State
    {

        public CombatStanceState combatStanceState;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0);
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0);

            Vector3 targetDirection = enemyManager.curretTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

            if (!enemyManager.isStunning && enemyManager.canRotate) RotateTowardTargetWhileAttacking(enemyManager);


            if (enemyManager.isInterActing || enemyManager.isStunning)
            {
                return this;
            }

            if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInterActing && !enemyManager.isAttacking)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behide", true);
                return combatStanceState;
            }
            else if (viewableAngle <= -101 && viewableAngle >= -180 && !enemyManager.isInterActing && !enemyManager.isAttacking)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behide", true);
                return combatStanceState;
            }
            else if (viewableAngle <= -45 && viewableAngle <= -100 && !enemyManager.isInterActing && !enemyManager.isAttacking)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Right", true);
                return combatStanceState;
            }
            else if (viewableAngle >= 45 && viewableAngle <= 100 && !enemyManager.isInterActing && !enemyManager.isAttacking)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Left", true);
                return combatStanceState;
            }

            return combatStanceState;
        }

        private void RotateTowardTargetWhileAttacking(EnemyManager enemyManager)
        {
            //Rotate manually
            if (enemyManager.canRotate && enemyManager.isInterActing)
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
        }

    }
}