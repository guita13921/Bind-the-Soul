using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of the Silver Guard")]
    public class EchoSilverGuard : PowerUp
    {
        private PlayerStats boundStats; // Store the reference to use in callbacks

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoSilverGuard = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            boundStats = playerStats;
        }

        private float GetReduceStaminaDamageModify()
        {

            float ReduceStaminaDamageModify = 0.25f * powerUpLevel;
            Debug.Log($"EchoSilverGuard: reducemodify {ReduceStaminaDamageModify} (Level {powerUpLevel}).");
            return ReduceStaminaDamageModify;
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoSilverGuardLevel = powerUpLevel;
        }

        // Optional: clean up if the power-up is removable
        public void Remove()
        {
            if (boundStats != null)
                //boundStats.OnParrySuccess -= RestoreHealth;

                boundStats = null;
        }
    }
}