using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        EnemyManager enemyManager;
        EnemyBossManager enemyBossManager;
        EnemyStat enemyStat;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            enemyManager = GetComponentInParent<EnemyManager>();
            enemyBossManager = GetComponentInParent<EnemyBossManager>();
            enemyStat = GetComponentInParent<EnemyStat>();
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidBody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidBody.velocity = velocity;

            if (enemyManager.isRotatingWithRootMotion)
            {
                enemyManager.transform.rotation *= animator.deltaRotation;
            }
        }

        public void InstanctiateBossParticleFX()
        {
            GameObject phaseFX = Instantiate(enemyBossManager.particleFX, enemyManager.transform);
        }
    }
}
