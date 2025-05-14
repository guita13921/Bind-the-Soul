using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerDamageCollider : DamageCollider
    {
        private HashSet<Collider> enemiesHitThisAttack = new HashSet<Collider>();
        private PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        protected override void Update()
        {
            base.Update();
            playerManager = GetComponentInParent<PlayerManager>();
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

            CheckForBlock(enemyStat, enemyManager, shield, playerManager);
        }

        protected virtual void CheckForBlock(EnemyStat enemyStat, EnemyManager enemyManager, BlockingCollider shield, PlayerManager playerManager)
        {
            if (enemyManager != null && enemyManager.isBlocking && shield != null)
            {
                float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                float damageAfterBlock = currentDamageWeapon - damageBlocked;

                if (playerManager.playerData.echoAnvilborn == true)
                {
                    float bonus = playerManager.playerData.echoAnvilbornLevel * 0.10f;
                    damageAfterBlock *= 1f + Mathf.Clamp(bonus, 0f, 1f); // Optional clamp at 100%
                    Debug.Log($"Echo of the Anvilborn: +{bonus * 100}% damage vs shielded target.");
                }

                if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Heavy)
                {

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlockedMaxShieldPoint(Mathf.RoundToInt(currentDamageWeapon * 1.25f));
                    return;
                }
                else if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Medium)
                {

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon * 0.8f));
                    return;
                }
                else if (characterManager.weaponSlotManager.attackingWeapon.stantType == StantType.Light)
                {

                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon * 0.5f));
                    return;
                }
                else
                {
                    enemyStat?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block_Guard");
                    shield?.GetBlocked(Mathf.RoundToInt(currentDamageWeapon));
                    return;
                }
            }
            else
            {
                DealDamage(enemyStat, enemyManager, currentDamageWeapon, playerManager);
            }

        }

        protected virtual void DealDamage(EnemyStat enemyStat, EnemyManager enemyManager, float damage, PlayerManager playerManager)
        {

            if (characterManager.characterCombatManager.currentAttackType == AttackType.light)
            {
                damage = currentDamageWeapon * characterManager.weaponSlotManager.attackingWeapon.lightAttackDamageModifiers;

                //Modify for power-up 
                damage = CheckCritNextAttack(damage, playerManager);
                damage = CheckBladeRush(damage, playerManager);
                damage = CheckBloodhound(damage, playerManager, enemyManager);
                int currentDamage = Mathf.RoundToInt(damage);


                if (enemyStat != null)
                {
                    if (enemyStat.isBoss || enemyManager.isStunning)
                        enemyStat.TakeDamageNoAnimation(currentDamage);
                    else
                        enemyStat.TakeDamage(currentDamage);
                }
            }
            else if (characterManager.characterCombatManager.currentAttackType == AttackType.Heavy)
            {
                damage = currentDamageWeapon * characterManager.weaponSlotManager.attackingWeapon.heavyAttackDamageModifiers;

                //Modify for power-up 
                damage = CheckCritNextAttack(damage, playerManager);
                damage = CheckStoneborns(damage, playerManager);
                damage = CheckBladeRush(damage, playerManager);
                damage = CheckBloodhound(damage, playerManager, enemyManager);


                int currentDamage = Mathf.RoundToInt(damage);
                Debug.Log(currentDamage);

                if (enemyStat != null)
                {
                    if (enemyStat.isBoss || enemyManager.isStunning)
                        enemyStat.TakeDamageNoAnimation(currentDamage);
                    else
                        enemyStat.TakeDamage(currentDamage);
                }

            }
        }

        protected virtual float CheckCritNextAttack(float damage, PlayerManager playerManager)
        {
            if (playerManager.playerData.duelistSet2Bonus == true && playerManager.playerData.critAttacksRemaining > 0)
            {
                playerManager.playerData.critAttacksRemaining--;
                return damage * 2;
            }
            else
            {
                return damage;
            }
        }

        protected virtual float CheckStoneborns(float damage, PlayerManager playerManager)
        {
            if (playerManager.playerData.echoStoneborn == true)
            {
                float bonus = playerManager.playerData.echoStonebornLevel * 0.25f; // 25% per level
                Debug.Log($"Echo of the Stoneborn (Lv {playerManager.playerData.echoStonebornLevel}): Charged attack damage increased by {bonus * 100}%.");
                return damage *= 1f + bonus;
            }
            else
            {
                return damage;
            }
        }

        protected virtual float CheckBladeRush(float damage, PlayerManager playerManager)
        {
            if (playerManager.playerData.echoBladeRush == true && playerManager.weaponSlotManager.attackingWeapon.weaponType == WeaponType.Dagger)
            {
                float bonus = playerManager.playerStats.bladeRushDamageBonus;
                Debug.Log($"Echo of the Stoneborn (Lv {playerManager.playerData.echoBladeRushLevel})");
                return damage *= 1f + bonus;
            }
            else
            {
                return damage;
            }
        }

        protected virtual float CheckBloodhound(float damage, PlayerManager playerManager, EnemyManager enemyManager)
        {
            if (playerManager.playerData.echoBloodhound == true && enemyManager.enemyStat.currentHealth < (enemyManager.enemyStat.maxHealth * 0.5f))
            {
                float bonus = 0.05f * playerManager.playerData.echoBloodhoundLevel + 0.05f; // 10%, 15%, ...
                Debug.Log($"Echo of the Bloodhound: Bonus damage applied ({bonus * 100}% vs <50% HP target)");
                return damage *= 1f + bonus;
            }
            else
            {
                return damage;
            }
        }




    }
}
