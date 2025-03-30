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

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        private void Awake()
        {
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
            }

            if (leftHandWeapon != null)
            {
                LoadWeaponOnSlot(leftHandWeapon, true);
            }
        }

        public void LoadWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }

        }

        public void OpenDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

    }

}