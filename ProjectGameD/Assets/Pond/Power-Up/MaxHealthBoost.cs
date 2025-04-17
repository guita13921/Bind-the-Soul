using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/MaxHealthBoost")]
    public class MaxHealthBoost : PowerUp
    {
        public int healthIncreaseAmount = 20;
        public int healthLevelIncreaseAmount = 2;

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
        }
    }
}