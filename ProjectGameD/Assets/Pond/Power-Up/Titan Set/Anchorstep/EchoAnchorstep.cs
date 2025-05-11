using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Titan/Echo of Anchorstep")]
    public class EchoAnchorstep : PowerUp
    {
        public override void Apply(PlayerData playerData)
        {
            playerData.echoAnchorstep = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // Passive effect handled during stagger checks
        }

    }

}