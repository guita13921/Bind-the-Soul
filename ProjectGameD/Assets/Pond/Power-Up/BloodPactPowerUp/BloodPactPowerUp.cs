using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/BloodPactPowerUp")]
    public class BloodPactPowerUp : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.bloodPactDamageModify = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.playerData.bloodPactDamageModify = true;
        }

        public void OnRoomComplete(PlayerStats playerStats)
        {
            if (playerStats.currentHealth <= 1)
            {
                playerStats.currentHealth = 1;
            }
            else
            {
                playerStats.currentHealth -= 1;
            }

            playerStats.healthBar.SetCurrentHealth(playerStats.currentHealth);

            Debug.Log("Blood Pact applied: -1 HP after room. Current HP: " + playerStats.currentHealth);

        }
    }
}