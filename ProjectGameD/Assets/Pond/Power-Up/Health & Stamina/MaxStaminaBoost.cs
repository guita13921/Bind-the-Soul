using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/MaxStaminaBoost")]
    public class MaxStaminaBoost : PowerUp
    {
        [SerializeField] public int staminaIncreaseAmount;
        [SerializeField] public int staminaLevelIncreaseAmount;


        public override void Apply(PlayerData playerData)
        {
            playerData.maxStamina += staminaIncreaseAmount;
            playerData.currentStamina = Mathf.Min(playerData.currentStamina + staminaIncreaseAmount, playerData.maxStamina);
            playerData.staminaLevel += staminaLevelIncreaseAmount;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.maxStamina += staminaIncreaseAmount;
            playerStats.currentStamina = Mathf.Min(playerStats.currentStamina + staminaIncreaseAmount, playerStats.maxStamina);
            playerStats.staminaLevel += staminaLevelIncreaseAmount;
            playerStats.staminaLevel += staminaLevelIncreaseAmount;

            playerStats.staminaBar.SetMaxStamina(playerStats.maxHealth);
            playerStats.staminaBar.SetcurrentStamina(playerStats.currentHealth);
        }
    }
}