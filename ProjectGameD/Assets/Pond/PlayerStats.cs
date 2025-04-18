using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class PlayerStats : CharacterStats
    {

        [Header("Player Data")]
        public PlayerData playerData;

        AnimatorHander animatorHander;
        PowerUpManager powerUpManager;
        PlayerManager playerManager;
        public HealthBar healthBar;
        public StaminaBar staminaBar;

        public float StaminaRegenerationAmount;
        public int staminaLevel;
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenTimer = 0;

        public int flatDamageBonus;
        public float StaminaRegenBonus;

        private void Awake()
        {
            powerUpManager = GetComponent<PowerUpManager>();
            playerManager = FindObjectOfType<PlayerManager>();
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHander = GetComponentInChildren<AnimatorHander>();
        }

        void Start()
        {
            LoadStatsFromPlayerData();
            maxHealth = SetMaxHealthFromHealthLevel();

            //currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);


            maxStamina = SetMaxStaminaFromStaminaLevel();
            //currentStamina = maxStamina;
            staminaBar.SetMaxStamina(Mathf.RoundToInt(maxStamina));
            staminaBar.SetcurrentStamina(Mathf.RoundToInt(currentStamina));
        }

        private void LoadStatsFromPlayerData()
        {
            healthLevel = playerData.healthLevel;
            staminaLevel = playerData.staminaLevel;
            currentHealth = playerData.currentHealth; // Load current from PlayerData
            currentStamina = playerData.currentStamina; // Load current from PlayerData
            goldCount = playerData.goldCount;
            flatDamageBonus = playerData.flatDamageBonus;
            StaminaRegenBonus = playerData.StaminaRegenBonus;


        }

        // ✅ คืนค่าโดยตรง ไม่ต้องเซ็ตค่าให้ maxHealth หรือ maxStamina ก่อน
        private int SetMaxHealthFromHealthLevel()
        {
            return healthLevel * 10;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            return staminaLevel * 10;
        }

        public void TakeDamage(int damage, string damageAnimation = "Damage_01") // ✅ แก้ชื่อให้ตรง
        {
            CheckDamagePowerUp();
            if (playerManager.isInvulnerable)
                return;
            if (isDead)
                return;
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            animatorHander.PlayTargetAnimation(damageAnimation, true);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHander.PlayTargetAnimation("Dead_01", true);
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            staminaBar.SetcurrentStamina(Mathf.RoundToInt(currentStamina));
        }

        public void RegenerateStamina()
        {
            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                if (currentStamina < maxStamina && staminaRegenTimer > 1f)
                {
                    currentStamina += (StaminaRegenerationAmount + StaminaRegenBonus) * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);

                    staminaBar.SetcurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }

        }

        public void AddGold(int golds)
        {
            goldCount += golds;
        }

        private void OnDestroy()
        {
            if (playerData != null)
            {
                playerData.currentHealth = currentHealth;
                playerData.currentStamina = currentStamina;
            }
        }

        private void CheckDamagePowerUp()
        {
            var Momentum = powerUpManager.collectedPowerUps.OfType<MomentumPowerUp>().FirstOrDefault();
            if (Momentum != null)
            {
                Momentum.OnPlayerDamaged(playerData);
                Debug.Log("OnRoomClear");
            }
        }
    }
}
