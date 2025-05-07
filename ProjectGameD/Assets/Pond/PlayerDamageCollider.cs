using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerDamageCollider : DamageCollider
    {
        private HashSet<Collider> enemiesHitThisAttack = new HashSet<Collider>();

        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public virtual void EnableDamageCollider()
        {
            enemiesHitThisAttack.Clear(); // Reset for new attack
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (!collider.CompareTag("Enemy"))
                return;

            if (enemiesHitThisAttack.Contains(collider))
                return; // Already hit this enemy in this attack

            enemiesHitThisAttack.Add(collider);

            EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
            EnemyManager enemyManager = collider.GetComponent<EnemyManager>();
            BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

            CheckForBlock(enemyStat, enemyManager, shield);
            DealDamage(enemyStat);
        }

        protected virtual void CheckForBlock(EnemyStat enemyStat, EnemyManager enemyManager, BlockingCollider shield)
        {
            if (enemyManager != null && enemyManager.isBlocking && shield != null)
            {
                Debug.Log(characterManager.weaponSlotManager.attackingWeapon.stantType);

                if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Heavy)
                {
                    float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float damageAfterBlock = currentDamageWeapon - damageBlocked;

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlockedMaxShieldPoint(Mathf.RoundToInt(currentDamageWeapon * 1.25f)); return;
                }
                else if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Medium)
                {
                    float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float damageAfterBlock = currentDamageWeapon - damageBlocked;

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon * 0.8f));
                    return;
                }
                else if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Light)
                {
                    float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float damageAfterBlock = currentDamageWeapon - damageBlocked;

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon * 0.5f));
                    return;
                }
                else
                {
                    float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float damageAfterBlock = currentDamageWeapon - damageBlocked;

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon));
                    return;
                }
            }

        }

        protected virtual void DealDamage(EnemyStat enemyStat)
        {
            float damage;

            if (characterManager.characterCombatManager.currentAttackType == AttackType.light)
            {
                damage = currentDamageWeapon * characterManager.weaponSlotManager.attackingWeapon.lightAttackDamageModifiers;

                /*
                Modify for power-up 
                */

                int currentDamage = Mathf.RoundToInt(damage);

                if (enemyStat != null)
                {
                    if (enemyStat.isBoss)
                        enemyStat.TakeDamageNoAnimation(currentDamage);
                    else
                        enemyStat.TakeDamage(currentDamage);
                }
            }
            else if (characterManager.characterCombatManager.currentAttackType == AttackType.Heavy)
            {
                damage = currentDamageWeapon * characterManager.weaponSlotManager.attackingWeapon.heavyAttackDamageModifiers;

                /*
                Modify for power-up 
                */

                int currentDamage = Mathf.RoundToInt(damage);
                Debug.Log(currentDamage);

                if (enemyStat != null)
                {
                    if (enemyStat.isBoss)
                        enemyStat.TakeDamageNoAnimation(currentDamage);
                    else
                        enemyStat.TakeDamage(currentDamage);
                }

            }
            else
            {

            }

        }
    }
}
