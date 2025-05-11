using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Phantom/Echo of Flicker Fang")]
    public class EchoFlickerFang : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoFlickerFang = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive, handled during dodge movement/animation
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoFlickerFangLevel = powerUpLevel;
        }
    }
}