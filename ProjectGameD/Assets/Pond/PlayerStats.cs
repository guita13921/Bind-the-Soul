using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerStats : CharacterStats
    {

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
            staminaBar.SetcurrentStamina(currentStamina); // ✅ แก้ชื่อฟังก์ชันให้ถูกต้อง
        }

        // ✅ คืนค่าโดยตรง ไม่ต้องเซ็ตค่าให้ maxHealth หรือ maxStamina ก่อน
        private int SetMaxHealthFromHealthLevel()
        {
            return healthLevel * 10;
        }

        private int SetMaxStaminaFromStaminaLevel()
        {
            return staminaLevel * 100;
        }

        public void TakeDamage(int damage, string damageAnimation = "Damage_01") // ✅ แก้ชื่อให้ตรง
        {
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
            staminaBar.SetcurrentStamina(currentStamina); // ✅ แก้ชื่อฟังก์ชันให้ถูกต้อง
        }
    }
}
