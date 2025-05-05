using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform shieldPositionOverride;
        public Transform daggerPositionOverride;
        public Transform SwordPositionOverride;
        public Transform BackWeaponPositionOverride;
        public WeaponItem currentWeapon;

        public bool isLeftHandSlot;
        public bool isRightHandSlot;
        public bool isShield;
        public bool isBackSlot;

        //public bool currentWeapon;
        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        public void UnloadWeaponAndDestroy()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            UnloadWeaponAndDestroy();

            if (weaponItem == null)
            {
                UnloadWeapon();
                return;
            }

            GameObject model = Instantiate(weaponItem.modelPrefab);
            if (model != null)
            {
                // Choose correct override parent based on weapon type
                Transform chosenParent = transform; // fallback default

                switch (weaponItem.weaponType)
                {
                    case WeaponType.Shield:
                        if (shieldPositionOverride != null)
                            chosenParent = shieldPositionOverride;
                        break;

                    case WeaponType.StrightSword:
                        if (SwordPositionOverride != null)
                            chosenParent = SwordPositionOverride;
                        break;

                    case WeaponType.Hammer:
                        if (SwordPositionOverride != null)
                            chosenParent = SwordPositionOverride;
                        break;

                    case WeaponType.Dagger:
                        if (daggerPositionOverride != null)
                            chosenParent = daggerPositionOverride;
                        break;

                    default:
                        chosenParent = transform;
                        break;
                }

                model.transform.parent = chosenParent;
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;

                // Adjust for left-hand slot if needed
                if (isLeftHandSlot)
                {
                    model.transform.localRotation = Quaternion.Euler(180, 0, 0);
                }
            }

            currentWeaponModel = model;
        }
    }
}
