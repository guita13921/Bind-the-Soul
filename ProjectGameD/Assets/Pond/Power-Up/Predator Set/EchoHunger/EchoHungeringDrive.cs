using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Predator/Echo of Hungering Drive")]
    public class EchoHungeringDrive : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoHungeringDrive = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive — handled during special attack logic
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoHungeringDriveLevel = powerUpLevel;
        }
    }
}