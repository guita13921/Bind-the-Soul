using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/MomentumPowerUp")]
    public class MomentumPowerUp : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.hasMomentum = true;
            playerData.momentumActive = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.playerData.hasMomentum = true;
            playerStats.playerData.momentumActive = true;
        }

        public void OnRoomEnter(PlayerData playerData)
        {
            if (playerData.hasMomentum)
            {
                playerData.momentumActive = true;
            }
        }

        public void OnPlayerDamaged(PlayerData playerData)
        {
            if (playerData.hasMomentum)
            {
                playerData.momentumActive = false;
            }
        }

        public void OnRoomClear(PlayerData playerData)
        {
            if (playerData.hasMomentum)
            {
                playerData.momentumActive = true;
            }
        }
    }
}