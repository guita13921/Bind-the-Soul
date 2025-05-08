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
            enemyWeaponSlotManager = GetComponent<EnemyWeaponSlotManager>();
        }

        public void TakeCriticalDamageAnimationEvent()
        {
            enemyStat.TakeDamageNoAnimation(enemyManager.pendingCriticalDamage);
            enemyManager.pendingCriticalDamage = 0;
        }

        public void DisableCanBeRiposted()
        {
            enemyManager.canBeRiposted = false;
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;

            if (delta <= 0f) return; // Prevent NaN

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

        public void AwardGoldOnDeath()
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            GoldContBar goldContBar = FindObjectOfType<GoldContBar>();

            if (playerStats != null)
            {
                playerStats.AddGold(enemyStat.goldAwardOnDeath);

                if (goldContBar != null)
                {
                    goldContBar.SetGoldCountText(playerStats.goldCount);
                }
            }
        }
    }
}
