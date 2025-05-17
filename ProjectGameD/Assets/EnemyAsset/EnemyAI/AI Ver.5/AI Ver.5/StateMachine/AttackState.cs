using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AttackState : State
    {

        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackAction currentAttack;
        public RotateTowardTargetState rotateTowardTargetState;

        public bool willDoComboOnNextAttack = false;
        public bool hasPerformAttack = false;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimator)
        {
            //Select one of our many attacks based on attack scores
            //if the selecteed attack is not able to be used because of bad angle or distance, select a ne attack 
            //if the attack is viable, stop our movement and attack our target
            //set our recovery timer to the attacks recovery time
            // return the combat stance state

            float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, enemyManager.transform.position);

            if (!enemyManager.isStunning) RotateTowardTargetWhileAttacking(enemyManager);

            if (willDoComboOnNextAttack)
            {
                AttackTargetWithCombo(enemyAnimator, enemyManager);
            }

            if (enemyManager.isInterActing || enemyManager.isStunning || enemyManager.currentRecoveryTime > 0) return this;

            if (currentAttack == null)
            {
                return combatStanceState;
            }

            if (enemyManager.hasShield && enemyManager.isBlocking == true)
            {
                enemyAnimator.PlayTargetAnimation("EndBlock01", true);
                enemyManager.isBlocking = false;
            }

            if (!hasPerformAttack)
            {
                AttackTarget(enemyAnimator, enemyManager);
                RollForComboChance(enemyManager, enemyAnimator);
            }

            if (distanceFromTarget > enemyManager.maximumAttackRange)
            {
                return pursueTargetState;
            }

            if (willDoComboOnNextAttack)
            {
                return this;
            }

            return rotateTowardTargetState;
        }

        private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager)
        {
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true, currentAttack.canRotate);
            enemyAnimatorManager.animator.SetBool("isAttacking", true);
            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
            hasPerformAttack = true;
            currentAttack = null;
        }

        private void AttackTargetWithCombo(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager)
        {
            willDoComboOnNextAttack = false;
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true, currentAttack.canRotate);
            enemyAnimatorManager.animator.SetBool("isAttacking", true);
            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
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

        private void RollForComboChance(EnemyManager enemyManagers, EnemyAnimatorManager enemyAnimatorManager)
        {

            float comboChance = Random.Range(0, 100);

            if (comboChance > enemyManagers.comboLikelyHood) return;

            if (currentAttack != null && currentAttack.comboAction != null)
            {
                willDoComboOnNextAttack = true;
                currentAttack = currentAttack.comboAction;
            }
            else
            {
                willDoComboOnNextAttack = false;
                currentAttack = null;
            }
        }
    }
}
