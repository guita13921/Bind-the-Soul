using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of Razor Timing")]
    public class EchoRazorTiming : PowerUp
    {
        private PlayerStats boundStats;

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoRazorTiming = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            boundStats = playerStats;
            playerStats.OnParrySuccess += TriggerAttackSpeedBoost;
        }

        private void TriggerAttackSpeedBoost()
        {
            if (boundStats == null) return;

            float bonusPercent = GetAttackSpeedBonus();
            float duration = GetDuration();

            boundStats.animatorHander.ApplyAttackSpeedBoost(bonusPercent, duration);
            Debug.Log($"Echo of Razor Timing (Lv {powerUpLevel}): +{bonusPercent * 100}% attack speed for {duration}s after parry.");
        }


        private float GetAttackSpeedBonus()
        {
            return 0.05f + (0.05f * powerUpLevel); // 10%, 15%, 20%, 25%, ...
        }

        private float GetDuration()
        {
            return 4f + powerUpLevel; // 5, 6, 7, 8, ... seconds
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoRazorTimingLevel = powerUpLevel;
        }

        public void Remove()
        {
            if (boundStats != null)
                boundStats.OnParrySuccess -= TriggerAttackSpeedBoost;

            boundStats = null;
        }
    }

}