using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of Resolute Mind")]
    public class EchoResoluteMind : PowerUp
    {

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoResoluteMind = true;
            playerData.echoResoluteMindLevel = powerUpLevel;
        }

        public override void Apply(PlayerStats playerStats)
        {
            // No event needed — it’s a passive regen handled during update
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoResoluteMindLevel = powerUpLevel;
        }
    }

}