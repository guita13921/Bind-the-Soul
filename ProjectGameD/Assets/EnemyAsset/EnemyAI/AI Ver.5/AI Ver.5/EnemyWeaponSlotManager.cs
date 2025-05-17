using System.Collections;
using System.Collections.Generic;
using SG;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        [SerializeField] public WeaponItem rightHandWeapon;
        [SerializeField] public WeaponItem leftHandWeapon;
        public ProjectileSpell projectileSpell;

        [SerializeField] public WeaponHolderSlot rightHandSlot;
        [SerializeField] public WeaponHolderSlot leftHandSlot;

        EnemyDamageCollider leftHandDamageCollider;
        EnemyDamageCollider rightHandDamageCollider;

        EnemyAnimatorManager enemyAnimatorManager;
        EnemyEffectManager enemyEffectManager;
        EnemyManager enemyManager;
        PlayerStats playerStats;
        [SerializeField] CharacterSoundFXManager characterSoundFXManager;

        [Header("Shield Config")]
        public BlockingCollider Shield;
        public UIEnemyShieldBar uIEnemyShieldBar;

        private void Awake()
        {
            playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyEffectManager = GetComponentInParent<EnemyEffectManager>();
            enemyManager = GetComponentInParent<EnemyManager>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            characterSoundFXManager = GetComponentInParent<CharacterSoundFXManager>();

            foreach (WeaponHolderSlot weponslot in weaponHolderSlots)
            {
                if (weponslot.isLeftHandSlot)
                {
                    leftHandSlot = weponslot;
                }
                else if (weponslot.isRightHandSlot)
                {
                    rightHandSlot = weponslot;
                }

            }
        }

        private void Start()
        {
            LoadWeaponOnBothHand();
        }

        public void CheckShieldWeapon(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                if (weapon.weaponType == WeaponType.Shield)
                {
                    enemyManager.hasShield = true;
                    Shield.SetShieldHealth(weapon);
                    uIEnemyShieldBar.SetMaxShield(weapon.ShieldPoint);
                    uIEnemyShieldBar.SetMaxShieldRegen(weapon.ShieldPoint);
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(isLeft);
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(isLeft);
            }
        }

        public void LoadWeaponOnBothHand()
        {
            if (rightHandWeapon != null)
            {
                LoadWeaponOnSlot(rightHandWeapon, false);
                CheckShieldWeapon(rightHandWeapon, false);
            }

            if (leftHandWeapon != null)
            {
                LoadWeaponOnSlot(leftHandWeapon, true);
                CheckShieldWeapon(leftHandWeapon, true);

            }
        }

        public void LoadWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel?.GetComponentInChildren<EnemyDamageCollider>();
                //  leftHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
                //enemyEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                //leftHandDamageCollider.currentDamageWeapon = leftHandWeapon.damage;
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel?.GetComponentInChildren<EnemyDamageCollider>();
                enemyEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                //rightHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
                if (rightHandDamageCollider != null) rightHandDamageCollider.currentDamageWeapon = rightHandWeapon.damage;
            }
        }

        public void OpenDamageCollider()
        {
            if (rightHandDamageCollider != null)
            {
                rightHandDamageCollider.EnableDamageCollider();
                characterSoundFXManager.PlayRandomWeaponWhooshesSoundFX();
            }

            if (leftHandDamageCollider != null)
            {
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        public void CloseDamageCollider()
        {
            enemyAnimatorManager.animator.SetBool("isAttacking", false);
            if (rightHandDamageCollider != null)
            {
                rightHandDamageCollider.DisableDamageCollider();
            }

            if (leftHandDamageCollider != null)
            {
                leftHandDamageCollider.DisableDamageCollider();
            }
        }

        public void OpenRightDamageCollider()
        {
            if (rightHandDamageCollider != null) rightHandDamageCollider.EnableDamageCollider();
            characterSoundFXManager.PlayRandomWeaponWhooshesSoundFX();
        }

        public void OpenLeftDamageCollider()
        {
            if (leftHandDamageCollider != null) leftHandDamageCollider.EnableDamageCollider();
            characterSoundFXManager.PlayRandomWeaponWhooshesSoundFX();
        }

        public bool LoadShield()
        {
            return leftHandSlot.isShield || rightHandSlot.isShield;
        }

        public void ShieldBreak()
        {
            leftHandSlot.UnloadWeaponAndDestroy();
        }

        public void SuccessfullyCastSpell()
        {
            projectileSpell.SuccessfullyCastSpell(enemyAnimatorManager, playerStats, null, this);
            enemyAnimatorManager.animator.SetBool("isFiringSpell", true);
        }

    }

}