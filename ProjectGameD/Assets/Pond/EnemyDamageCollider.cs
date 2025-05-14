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
            if (!enemyManager.enemyStat.isBoss)
            {
                characterSoundFXManager?.PlayPariedSounds();
                var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();

                animMgr?.PlayTargetAnimation("Start Stun", true);
                animMgr?.animator.SetBool("IsBlocking", false);

                playerManager.playerStats.TriggerParrySuccess();

                if (enemyManager != null)
                {
                    enemyManager.canBeRiposted = true;
                    enemyManager.isBlocking = false;
                    enemyManager.isStunning = true;
                    enemyManager.currentStunningTime = enemyManager.stunningTime;

                    if (enemyManager.enemyStat.isBoss == false) playerManager.playerAttack.AttemptInstantRiposte(); //IstanceKillMinioun
                }
            }
            else
            {
                characterSoundFXManager?.PlayPariedSounds();
                var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();

                if (enemyManager.enemyStat.currentBlockRippost > 0)
                {
                    string recoilAnim = $"AttackRecoil_0{Random.Range(1, 6)}";
                    animMgr.PlayRecoilAnimation(recoilAnim);
                    enemyManager.enemyStat.currentBlockRippost--;
                }
                else
                {
                    animMgr?.PlayTargetAnimation("Start Stun", true);
                    animMgr?.animator.SetBool("IsBlocking", false);
                    enemyManager.enemyStat.currentBlockRippost = enemyManager.enemyStat.blockRippost;
                    enemyManager.canBeRiposted = true;
                    enemyManager.isStunning = true;
                    enemyManager.currentStunningTime = enemyManager.stunningTime;
                }
            }

            playerManager.playerStats.TriggerParrySuccess();
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
                    CheckForBlock(playerStats, playerManager, shield);
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

        public void CheckForBlock(PlayerStats playerStats, PlayerManager playerManager, BlockingColliderPlayer shield)
        {
            float blocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
            float finalDamage = currentDamageWeapon - blocked;
            int staminaDamage = Mathf.RoundToInt(currentDamageWeapon * shield.staminaDamageModifier / 100);

            CheckPowerUp(playerManager, staminaDamage);

            playerStats?.TakeStaminaDamage(staminaDamage);

            if (playerManager.playerStats != null && playerManager.playerStats.currentStamina <= 0)
            {
                GuardBreakPlayer(playerManager);
                return;
            }

            playerStats?.TakeDamage(Mathf.RoundToInt(finalDamage), "Block Guard");

            var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();
            bool isBoss = enemyManager.enemyStat.isBoss;

            if (animMgr != null && isBoss == false)
            {
                string recoilAnim = $"AttackRecoil_0{Random.Range(1, 6)}";
                animMgr.PlayRecoilAnimation(recoilAnim);
            }

            return;

        }

        private void GuardBreakPlayer(PlayerManager playerManager)
        {
            characterSoundFXManager.PlayRandomShielBreakSoundFX();

            if (playerManager.playerData.echoUnbrokenWall == true) return;

            playerManager.animatorHander.PlayTargetAnimation("Start Stun", true);
        }

        private void CheckPowerUp(PlayerManager playerManager, float power)
        {
            int guardLevel = playerManager.playerData.echoSilverGuardLevel;

            if (guardLevel > 0)
            {
                float reduction = 0.25f * guardLevel; // 25% per level
                float reducedPower = power * (1f - Mathf.Clamp01(reduction));
                Debug.Log($"Echo of the Silver Guard active (Level {guardLevel}): Reduced stamina cost from {power} to {reducedPower}");

                // Apply reduced power (example usage, may vary)
                power = reducedPower;
            }
        }



    }
}
