using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace SG
{

    public class EnemyStat : CharacterStats
    {
        private EnemyStat enemyStats;
        public EnemyRoomManager roomManager;

        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyBossManager enemyBossManager;
        [SerializeField] DemonBossManager demonBossManager; //Only old Demon Boss
        public UIEnemyHealthBar enemyHealthBar;

        EnemySoundManager enemySoundManager;

        public int goldAwardOnDeath = 10;
        public bool isBoss;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStat>();
            roomManager = GetComponentInParent<EnemyRoomManager>();
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyBossManager = GetComponent<EnemyBossManager>();
            demonBossManager = GetComponent<DemonBossManager>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar = GetComponentInChildren<UIEnemyHealthBar>();
            enemySoundManager = GetComponent<EnemySoundManager>();
        }

        void Start()
        {
            if (!isBoss)
            {
                maxHealth = SetMaxHealthFromHealthLevel();
            }
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            if (enemyHealthBar != null) enemyHealthBar.SetMaxHealth(maxHealth);

            return maxHealth;
        }

        public void TakeDamageNoAnimation(int damage)
        {
            currentHealth -= damage;

            if (!isBoss)
            {
                enemyHealthBar.SetHealth(currentHealth);
            }
            else if (isBoss && enemyBossManager != null)
            {
                enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            }

            else if (isBoss && demonBossManager != null) //Only For Demon Boss
            {
                demonBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            }


            if (isBoss && enemyBossManager != null)

                if (currentHealth <= 0)
                {
                    enemyHealthBar.SetHealth(0);
                    isDead = true;
                }
        }

        public void TakeDamage(int damage, string damageAinmation = "Damage01")
        {

            if (enemyManager.isPhaseShifting)
            {
                TakeDamageNoAnimation(damage);
                if (enemySoundManager != null) enemySoundManager.PlayHitSound();
            }
            else
            {
                currentHealth -= damage;
                if (currentHealth > 0)
                {

                    if (damageAinmation == "Block_Guard")
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, false);
                        if (enemySoundManager != null) enemySoundManager.PlayShieldHitSounds();
                    }
                    else
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, true);
                        if (enemySoundManager != null) enemySoundManager.PlayHitSound();
                    }
                }
                else
                {
                    if (isDead) return;
                    enemyHealthBar.SetHealth(0);
                    HandleDeath();
                }
            }

            if (!isBoss)
            {
                enemyHealthBar.SetHealth(currentHealth);
            }
            else if (isBoss && enemyBossManager != null)
            {
                enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            }
        }

        private IEnumerator DestroyAfterDelay()
        {
            float delay = 2.5f; // Adjust based on animation/sound length
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

        private void HandleDeath()
        {
            currentHealth = 0;
            enemyAnimatorManager.PlayTargetAnimation("Dead01", true);

            // Play death sound
            if (enemySoundManager != null)
            {
                enemySoundManager.PlayHitSound();
                enemySoundManager.PlayDeathSound();
            }

            if (roomManager != null)
                roomManager.OnEnemyDefeated(this);

            isDead = true;

            StartCoroutine(DestroyAfterDelay());
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void RestoreFullHealth()
        {
            currentHealth = maxHealth;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }

}