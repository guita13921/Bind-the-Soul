using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class WeaponPickUp : Interactable
    {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHander animatorHandler; // ✅ ประกาศตัวแปรก่อนใช้

            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponentInChildren<AnimatorHander>(); // ✅ ใช้ playerManager ไม่ใช่ PlayerManager

            // ✅ เช็ค null ก่อนใช้งาน rigidbody
            if (playerLocomotion != null && playerLocomotion.rigidbody != null)
            {
                playerLocomotion.rigidbody.velocity = Vector3.zero;
            }

            // ✅ เช็คว่า animatorHandler ไม่เป็น null ก่อนเล่นอนิเมชัน
            if (animatorHandler != null)
            {
                animatorHandler.PlayTargetAnimation("Pick Up Item", true);
            }

            // ✅ ตรวจสอบ playerInventory และ weapon ก่อนเพิ่มอาวุธเข้า inventory
            if (playerInventory != null && weapon != null)
            {
                playerInventory.weaponInventory.Add(weapon);
                playerManager.itemInteractbleGameObject.GetComponentInChildren<TextMeshProUGUI>().text = weapon.itemName;
                playerManager.itemInteractbleGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
                playerManager.itemInteractbleGameObject.SetActive(true);
                Destroy(gameObject); // ✅ ทำลาย object หลังจากเก็บอาวุธแล้ว
            }
        }
    }
}
