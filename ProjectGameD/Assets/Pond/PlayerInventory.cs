using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;

        public SpellItem currentSpell;
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = -1;
        public int currentLeftWeaponIndex = -1;

        public List<WeaponItem> weaponInventory;

        [Header("Player Data")]
        public PlayerData playerData; // <-- Assign this in the Inspector!s

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            // Load from ScriptableObject
            if (playerData != null)
            {
                weaponsInRightHandSlots = playerData.weaponsInRightHandSlots;
                weaponsInLeftHandSlots = playerData.weaponsInLeftHandSlots;

                weaponInventory = new List<WeaponItem>(playerData.weaponInventory);
            }
            else
            {
                Debug.LogWarning("PlayerData ScriptableObject not assigned to PlayerInventory!");
            }

            rightWeapon = unarmedWeapon;
            leftWeapon = unarmedWeapon;

        }

        public void ChangeRightWeapon()
        {
            int slotsCount = weaponsInRightHandSlots.Length;

            do
            {
                currentRightWeaponIndex++;

                // ถ้าเกินจำนวน slot แล้วให้กลับไป -1 (ถือว่าเป็น unarmed)
                if (currentRightWeaponIndex >= slotsCount)
                {
                    currentRightWeaponIndex = -1;
                    rightWeapon = unarmedWeapon;
                    weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
                    return;
                }

                // ถ้ามีอาวุธใน slot นั้น
                if (weaponsInRightHandSlots[currentRightWeaponIndex] != null)
                {
                    rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                    weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
                    return;
                }

                // ถ้าไม่มีอาวุธ จะวนลูปไปเช็ค index ถัดไป
            } while (true);
        }

        /*public void ChangeLeftWeapon()
        {
            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], false);

            }
            else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }
            else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], false);
            }
            else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }
            if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
            {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
            }
        }*/
        public void ChangeLeftWeapon()
        {
            currentLeftWeaponIndex++;

            // ตรวจสอบว่า index เกินขนาดของอาร์เรย์หรือไม่
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length)
            {
                currentLeftWeaponIndex = -1; // Reset เป็นอาวุธว่างเปล่า
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
                return;
            }

            // ข้าม index ที่เป็น null และลองเปลี่ยนไปอันถัดไป
            while (currentLeftWeaponIndex < weaponsInLeftHandSlots.Length && weaponsInLeftHandSlots[currentLeftWeaponIndex] == null)
            {
                currentLeftWeaponIndex++;
            }

            // ถ้าทุกช่องไม่มีอาวุธ ให้กลับไปใช้อาวุธเปล่า
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length)
            {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
            }
            else
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
        }

    }
}
