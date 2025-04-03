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
        EnemyEffectManager enemyEffectManager;
        EnemyWeaponSlotManager enemyWeaponSlotManager;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            enemyManager = GetComponentInParent<EnemyManager>();
            enemyBossManager = GetComponentInParent<EnemyBossManager>();
            enemyStat = GetComponentInParent<EnemyStat>();
            enemyEffectManager = GetComponentInParent<EnemyEffectManager>();
            enemyWeaponSlotManager = GetComponentInParent<EnemyWeaponSlotManager>();
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

        public void PlayWeaponTrailFX()
        {
            enemyEffectManager.PlayWeaponFX(false);
        }

        public void InstanctiateBossParticleFX()
        {
            GameObject phaseFX = Instantiate(enemyBossManager.particleFX, enemyManager.transform);
        }
    }
}
