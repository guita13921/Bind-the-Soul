using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Predator/Echo of the Apex Drive")]
    public class EchoApexDrive : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoApexDrive = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive — checked at the moment of damage
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoApexDriveLevel = powerUpLevel;
        }
    }

}