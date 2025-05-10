using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of the Crimson Edge")]
    public class EchoCrimsonEdge : PowerUp
    {
        private PlayerStats boundStats; // Store the reference to use in callbacks

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoCrimsonEdge = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            boundStats = playerStats;
            playerStats.OnParrySuccess += RestoreHealth;
        }

        private void RestoreHealth()
        {
            if (boundStats == null) return;

            int healAmount = 5 * powerUpLevel;
            boundStats.RestoreHealth(healAmount);
            Debug.Log($"Echo of the Crimson Edge: Restored {healAmount} HP after parry (Level {powerUpLevel}).");
        }

        public override void OnStacked(PlayerData playerData)
        {
            playerData.echoCrimsonEdgeLevel = powerUpLevel;
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
        }

        // Optional: clean up if the power-up is removable
        public void Remove()
        {
            if (boundStats != null)
                boundStats.OnParrySuccess -= RestoreHealth;

            boundStats = null;
        }



    }

}
