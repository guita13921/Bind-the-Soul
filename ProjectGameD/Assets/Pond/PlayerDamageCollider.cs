using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerDamageCollider : MonoBehaviour
    {
        public int currentDamageWeapon;
        private Collider damageCollider;

        // 🔐 Enemies hit this activation
        private HashSet<Collider> enemiesHitThisAttack = new HashSet<Collider>();

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            enemiesHitThisAttack.Clear(); // Reset for new attack
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.CompareTag("Enemy"))
                return;

            if (enemiesHitThisAttack.Contains(collider))
                return; // Already hit this enemy in this attack

            enemiesHitThisAttack.Add(collider);

            EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
            EnemyManager enemyManager = collider.GetComponent<EnemyManager>();
            BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

            if (enemyManager != null && enemyManager.isBlocking && shield != null)
            {
                float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                float damageAfterBlock = currentDamageWeapon - damageBlocked;

                enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon));
                return;
            }

            if (enemyStat != null)
            {
                if (enemyStat.isBoss)
                    enemyStat.TakeDamageNoAnimation(currentDamageWeapon);
                else
                    enemyStat.TakeDamage(currentDamageWeapon);
            }
        }
    }
}
