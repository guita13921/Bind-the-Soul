using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Titan/Echo of the Iron Maw")]
    public class EchoIronMaw : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoIronMaw = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive, no event subscription needed
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoIronMawLevel = powerUpLevel;
        }
    }
}
