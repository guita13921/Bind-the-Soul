using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of Returning Flow")]
    public class EchoReturningFlow : PowerUp
    {
        private PlayerStats boundStats;

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoReturningFlow = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            boundStats = playerStats;
            playerStats.OnParrySuccess += ActivateFreeDodgeWindow;
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoReturningFlowLevel = powerUpLevel;
        }

        private void ActivateFreeDodgeWindow()
        {
            if (boundStats == null) return;

            float duration = 3f + (2f * powerUpLevel); // 5, 7, 9, 11...
            boundStats.playerLocomotion.ActivateFreeDodge(duration);
            Debug.Log($"Echo of Returning Flow (Lv {powerUpLevel}): Free dodge for {duration}s after parry.");
        }

        public void Remove()
        {
            if (boundStats != null)
                boundStats.OnParrySuccess -= ActivateFreeDodgeWindow;

            boundStats = null;
        }
    }
}