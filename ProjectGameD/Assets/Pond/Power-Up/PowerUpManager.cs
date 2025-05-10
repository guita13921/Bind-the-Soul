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
            foreach (var powerUp in collectedPowerUps)
            {
                if (powerUp.setName == setName)
                {
                    if (setCount == 2)
                        powerUp.Apply2SetBonus(playerData, playerStats);
                    else if (setCount == 4)
                    {
                        powerUp.Apply4SetBonus(playerData, playerStats);
                        powerUp.ApplyCurse(playerData, playerStats);
                    }
                    break;
                }
            }
            Debug.Log($"{setCount}-Set Bonus Applied for: {setName}");
        }
    }

}