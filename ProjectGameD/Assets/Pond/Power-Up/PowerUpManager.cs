using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PowerUpManager : MonoBehaviour
    {
        public PlayerData playerData;
        public PlayerStats playerStats;
        public List<PowerUp> collectedPowerUps = new();

        private Dictionary<SetName, int> setCounts = new();
        private HashSet<SetName> applied2SetBonuses = new();
        private HashSet<SetName> applied4SetBonuses = new();

        public void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
        }

        public void AddPowerUp(PowerUp newPowerUpAsset)
        {
            PowerUp existing = collectedPowerUps.Find(p => p.originalAsset == newPowerUpAsset);

            if (existing != null)
            {
                existing.powerUpLevel++;
                existing.OnStacked(playerData);
            }
            else
            {
                PowerUp instance = Instantiate(newPowerUpAsset);
                instance.powerUpLevel = 1;
                instance.OnStacked(playerData);

                instance.originalAsset = newPowerUpAsset;

                collectedPowerUps.Add(instance);
                instance.Apply(playerData);
                instance.Apply(playerStats);
            }

            CountSets();
        }


        private void CountSets()
        {
            setCounts.Clear();

            // Count how many *unique* power-ups exist per set (not total levels)
            foreach (var powerUp in collectedPowerUps)
            {
                if (!setCounts.ContainsKey(powerUp.setName))
                    setCounts[powerUp.setName] = 0;

                setCounts[powerUp.setName] += 1; // count 1 per unique PowerUp
            }

            foreach (var pair in setCounts)
            {
                var setName = pair.Key;
                var count = pair.Value;

                if (count >= 2 && !applied2SetBonuses.Contains(setName))
                {
                    ApplySetBonus(setName, 2);
                    applied2SetBonuses.Add(setName);
                }

                if (count >= 4 && !applied4SetBonuses.Contains(setName))
                {
                    ApplySetBonus(setName, 4);
                    applied4SetBonuses.Add(setName);
                }
            }
        }


        private void ApplySetBonus(SetName setName, int setCount)
        {
            if (setName == SetName.DuelistSet)
            {
                if (setCount == 2)
                {
                    playerData.duelistSet2Bonus = true;
                    playerStats.OnParrySuccess += ActivateDuelingGrace;
                    Debug.Log("Dueling Grace (2-set) activated: Parry will grant 2 crit attacks.");
                }

                if (setCount == 4)
                {
                    playerData.duelistSet4Bonus = true;
                    playerStats.OnParrySuccess += ActivatePerfectFormTimeSlow;

                    foreach (var powerUp in collectedPowerUps)
                    {
                        if (powerUp.setName == setName)
                        {
                            powerUp.Apply4SetBonus(playerData, playerStats);
                            ApplyCurse(playerData, playerStats);
                            break;
                        }
                    }

                    Debug.Log("Perfect Form (4-set) activated: Parry slows time for 5 seconds.");
                    Debug.Log("Curse activated: +25% back damage taken, +10% stamina drain on guard.");
                }
            }

            Debug.Log($"{setCount}-Set Bonus Applied for: {setName}");
        }



        private void ActivateDuelingGrace()
        {
            if (playerData.duelistSet2Bonus)
            {
                playerData.critAttacksRemaining = 2;
                Debug.Log("Dueling Grace triggered: Next 2 attacks are critical.");
            }
        }

        private void ActivatePerfectFormTimeSlow()
        {
            if (!playerData.duelistSet4Bonus) return;

            StartCoroutine(SlowTimeRoutine());
        }

        public void ApplyCurse(PlayerData playerData, PlayerStats stats)
        {
            playerData.duelistSetCurse = true;
            playerData.duelistSet4CurseDamageMultiplier = 1.15f;
        }


        private IEnumerator SlowTimeRoutine()
        {
            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            playerStats.animatorHander.anim.speed = 1f / Time.timeScale;

            yield return new WaitForSecondsRealtime(6f); // unaffected by timeScale

            float duration = 2f; // seconds to transition back to normal
            float elapsed = 0f;
            float startScale = Time.timeScale;
            float endScale = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(startScale, endScale, elapsed / duration);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                playerStats.animatorHander.anim.speed = 1f / Time.timeScale;
                yield return null;
            }

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }




    }

}