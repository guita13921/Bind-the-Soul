using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class BossCombatStanceState : CombatStanceState
    {
        [Header("Second Phase Attacks")]
        public bool hasPhaseShifted;
        public EnemyAttackAction[] secondPhaseAttacks;

        protected override void GetNewAttack(EnemyManager enemyManager)
        {
            if (hasPhaseShifted)
            {
                Vector3 targetDirection = enemyManager.curretTarget.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                float distanceFromTarget = Vector3.Distance(enemyManager.curretTarget.transform.position, transform.position);

                int maxScore = 0;

                for (int i = 0; i < secondPhaseAttacks.Length; i++)
                {
                    EnemyAttackAction enemyAttackAction = secondPhaseAttacks[i];

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

                for (int i = 0; i < secondPhaseAttacks.Length; i++)
                {
                    EnemyAttackAction enemyAttackAction = secondPhaseAttacks[i];

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
            else
            {

            }

            base.GetNewAttack(enemyManager);
        }

    }
}