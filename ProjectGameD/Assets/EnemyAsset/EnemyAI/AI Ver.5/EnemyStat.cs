using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyStat : CharacterStats
    {

        EnemyAnimatorManager enemyAnimatorManager;
        public UIEnemyHealthBar enemyHealthBar;
        public int coinAwardOnDeath = 10;

        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamageNoAnimation(int damage)
        {
            currentHealth -= damage;
            enemyHealthBar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }
        }

        public void TakeDamage(int damage, string damageAinmation = "Damage01")
        {
            currentHealth -= damage;
            enemyHealthBar.SetHealth(currentHealth);
            //Debug.Log(currentHealth);

            if (currentHealth >= 0)
            {
                enemyAnimatorManager.PlayTargetAnimation(damageAinmation, true);
            }
        }


    }

}