using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerStat : CharacterStat
    {
        void Start()
        {
            InitializeHealth();
        }

        void InitializeHealth()
        {
            maxHealth = healthLevel * 50; // Example: Each level adds 50 health
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Enemy died!");
            Destroy(gameObject); // Example behavior when the enemy dies
        }
    }

}