using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/StaminaRegenBoost")]
    public class StaminaRegenBoostPowerUp : PowerUp
    {
        public float staminaRegenBoost;

        public override void Apply(PlayerData playerData)
        {
            playerData.StaminaRegenBonus += staminaRegenBoost;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.StaminaRegenBonus += staminaRegenBoost;
        }
    }

}