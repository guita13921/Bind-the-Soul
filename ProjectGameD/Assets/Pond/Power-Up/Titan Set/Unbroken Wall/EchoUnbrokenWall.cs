using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Titan/Echo of the Unbroken Wall")]
    public class EchoUnbrokenWall : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoUnbrokenWall = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // No subscription needed — passive immunity to guard break
        }
    }
}