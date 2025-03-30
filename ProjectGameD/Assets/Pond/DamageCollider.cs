using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        int currentDamageWeapon = 25;
        Collider damageCollider;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                PlayerManager enemyCharacterManager = collider.GetComponent<PlayerManager>();
                BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                if (enemyCharacterManager != null)
                {
                    /*
                    if (enemyManager.isParrting)
                    {
                        enemyManager.GetComponent<EnemyAnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    }
                    else */

                    //Shielded Damage
                    if (shield != null && enemyCharacterManager.isBlocking)
                    {
                        float physicalDamageAfterBlock =
                            currentDamageWeapon - (currentDamageWeapon * shield.blockingColliderDamageAbsorption) / 100;

                        if (playerStats != null)
                        {
                            playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block_Guard");
                            return;
                        }
                    }
                }

                //Normal Damage
                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentDamageWeapon);
                }

            }

            if (collider.tag == "Enemy")
            {
                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                EnemyManager enemyCharacterManager = collider.GetComponent<EnemyManager>();
                BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                if (enemyCharacterManager != null)
                {

                    //Shielded Damage
                    if (shield != null && enemyCharacterManager.isBlocking)
                    {
                        float physicalDamageAfterBlock =
                            currentDamageWeapon - (currentDamageWeapon * shield.blockingColliderDamageAbsorption) / 100;

                        if (enemyStat != null)
                        {
                            enemyStat.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block_Guard");
                            return;
                        }
                    }
                }

                //Normal Damage
                if (enemyStat != null)
                {
                    enemyStat.TakeDamage(currentDamageWeapon);
                }

            }
        }
    }
}