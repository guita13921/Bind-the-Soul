using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

namespace SG
{

    public class BossWeapon : MonoBehaviour
    {
        public int damage;
        public PlayerStats playerStats;

        [SerializeField] bool isCurseAttack = false;

        void Awake()
        {
            // Find the first active PlayerCombat in the scene
            playerStats = FindObjectOfType<PlayerStats>();
        }

        private void OnTriggerEnter(Collider other)
        {

            if (playerStats != null && other.CompareTag("Player"))
            {
                playerStats.TakeDamage(damage);
            }
        }

    }
}