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

        void Awake()
        {
            // Find the first active PlayerCombat in the scene
            playerStats = FindObjectOfType<PlayerStats>();
        }

        private void OnTriggerEnter(Collider collider)
        {

            if (playerStats != null && collider.CompareTag("Player"))
            {
                if (collider.tag == "Player")
                {
                    PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                    PlayerManager playerManager = collider.GetComponent<PlayerManager>();
                    BlockingColliderPlayer shield = collider.transform.GetComponentInChildren<BlockingColliderPlayer>();

                    if (playerManager.isInvulerable)
                    {
                        return;
                    }

                    if (playerManager != null)
                    {
                        if (playerManager.isInvulerable)
                        {
                            return;
                        }
                        else if (shield != null && playerManager.isBlocking)
                        {
                            float physicalDamageAfterBlock =
                            damage - (damage * shield.blockingColliderDamageAbsorption) / 100;
                            if (playerStats != null)
                            {
                                playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block Guard");
                                return;
                            }
                        }

                    }

                    //Normal Damage
                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(damage);
                    }

                }
            }
        }

    }
}