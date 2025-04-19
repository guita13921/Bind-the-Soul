using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Heal Current Health")]
    public class HealCurrentHealth : PowerUp
    {
        public int healAmount = 25;

        public override void Apply(PlayerData playerData)
        {
            playerData.currentHealth = Mathf.Min(playerData.currentHealth + healAmount, playerData.maxHealth);
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.currentHealth = Mathf.Min(playerStats.currentHealth + healAmount, playerStats.maxHealth);

            if (playerStats.healthBar != null)
            {
                playerStats.healthBar.SetMaxHealth(playerStats.maxHealth);
                playerStats.healthBar.SetCurrentHealth(playerStats.currentHealth);
            }
        }
    }
}
