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
        public PlayerManager playerManager;

        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyBossManager enemyBossManager;
        [SerializeField] DemonBossManager demonBossManager; //Only old Demon Boss
        public UIEnemyHealthBar enemyHealthBar;
        EnemyDebuff enemyDebuff;

        CharacterSoundFXManager characterSoundFXManager;

        public int goldAwardOnDeath = 10;
        public bool isBoss;

        private void Awake()
        {
            playerManager = FindAnyObjectByType<PlayerManager>();
            enemyStats = GetComponent<EnemyStat>();
            roomManager = GetComponentInParent<EnemyRoomManager>();
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyBossManager = GetComponent<EnemyBossManager>();
            demonBossManager = GetComponent<DemonBossManager>();
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar = GetComponentInChildren<UIEnemyHealthBar>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            enemyDebuff = GetComponent<EnemyDebuff>();
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

            if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomDamageSoundFX();
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                isDead = true;
                HandleDeathWithOutAniamtion();
                playerManager.playerStats.OnEnemyKilled?.Invoke();
                currentHealth = 0;
            }

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
        }

        public void TakeDamage(int damage, string damageAinmation = "Damage01")
        {

            if (enemyManager.isPhaseShifting)
            {
                if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomDamageSoundFX();
                TakeDamageNoAnimation(damage);
            }
            else
            {
                currentHealth -= damage;
                if (currentHealth > 0)
                {

                    if (damageAinmation == "Block_Guard")
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, false);
                        if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomShielHitSoundFX();
                    }
                    else
                    {
                        enemyAnimatorManager.PlayTargetAnimation(damageAinmation, true);
                        if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomDamageSoundFX();
                    }
                }
                else
                {
                    if (isDead) return;
                    enemyHealthBar.SetHealth(0);
                    HandleDeath();
                    playerManager.playerStats.OnEnemyKilled?.Invoke();
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
            DropItem();

            // Play death sound
            if (characterSoundFXManager != null)
            {
                characterSoundFXManager.PlayRandomDamageSoundFX();
                characterSoundFXManager.PlayDeathSound();
            }

            if (roomManager != null)
                roomManager.OnEnemyDefeated(this);

            isDead = true;

            if (enemyDebuff.stuckKnife != null && enemyDebuff.stuckKnife.ownerAttack != null)
            {
                enemyDebuff.stuckKnife.ownerAttack.RecoverKnifeCharge();
            }
            StartCoroutine(DestroyAfterDelay());
        }

        private void HandleDeathWithOutAniamtion()
        {
            currentHealth = 0;
            DropItem();

            // Play death sound
            if (characterSoundFXManager != null)
            {
                characterSoundFXManager.PlayRandomDamageSoundFX();
                characterSoundFXManager.PlayDeathSound();
            }

            if (roomManager != null)
                roomManager.OnEnemyDefeated(this);

            isDead = true;

            if (enemyDebuff.stuckKnife != null && enemyDebuff.stuckKnife.ownerAttack != null)
            {
                enemyDebuff.stuckKnife.ownerAttack.RecoverKnifeCharge();
            }
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

        public void DropItem()
        {
            foreach (Drop drop in PlayerManager.availableDrops)
            {
                //Debug.Log(drop);
                int limiter = 0;
                while (Random.value < drop.DropChance && limiter <= 3)
                {
                    limiter++;
                    GameObject droppedItem = Instantiate(drop.Prefab,
                    this.transform.position, Quaternion.identity);

                    //droppedItem.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.1f, 0.1f),
                    // Random.Range(30, 50),
                    //Random.Range(-1, 1)), ForceMode.Impulse);
                }
            }
        }
    }

}