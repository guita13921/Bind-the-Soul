using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Titan/Echo of the Anvilborn")]
    public class EchoAnvilborn : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoAnvilborn = true;
            playerData.echoAnvilbornLevel = powerUpLevel;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive effect — handled in attack/damage system
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoAnvilbornLevel = powerUpLevel;
        }
    }

}