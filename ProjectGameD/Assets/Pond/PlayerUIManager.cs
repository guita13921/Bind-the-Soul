using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PlayerUIManager : MonoBehaviour
    {
        public GameObject playerDeadUI; // Assign this in the Inspector
        public PlayerStats playerStats;

        void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
        }

        void Update()
        {
            if (playerStats.currentHealth <= 0 && !playerDeadUI.activeSelf)
            {
                playerDeadUI.SetActive(true);
            }
        }

    }
}