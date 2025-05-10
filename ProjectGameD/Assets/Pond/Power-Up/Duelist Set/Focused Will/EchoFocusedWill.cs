using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    [CreateAssetMenu(menuName = "PowerUps/Duelist/Echo of Focused Will")]
    public class EchoFocusedWill : PowerUp
    {
        private PlayerStats boundStats;

        private void OnEnable()
        {
            setName = SetName.DuelistSet;
        }

        public override void Apply(PlayerData playerData)
        {
            playerData.echoFocusedWill = true;
        }

        public override void Apply(PlayerStats playerStats)
        {
            boundStats = playerStats;
            playerStats.OnParrySuccess += RestoreStamina;
        }

        private void RestoreStamina()
        {
            if (boundStats == null) return;

            float percent = Mathf.Clamp01(powerUpLevel * 0.15f); // 15% per level
            float restoreAmount = boundStats.maxStamina * percent;

            boundStats.RestoreStamina(restoreAmount);
            Debug.Log($"Echo of Focused Will (Level {powerUpLevel}): Restored {restoreAmount} stamina after parry.");
        }

        public override void OnStacked(PlayerData playerData)
        {
            Debug.Log($"{Name} stacked to level {powerUpLevel}!");
            playerData.echoFocusedWillLevel = powerUpLevel;
        }

        public void Remove()
        {
            if (boundStats != null)
                boundStats.OnParrySuccess -= RestoreStamina;

            boundStats = null;
        }
    }

}