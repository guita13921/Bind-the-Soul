using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class IdleState : State
    {
        [SerializeField] public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager)
        {
            #region Handle Enemy Target Detection
            //Look for a potential target
            //Switch to the pursue target state if target is found
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStat characterStat = colliders[i].transform.GetComponent<CharacterStat>();

                if (characterStat != null)
                {
                    //Check for team id
                    Vector3 targetDirection = characterStat.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.curretTarget = characterStat;
                    }
                }
            }

            #endregion

            #region Handle Switch State To Next State
            if (enemyManager.curretTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
            #endregion
        }
    }
}