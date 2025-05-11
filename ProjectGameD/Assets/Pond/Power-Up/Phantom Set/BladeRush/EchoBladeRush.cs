using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Phantom/Echo of Blade Rush")]
    public class EchoBladeRush : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoBladeRush = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // No subscription needed – trig
            // gered on weapon switch
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoBladeRushLevel = powerUpLevel;
        }
    }

}