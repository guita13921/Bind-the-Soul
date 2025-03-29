using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

namespace SG
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot rightHandslot;
        WeaponHolderSlot leftHandslot;

        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;


        public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                //leftHandslot.currentWeaponModel = weapon;
                leftHandslot.LoadWeaponModel(weapon);
            }
            else
            {
                //rightHandslot.currentWeaponModel = weapon;
                leftHandslot.LoadWeaponModel(weapon);
            }
        }

        public void LoadWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandslot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
            else
            {
                rightHandDamageCollider = rightHandslot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
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