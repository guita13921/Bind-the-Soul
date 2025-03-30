using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace SG
{
    public class PlayerStats : CharacterStats
    {
        public int healthlevel = 10;
        public HealthBar healthBar;

        AnimatorHander animatorHander;

        private void Awake()
        {
            animatorHander = GetComponentInChildren<AnimatorHander>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        // เปลี่ยน void เป็น int
        private int SetMaxHealthFromHealthLevel()
        {
            return healthlevel * 10; // คืนค่า maxHealth
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
    }
}
