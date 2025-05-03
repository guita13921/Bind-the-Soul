using UnityEngine;

namespace SG
{
    public class EnemyDamageCollider : MonoBehaviour
    {
        public int currentDamageWeapon;
        public LayerMask damageableLayers;

        private Collider damageCollider;
        private EnemyManager enemyManager;
        private EnemySoundManager enemySoundManager;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        private void Start()
        {
            enemyManager = GetComponentInParent<EnemyManager>();
            enemySoundManager = GetComponentInParent<EnemySoundManager>();
        }

        public void EnableDamageCollider() => damageCollider.enabled = true;
        public void DisableDamageCollider() => damageCollider.enabled = false;

        private void OnTriggerEnter(Collider collider)
        {
            // Ignore if not in the allowed layer mask
            if ((damageableLayers.value & (1 << collider.gameObject.layer)) == 0)
                return;

            // Player hit
            if (collider.CompareTag("Player"))
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                PlayerManager playerManager = collider.GetComponent<PlayerManager>();
                BlockingColliderPlayer shield = collider.transform.GetComponentInChildren<BlockingColliderPlayer>();

                if (playerManager == null || playerManager.isInvulerable) return;

                if (playerManager.isParrying)
                {
                    enemySoundManager?.PlayPariedSounds();
                    var animMgr = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();
                    animMgr?.PlayTargetAnimation("Start Stun", true);
                    animMgr?.animator.SetBool("isBlocking", false);

                    if (enemyManager != null)
                    {
                        enemyManager.isBlocking = false;
                        enemyManager.isStunning = true;
                        enemyManager.currentStunningTime = enemyManager.stunningTime;
                    }

                    return;
                }

                if (shield != null && playerManager.isBlocking)
                {
                    float damageBlocked = currentDamageWeapon * shield.blockingColliderDamageAbsorption / 100f;
                    float damageAfterBlock = currentDamageWeapon - damageBlocked;

                    // Apply health damage
                    playerStats?.TakeDamage(Mathf.RoundToInt(damageAfterBlock), "Block Guard");

                    // Apply stamina damage based on weapon damage and shield modifier
                    int staminaDamage = Mathf.RoundToInt(currentDamageWeapon * shield.staminaDamageModifier / 100);
                    playerStats?.TakeStaminaDamage(staminaDamage);

                    var enemyAnimator = enemyManager?.GetComponentInChildren<EnemyAnimatorManager>();
                    if (enemyAnimator != null)
                    {
                        int randomIndex = Random.Range(1, 6);
                        string recoilAnim = $"AttackRecoil_0{randomIndex}";
                        enemyAnimator.PlayRecoilAnimation(recoilAnim);
                    }

                    return;
                }

                playerStats?.TakeDamage(currentDamageWeapon);
                return;
            }

            // Enemy hit (friendly fire allowed via LayerMask)
            if (collider.CompareTag("Enemy"))
            {
                EnemyStat enemyStat = collider.GetComponent<EnemyStat>();
                EnemyManager targetEnemyManager = collider.GetComponent<EnemyManager>();
                BlockingCollider shield = collider.transform.GetComponentInChildren<BlockingCollider>();

                if (targetEnemyManager == enemyManager)
                    return; // Don't hit self

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
