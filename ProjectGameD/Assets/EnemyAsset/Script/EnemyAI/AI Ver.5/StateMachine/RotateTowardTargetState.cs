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
            float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up) + (-45);

            if (enemyManager.isInterActing)
            {
                return this;
            }

            if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInterActing)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behide", true);
                return combatStanceState;
            }
            else if (viewableAngle <= -101 && viewableAngle >= -180 && !enemyManager.isInterActing)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Behide", true);
                return combatStanceState;
            }
            else if (viewableAngle <= -45 && viewableAngle <= -100 && !enemyManager.isInterActing)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Right", true);
                return combatStanceState;
            }
            else if (viewableAngle >= 45 && viewableAngle <= 100 && !enemyManager.isInterActing)
            {
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn Left", true);
                return combatStanceState;
            }

            return combatStanceState;
        }
    }
}