using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class BlockingCollider : MonoBehaviour
    {
        [SerializeField] EnemyManager enemyManager;
        [SerializeField] EnemyAnimatorManager enemyAnimatorManager;
        [SerializeField] EnemyWeaponSlotManager enemyWeaponSlotManager;
        BoxCollider blockingCollider;
        public float blockingColliderDamageAbsorption; // Using Both Player/Enemy

        public bool isActive = true;
        public int blockingColliderShieldPoint;
        public int maxShieldPoint;
        public float shieldRegenDelay; // Time before regen starts
        public float shieldRegenRate; // Time between each shield point regen
        public int shieldRegenAmount; // Amount of shield points restored per tick

        [Header("UIEnemyShieldBar")]
        public UIEnemyShieldBar uIEnemyShieldBar;
        private float lastAttackTime;
        private Coroutine regenCoroutine;

        private void Awake()
        {
            //enemyAnimatorManager = GetComponentInParent<EnemyAnimatorManager>();
            enemyManager = GetComponentInParent<EnemyManager>();
            blockingCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            uIEnemyShieldBar.SetCurrentShield(blockingColliderShieldPoint);
            // Continuously check if enough time has passed for regen
            if (Time.time - lastAttackTime >= shieldRegenDelay)
            {
                if (regenCoroutine == null) // Start regen if not already running
                {
                    regenCoroutine = StartCoroutine(RegenerateShield());
                }
            }
        }


        public void SetShieldHealth(WeaponItem weapon) // Used only by Enemy
        {
            if (weapon != null && weapon.weaponType == WeaponType.Shield)
            {
                blockingColliderShieldPoint = weapon.ShieldPoint;
                maxShieldPoint = weapon.ShieldPoint;
            }
        }

        public void GetBlocked(int damage)
        {
            blockingColliderShieldPoint -= damage;
            blockingColliderShieldPoint = Mathf.Max(0, blockingColliderShieldPoint); // Ensure shield doesn't go negative

            if (blockingColliderShieldPoint <= 0 && isActive)
            {
                GuardBreak();
            }

            lastAttackTime = Time.time; // Reset the regen timer when taking damage

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
            }
        }

        public void EnableBlockingCollider()
        {
            blockingCollider.enabled = true;
        }

        public void DisableBlockingCollider()
        {
            blockingCollider.enabled = false;
        }

        private void GuardBreak()
        {
            isActive = false;
            enemyManager.hasShield = false;
            enemyManager.isBlocking = false;
            enemyManager.isStunning = true;
            enemyManager.currentStunningTime = enemyManager.stunningTime;
            enemyWeaponSlotManager.ShieldBreak();
            enemyAnimatorManager.PlayTargetAnimation("Start Stun", true);
            enemyAnimatorManager.animator.SetBool("isBlocking", false);
        }

        private IEnumerator RegenerateShield()
        {
            while (blockingColliderShieldPoint < maxShieldPoint)
            {
                blockingColliderShieldPoint += shieldRegenAmount;
                blockingColliderShieldPoint = Mathf.Min(blockingColliderShieldPoint, maxShieldPoint); // Cap at max value

                yield return new WaitForSeconds(shieldRegenRate);
            }

            regenCoroutine = null; // Reset coroutine reference when fully regenerated
        }


    }
}