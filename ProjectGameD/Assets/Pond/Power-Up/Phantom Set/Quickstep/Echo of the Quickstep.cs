using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Phantom/Echo of the Quickstep")]
    public class EchoQuickstep : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoQuickstep = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive: trigger in combo finisher logic
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoQuickstepLevel = powerUpLevel;
        }
    }

}