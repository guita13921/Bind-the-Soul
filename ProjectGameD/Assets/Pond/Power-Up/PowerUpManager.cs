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

        public void Awake()
        {
            playerStats = gameObject.GetComponent<PlayerStats>();
        }

        public void AddPowerUp(PowerUp powerUp)
        {
            collectedPowerUps.Add(powerUp);
            powerUp.Apply(playerData);
            powerUp.Apply(playerStats);
        }

    }

}