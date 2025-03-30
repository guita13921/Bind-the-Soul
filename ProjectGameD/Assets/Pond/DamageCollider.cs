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
            if (collider.tag == "Hittable")
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentDamageWeapon);
                }

                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();

                if (enemyStat != null)
                {
                    enemyStat.TakeDamage(currentDamageWeapon);
                }
            }
        }

    }
}