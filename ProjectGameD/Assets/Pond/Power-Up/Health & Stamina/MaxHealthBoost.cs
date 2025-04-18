using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/MaxHealthBoost")]
    public class MaxHealthBoost : PowerUp
    {
        [SerializeField] public int healthIncreaseAmount;
        [SerializeField] public int healthLevelIncreaseAmount;


        public override void Apply(PlayerData playerData)
        {
            playerData.maxHealth += healthIncreaseAmount;
            playerData.currentHealth = Mathf.Min(playerData.currentHealth + healthIncreaseAmount, playerData.maxHealth);
            playerData.healthLevel += healthLevelIncreaseAmount;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.maxHealth += healthIncreaseAmount;
            playerStats.currentHealth = Mathf.Min(playerStats.currentHealth + healthIncreaseAmount, playerStats.maxHealth);
            playerStats.healthLevel += healthLevelIncreaseAmount;
            playerStats.healthLevel += healthLevelIncreaseAmount;

            playerStats.healthBar.SetMaxHealth(playerStats.maxHealth);
            playerStats.healthBar.SetCurrentHealth(playerStats.currentHealth);
        }
    }
}