using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Predator/Echo of the Bloodhound")]
    public class EchoBloodhound : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoBloodhound = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive effect — evaluated during damage calculation
        }


        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoBloodhoundLevel = powerUpLevel;
        }
    }
}