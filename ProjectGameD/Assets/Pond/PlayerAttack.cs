using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerAttack : MonoBehaviour
    {

        AnimatorHander animatorHander;
        PlayerManager playerManager;
        PlayerInventory playerInventory;
        private PlayerStats playerStats;
        InputHander inputHander;
        WeaponSlotManager weaponSlotManager;
        public string lastAttack;
        public string lastAttack2;


        private void Awake()
        {
            animatorHander = GetComponent<AnimatorHander>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
            inputHander = GetComponentInParent<InputHander>();



            if (animatorHander == null)
            {
                Debug.LogError("❌ ไม่พบ AnimatorHander ใน " + gameObject.name);
            }
            else
            {
                Debug.Log("✅ พบ AnimatorHander ใน " + gameObject.name);
            }
        }
        private IEnumerator HandleLightLastAttack()
        {
            yield return null;
            lastAttack = lastAttack2;

        }
        public void HandleWeaponCombo(WeaponItem weapon)
        {
            //        if (playerStats.currentStamina <= 0)
            //            return;
            if (inputHander.comboflang)
            {
                animatorHander.anim.SetBool("CanDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                    lastAttack2 = weapon.OH_Light_Attack_2;

                    StartCoroutine(HandleLightLastAttack());
                }

                if (lastAttack == weapon.OH_Light_Attack_2)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_3, true);
                }
            }

        }

        public void HandleLightAttack(WeaponItem weapon)
        {

            //            if (playerStats.currentStamina <= 0)
            //               return;
            weaponSlotManager.attackingWeapon = weapon;
            animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            lastAttack = weapon.OH_Light_Attack_1;
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {

            //          if (playerStats.currentStamina <= 0)
            //              return;
            weaponSlotManager.attackingWeapon = weapon;
            animatorHander.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }
        #region Input Action
        public void HandleALAction()
        {
            if (playerInventory.rightWeapon.isMeleeWeapon)
            {
                PerformALMeleeAction();
            }
            else if (playerInventory.rightWeapon.isSpellCaster || playerInventory.rightWeapon.isFaithCaster || playerInventory.rightWeapon.isPyroCaster)
            {
                PerformALMagicAction(playerInventory.rightWeapon);
            }

        }
        public void HandleLTAction()
        {
            if (playerInventory.leftWeapon.isShieldWeapon)
            {
                PerformLTWeaponArt(true);
            }
            else if (playerInventory.leftWeapon.isMeleeWeapon)
            {

            }
        }
        #endregion
        #region Attack Actions
        private void PerformALMeleeAction()
        {

            if (playerManager.CanDoCombo)
            {
                inputHander.comboflang = true;
                HandleWeaponCombo(playerInventory.rightWeapon);
                inputHander.comboflang = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.CanDoCombo)
                    return;
                animatorHander.anim.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerInventory.rightWeapon);
            }
        }
        private void PerformLTWeaponArt(bool isLeftWeapon)
        {
            if (playerManager.isInteracting)
                return;
            animatorHander.PlayTargetAnimation(playerInventory.leftWeapon.weapon_art, true);
            /* if (isLeftWeapon)
              {
                  animatorHander.PlayTargetAnimation(playerInventory.leftWeapon.weapon_art, true);
              }
              els
              {

              }*/
        }

        private void PerformALMagicAction(WeaponItem weapon)
        {
            /*
            if (weapon.isFaithCaster)
            {
                if (playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell)
                {

                }
            }
            */
        }
        #endregion

    }
}


