using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Titan/Echo of the Stoneborn")]
    public class EchoStoneborn : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoStoneborn = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // No subscription needed, this is a passive damage buff
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoStonebornLevel = powerUpLevel;
        }
    }

}