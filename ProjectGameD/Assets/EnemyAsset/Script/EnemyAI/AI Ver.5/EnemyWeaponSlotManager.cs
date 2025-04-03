using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

namespace SG
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        public WeaponItem rightHandWeapon;
        public WeaponItem leftHandWeapon;

        [SerializeField] WeaponHolderSlot rightHandSlot;
        [SerializeField] WeaponHolderSlot leftHandSlot;

        [SerializeField] DamageCollider leftHandDamageCollider;
        [SerializeField] DamageCollider rightHandDamageCollider;

        EnemyEffectManager enemyEffectManager;
        EnemyManager enemyManager;

        private void Awake()
        {
            enemyEffectManager = GetComponentInParent<EnemyEffectManager>();
            enemyManager = GetComponentInParent<EnemyManager>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

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
                leftHandSlot.isShield = weapon.isShield;
                if (weapon.isShield)
                {
                    enemyManager.hasShield = weapon.isShield;
                }
            }
            else
            {
                rightHandSlot.isShield = weapon.isShield;
                if (weapon.isShield)
                {
                    enemyManager.hasShield = weapon.isShield;
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
                leftHandDamageCollider = leftHandSlot.currentWeaponModel?.GetComponentInChildren<DamageCollider>();
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel?.GetComponentInChildren<DamageCollider>();
                enemyEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
            }
        }

        public void OpenDamageCollider()
        {
            Debug.Log("OpenDamageCollider() called.");
            if (rightHandDamageCollider != null)
            {
                rightHandDamageCollider.EnableDamageCollider();
            }
            else
            {
                Debug.LogError("rightHandDamageCollider is null!");
            }
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public bool LoadShield()
        {
            return leftHandSlot.isShield || rightHandSlot.isShield;
        }


    }

}