using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyDamageCollider : DamageCollider
    {
        public LayerMask damageableLayers;
        private EnemyManager enemyManager;

        // 🛑 Track who has already been hit
        private HashSet<Collider> hitTargets = new HashSet<Collider>();

        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponent<Collider>();
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        private void Start()
        {
            enemyManager = GetComponentInParent<EnemyManager>();
            characterSoundFXManager = GetComponentInParent<CharacterSoundFXManager>();
        }

        public void EnableDamageCollider()
        {
            hitTargets.Clear(); // Reset hit list each time a new attack starts
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        public void CheckForParry(PlayerManager playerManager)
        {
            characterSoundFXManager?.PlayPariedSounds();
            var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();

            animMgr?.PlayTargetAnimation("Start Stun", true);
            animMgr?.animator.SetBool("IsBlocking", false);

            if (enemyManager != null)
            {
                enemyManager.canBeRiposted = true;
                enemyManager.isBlocking = false;
                enemyManager.isStunning = true;
                enemyManager.currentStunningTime = enemyManager.stunningTime;
            }

        }

        private void OnTriggerEnter(Collider collider)
        {
            if ((damageableLayers.value & (1 << collider.gameObject.layer)) == 0)
                return;

            // 🚫 Already hit this object
            if (hitTargets.Contains(collider))
                return;

            hitTargets.Add(collider); // ✅ Mark as hit

            // 🎯 Player
            if (collider.CompareTag("Player"))
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                PlayerManager playerManager = collider.GetComponent<PlayerManager>();
                BlockingColliderPlayer shield = collider.transform.GetComponentInChildren<BlockingColliderPlayer>();

                if (playerManager.isInvulerable)
                    return;

                if (playerManager.isParrying)
                {
                    CheckForParry(playerManager);
                    return;
                }

                if (shield != null && playerManager.isBlocking)
                {
                    float blocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float finalDamage = currentDamageWeapon - blocked;

                    playerStats?.TakeDamage(Mathf.RoundToInt(finalDamage), "Block Guard");

                    int staminaDamage = Mathf.RoundToInt(currentDamageWeapon * shield.staminaDamageModifier / 100);
                    playerStats?.TakeStaminaDamage(staminaDamage);

                    var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();
                    if (animMgr != null)
                    {
                        string recoilAnim = $"AttackRecoil_0{Random.Range(1, 6)}";
                        animMgr.PlayRecoilAnimation(recoilAnim);
                    }

                    return;
                }

                playerStats?.TakeDamage(currentDamageWeapon);
                return;
            }

            // 🧟 Enemy (friendly fire)
            if (collider.CompareTag("Enemy"))
            {
                if (collider.GetComponent<EnemyManager>() == enemyManager)
                    return; // Prevent self-hit

                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                EnemyManager targetEnemyManager = collider.GetComponent<EnemyManager>();
                BlockingCollider shield = collider.GetComponentInChildren<BlockingCollider>();

                if (targetEnemyManager != null && targetEnemyManager.isBlocking && shield != null)
                {
                    float blocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float final = currentDamageWeapon - blocked;

                    enemyStat?.TakeDamage(Mathf.RoundToInt(final), "Block_Guard");
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
}
