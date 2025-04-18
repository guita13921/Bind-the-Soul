using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        public int currentDamageWeapon;
        Collider damageCollider;
        public bool enableOnStartUp = false;
        [SerializeField] EnemyManager enemyManager1;


        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enableOnStartUp;
            damageCollider.enabled = false;
        }

        void Start()
        {
            enemyManager1 = GetComponentInParent<EnemyManager>();
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
            //Hit Player
            if (collider.tag == "Player")
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                PlayerManager playerManager = collider.GetComponent<PlayerManager>();
                BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                if (playerManager != null)
                {
                    if (playerManager.isParrying)
                    {
                        enemyManager1.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("Start Stun", true);
                        enemyManager1.GetComponentInChildren<EnemyAnimatorManager>().animator.SetBool("isBlocking", false);
                        enemyManager1.isBlocking = false;
                        enemyManager1.isStunning = true;
                        enemyManager1.currentStunningTime = enemyManager1.stunningTime;
                        return;
                    }
                    else if (shield != null && playerManager.isBlocking)
                    {
                        float physicalDamageAfterBlock =
                        currentDamageWeapon - (currentDamageWeapon * shield.blockingColliderDamageAbsorption) / 100;
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
                    playerStats.TakeDamage(currentDamageWeapon);
                }

            }

            //Hit Enemy
            if (collider.tag == "Enemy")
            {
                //Debug.Log("Enemy");
                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                CharacterManager characterManager = collider.GetComponent<CharacterManager>();
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

                //Debug.Log(enemyStat);

                //Normal Damage
                if (enemyStat != null)
                {
                    if (enemyStat.isBoss)
                    {
                        enemyStat.TakeDamageNoAnimation(currentDamageWeapon);
                    }
                    else
                    {
                        enemyStat.TakeDamage(currentDamageWeapon);
                    }
                }

            }
        }
    }
}