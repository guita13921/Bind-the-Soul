using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/FlatDamageBoost")]
    public class FlatDamageBoostPowerUp : PowerUp
    {
        public int flatDamageBonus;

        public override void Apply(PlayerData playerData)
        {
            playerData.flatDamageBonus += flatDamageBonus;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.flatDamageBonus += flatDamageBonus;
        }
    }
}