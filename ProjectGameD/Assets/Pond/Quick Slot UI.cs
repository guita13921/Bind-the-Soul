using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG

{
    public class QuickSlotUI : MonoBehaviour
    {
        public Image leftWeaponIcon;  // ✅ Fixed naming consistency
        public Image rightWeaponIcon; // ✅ Fixed spelling error

        /*public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItem weapon)
        {
            if (isLeft == false)
            {
                if (weapon.itemIcon != null)
                {
                    rightWeaponIcon.sprite = weapon.itemIcon; // ✅ Fixed variable name
                    rightWeaponIcon.enabled = true;
                }
                else
                {
                    rightWeaponIcon.sprite = null;
                    rightWeaponIcon.enabled = false;
                }

            }
            else
            {
                if (weapon.itemIcon != null)
                {
                    leftWeaponIcon.sprite = weapon.itemIcon; // ✅ Fixed variable name
                    leftWeaponIcon.enabled = true;
                }
                else
                {
                    leftWeaponIcon.sprite = null;
                    leftWeaponIcon.enabled = false;
                }

            }
        }*/
        public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItem weapon)
        {
            // เลือก targetIcon ตาม isLeft
            Image targetIcon = isLeft ? leftWeaponIcon : rightWeaponIcon;

            // อัพเดต sprite และ visibility
            targetIcon.sprite = weapon?.itemIcon; // ใช้ ? เพื่อหลีกเลี่ยง null reference
            targetIcon.enabled = weapon?.itemIcon != null;
        }

    }
}