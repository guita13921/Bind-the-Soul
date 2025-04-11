using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace SG
{

    public class EnemyStat : CharacterStats
    {

        private EnemyStat enemyStats;
        private EnemyRoomManager roomManager;

        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyBossManager enemyBossManager;
        public UIEnemyHealthBar enemyHealthBar;
        public int coinAwardOnDeath = 10;

        public bool isBoss;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStat>();
            roomManager = GetComponentInParent<EnemyRoomManager>();
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyBossManager = GetComponent<EnemyBossManager>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
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
            enemyHealthBar.SetMaxHealth(maxHealth);

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


            if (isBoss && enemyBossManager != null)

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    isDead = true;
                }
        }

        public override void TakeDamage(int damage, string damageAinmation = "Damage01")
        {
            if (enemyManager.isPhaseShifting)
            {
                TakeDamageNoAnimation(damage);
            }
            else
            {
                currentHealth -= damage;
                if (currentHealth >= 0)
                {
                    if (damageAinmation == "Block_Guard")
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, false);
                    }
                    else
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, true);
                    }
                }
                else
                {
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

        private void HandleDeath()
        {
            enemyAnimatorManager.PlayTargetAnimation("Dead01", true);
            roomManager?.OnEnemyDefeated(this);
            Destroy(gameObject);
        }

    }

}