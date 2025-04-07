using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        public int currentDamageWeapon;
        Collider damageCollider;
        CharacterManager characterManager;
        CharacterStats characterStats;

        private void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
            characterStats = GetComponent<CharacterStats>();
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
                //Debug.Log("Enemy");
                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                EnemyManager enemyManager = collider.GetComponent<EnemyManager>();
                BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                if (enemyManager != null && enemyStat != null)
                {

                    if (shield != null && enemyManager.isBlocking)
                    {
                        float physicalDamageAfterBlock =
                            currentDamageWeapon - (currentDamageWeapon * shield.blockingColliderDamageAbsorption) / 100;

                        if (enemyStat != null)
                        {
                            enemyStat.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block_Guard");
                            shield.GetBlocked(Mathf.RoundToInt(currentDamageWeapon));
                            return;
                        }
                    }
                }

                //Normal Damage
                if (enemyStat != null)
                {
                    if (enemyStat.isBoss)
                    {
                        enemyStat.TakeDamageNoAnimation(currentDamageWeapon);
                    }
                    else
                    {
                        Debug.Log("Damage:" + currentDamageWeapon);
                        enemyStat.TakeDamage(currentDamageWeapon);
                    }
                }

            }
        }
    }
}