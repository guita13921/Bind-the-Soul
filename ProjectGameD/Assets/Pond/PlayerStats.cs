using System;
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
        public AnimatorHander animatorHander;
        public PlayerLocomotion playerLocomotion;
        public PlayerInventory playerInventory;

        PowerUpManager powerUpManager;
        public PlayerManager playerManager;
        CharacterSoundFXManager characterSoundFXManager;
        public HealthBar healthBar;
        public StaminaBar staminaBar;

        public float StaminaRegenerationAmount;
        public int staminaLevel;
        public float maxStamina;
        public float currentStamina;
        public float staminaRegenTimer = 0;
        float extraRegenPerSecond;

        public int flatDamageBonus;
        public float StaminaRegenBonus;

        public Action OnParrySuccess;

        private void Awake()
        {
            powerUpManager = GetComponent<PowerUpManager>();
            playerManager = FindObjectOfType<PlayerManager>();
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHander = GetComponentInChildren<AnimatorHander>();
            characterSoundFXManager = GetComponentInParent<CharacterSoundFXManager>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerInventory = GetComponent<PlayerInventory>();
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
            animatorHander.PlayTargetAnimation(damageAnimation, true, false, 3.0f);

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                characterSoundFXManager.PlayDeathSound();

                animatorHander.PlayTargetAnimation("Dead_01", true);
                isDead = true;
                HandledeathLocomotion();
            }
            else
            {
                if (damageAnimation == "Block Guard")
                {
                    animatorHander.PlayTargetAnimation(damageAnimation, false);
                    if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomShielHitSoundFX();
                }
                else
                {
                    animatorHander.PlayTargetAnimation(damageAnimation, true);
                    if (characterSoundFXManager != null) characterSoundFXManager.PlayRandomDamageSoundFX();
                }
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            staminaBar.SetcurrentStamina(Mathf.RoundToInt(currentStamina));
        }

        public void RegenerateStamina()
        {
            if (playerManager.isInteracting || playerManager.isSprinting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                if (currentStamina < maxStamina && staminaRegenTimer > 2f)
                {

                    if (playerData.echoResoluteMind && playerInventory.leftWeapon.weaponType == WeaponType.Shield)
                    {
                        extraRegenPerSecond = GetEchoResoluteMindBonus();
                    }

                    currentStamina += (StaminaRegenerationAmount + StaminaRegenBonus + extraRegenPerSecond) * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);

                    staminaBar.SetcurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }

        }


        public void RestoreHealth(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"Player healed for {amount}. Current Health: {currentHealth}");
            healthBar.SetCurrentHealth(currentHealth);
        }

        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            Debug.Log($"Stamina restored by {amount}, now at {currentStamina}/{maxStamina}");
            staminaBar.SetcurrentStamina((int)currentStamina);
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

        public void HandledeathLocomotion()
        {
            animatorHander.anim.SetFloat("Vertical", 0);
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

        // Call this when your parry system determines a successful parry
        public void TriggerParrySuccess()
        {
            Debug.Log("Parry was successful!");
            OnParrySuccess?.Invoke(); // This safely triggers all subscribed listeners
        }

        // Example method to simulate a parry (could be triggered via animation event, etc.)
        public void SimulateParry()
        {
            // Logic to check if timing was correct...
            TriggerParrySuccess(); // Notify all systems
        }

        private float GetEchoResoluteMindBonus()
        {
            return playerData.echoResoluteMindLevel switch
            {
                1 => 2f,
                2 => 3f,
                3 => 4f,
                4 => 5f,
                _ => 0f
            };
        }
    }
}
