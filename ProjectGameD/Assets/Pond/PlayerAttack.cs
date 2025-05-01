using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerAttack : MonoBehaviour
    {

        AnimatorHander animatorHander;
        PlayerEquipmentManager playerEquipmentManager;
        PlayerManager playerManager;
        PlayerInventory playerInventory;
        private PlayerStats playerStats;
        InputHander inputHander;
        WeaponSlotManager weaponSlotManager;
        PlayerData playerData;

        public string lastAttack;
        public string lastAttack_current;


        private void Awake()
        {
            animatorHander = GetComponent<AnimatorHander>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            weaponSlotManager = GetComponentInParent<WeaponSlotManager>();
            inputHander = GetComponentInParent<InputHander>();

            if (animatorHander == null)
            {
                Debug.LogError("❌ ไม่พบ AnimatorHander ใน " + gameObject.name);
            }
        }

        private IEnumerator HandleLightLastAttack(String Current_lastAttack)
        {
            yield return null;
            lastAttack = Current_lastAttack;

        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;
            if (inputHander.comboflang)
            {
                animatorHander.anim.SetBool("CanDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                    lastAttack_current = weapon.OH_Light_Attack_2;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

                if (lastAttack == weapon.OH_Light_Attack_2)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_3, true);
                    lastAttack_current = weapon.OH_Light_Attack_3;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

                if (lastAttack == weapon.OH_Light_Attack_3)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_4, true);
                    lastAttack_current = weapon.OH_Light_Attack_3;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

            }

        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0 || weaponSlotManager.righthandDamgeCollider == null)
                return;

            weaponSlotManager.attackingWeapon = weapon;

            if (weaponSlotManager.attackingWeapon != null)
            {
                float damage = weapon.damage * weapon.lightAttackDamageMultiplier;

                // Add flat damage AFTER multipliers
                damage += playerStats.flatDamageBonus;

                // Blood Pact boost
                if (playerStats.playerData.bloodPactDamageModify)
                {
                    damage *= 1.2f;
                }

                // Momentum boost
                if (playerStats.playerData.momentumActive)
                {
                    damage *= 1.15f;
                }

                weaponSlotManager.righthandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);
                animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0 || weaponSlotManager.righthandDamgeCollider == null)
                return;

            weaponSlotManager.attackingWeapon = weapon;

            if (weaponSlotManager.attackingWeapon != null)
            {
                float damage = weapon.damage * weapon.heavyAttackDamageMultiplier;

                // Add flat damage AFTER multipliers
                damage += playerStats.flatDamageBonus;

                // Apply Blood Pact 20% damage increase if active
                if (playerStats.playerData.bloodPactDamageModify)
                {
                    damage *= 1.2f;
                }

                if (playerStats.playerData.momentumActive)
                {
                    damage *= 1.15f;
                }

                weaponSlotManager.righthandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);
                animatorHander.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
                lastAttack = weapon.OH_Heavy_Attack_1;

            }
        }

        /*
        public void PerformDirectionalLightAttack(WeaponItem weapon, Vector3 direction)
        {
            if (playerStats.currentStamina <= 0 || weaponSlotManager.righthandDamgeCollider == null)
                return;

            weaponSlotManager.attackingWeapon = weapon;

            if (weaponSlotManager.attackingWeapon != null)
            {
                float damage = weapon.damage * weapon.lightAttackDamageMultiplier;

                damage += playerStats.flatDamageBonus;

                if (playerStats.playerData.bloodPactDamageModify)
                {
                    damage *= 1.2f;
                }

                if (playerStats.playerData.momentumActive)
                {
                    damage *= 1.15f;
                }

                weaponSlotManager.righthandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);

                // Normalize direction and convert to local space
                Vector3 localDir = transform.InverseTransformDirection(direction.normalized);

                // Clamp the local direction to only horizontal plane (Y=0)
                Vector2 blendDirection = new Vector2(localDir.x, localDir.z).normalized;

                // Pass blend values to animator
                animatorHander.anim.SetFloat("AttackX", blendDirection.x);
                animatorHander.anim.SetFloat("AttackY", blendDirection.y);

                animatorHander.PlayTargetAnimation("HopAttack", true);

                lastAttack = weapon.OH_Light_Attack_1; // Optional if animation names are different
            }
        }
                */

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
        public void HandleQAction()
        {
            PerformQBlockingAction();
        }

        public void HandleLTAction()
        {
            if (playerInventory.leftWeapon.isShieldWeapon)
            {
                PerformLTWeaponArt(inputHander.twohandflag);
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

        private void PerformLTWeaponArt(bool isTwoHanding)
        {
            if (playerManager.isInteracting)
                return;
            if (isTwoHanding)
            {

            }
            else
            {
                animatorHander.PlayTargetAnimation(playerInventory.leftWeapon.weapon_art, true);
            }
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
        #region Defense Action

        private void PerformQBlockingAction()
        {
            if (playerManager.isInteracting)
                return;
            if (playerManager.isBlocking)
                return;
            animatorHander.PlayTargetAnimation("Block Start", false, true);
            playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;

        }
        #endregion

    }
}


