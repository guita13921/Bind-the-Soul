using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace SG
{
    public class PlayerStats : CharacterStats
    {
        public int healthlevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public int maxStamina;
        public int currentStamina;
        public HealthBar healthBar;
        public StaminaBar staminaBar;

        AnimatorHander animatorHander;

        private void Awake()
        {
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHander = GetComponentInChildren<AnimatorHander>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetcurrentStamina(currentStamina);

        }

        // เปลี่ยน void เป็น int
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth; // คืนค่า maxHealth
        }
        private int SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 100;
            return maxStamina; // คืนค่า maxHealth
        }

        public void TakeDamage(int damage, string damageAinmation = "Damage_01")
        {
            currentHealth -= damage; // ลดค่าพลังชีวิต
            healthBar.SetCurrentHealth(currentHealth); // อัปเดต Health Bar
            animatorHander.PlayTargetAnimation(damageAinmation, true);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHander.PlayTargetAnimation("Dead_01", true);
            }
        }
        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;
            staminaBar.SetcurrentStamina(currentStamina);

        }
    }
}
