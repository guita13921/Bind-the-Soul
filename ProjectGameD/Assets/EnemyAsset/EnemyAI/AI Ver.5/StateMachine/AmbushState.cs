using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

namespace SG
{

    public class AmbushState : State
    {
        public bool isSleep = true;
        public float detectionRadius = 2;
        public string sleepAnimation;
        public string wakeAnimation;

        public LayerMask detectionLayer;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (isSleep && enemyManager.isInterActing == false)
            {
                enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
            }

            #region  Handle Target Detection

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterStats characterStat = colliders[i].GetComponent<CharacterStats>();

                if (characterStat != null)
                {
                    Vector3 targetDirection = characterStat.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle
                        && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.curretTarget = characterStat;
                        isSleep = false;
                        enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }

            #endregion

            #region handle State Cahnge

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