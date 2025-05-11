using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Predator/Echo of the First Fang")]
    public class EchoFirstFang : PowerUp
    {

        public override void Apply(PlayerData playerData)
        {
            playerData.echoFirstFang = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            playerStats.OnEnemyKilled += playerStats.TriggerFirstFangRegenBuff;
        }

        public void Remove(PlayerStats playerStats)
        {
            playerStats.OnEnemyKilled -= playerStats.TriggerFirstFangRegenBuff;
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoFirstFangLevel = powerUpLevel;
        }
    }

}
