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
        PlayerStats playerStats;
        InputHander inputHander;
        WeaponSlotManager weaponSlotManager;
        PlayerData playerData;

        [Header("Attack Animations")]
        string OH_Light_Attack_1 = "OH_Light_Attack_1";
        string OH_Light_Attack_2 = "OH_Light_Attack_2";
        string OH_Light_Attack_3 = "OH_Light_Attack_3";
        string OH_Light_Attack_4 = "OH_Light_Attack_4";
        string OH_Light_Attack_5 = "OH_Light_Attack_5";
        string OH_Heavy_Attack_1 = "OH_Heavy_Attack_1";
        string OH_Shield_Attack_1 = "OH_Shield_Attack_1";

        string weapon_art = "Weapon_Art";

        public string lastAttack;
        public string lastAttack_current;

        LayerMask backStabLayer = 1 << 12;
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

        #region Input Action
        public void HandleALAction()
        {
            if (playerInventory.rightWeapon.weaponType == WeaponType.StrightSword
                || playerInventory.rightWeapon.weaponType == WeaponType.Hammer
                || playerInventory.rightWeapon.weaponType == WeaponType.Dagger)
            {
                PerformALMeleeAction();
            }
            else if (playerInventory.rightWeapon.weaponType == WeaponType.PyroCaster
                || playerInventory.rightWeapon.weaponType == WeaponType.SpellCaster
                || playerInventory.rightWeapon.weaponType == WeaponType.FaithCaster)
            {
                PerformALMagicAction(playerInventory.rightWeapon);
            }
        }

        public void HandleQAction() //Shields
        {
            PerformQBlockingAction();
        }

        public void HandleLTAction() //Parry
        {
            if (playerInventory.rightWeapon.weaponType == WeaponType.Shield)
            {
                PerformLTWeaponArt(inputHander.twohandflag);
            }
            else if (playerInventory.rightWeapon.weaponType == WeaponType.StrightSword)
            {

            }
        }
        #endregion

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
                if (lastAttack == OH_Light_Attack_1)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_2, true);
                    lastAttack_current = OH_Light_Attack_2;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

                if (lastAttack == OH_Light_Attack_2)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_3, true);
                    lastAttack_current = OH_Light_Attack_3;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

                if (lastAttack == OH_Light_Attack_3)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_4, true);
                    lastAttack_current = OH_Light_Attack_3;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                }

                if (lastAttack == OH_Light_Attack_4)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_5, true);
                    lastAttack_current = OH_Light_Attack_3;
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

                if (playerManager.isBlocking)
                {
                    animatorHander.PlayTargetAnimation(OH_Shield_Attack_1, true);
                    lastAttack = OH_Shield_Attack_1;
                }
                else
                {
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_1, true);
                    lastAttack = OH_Light_Attack_1;
                }
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
                animatorHander.PlayTargetAnimation(OH_Heavy_Attack_1, true);
                lastAttack = OH_Heavy_Attack_1;

            }
        }

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
                animatorHander.anim.SetBool("isInteracting", true);
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
                animatorHander.PlayTargetAnimation(weapon_art, true);
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
        public void AttemptBackStabOrRiposte()
        {
            RaycastHit hit;
            if (Physics.Raycast(inputHander.CriticalAttackRayCastStartPoint.position,
            transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer))
            {
                CharacterManager enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                if (enemyCharacterManger != null)
                {
                    //CHECK FOR TEAM I.D (So you cant back stab friend or yourself ?)
                    playerManager.lockOnTransform.position = enemyCharacterManger.backStabCollider.backStabberStandPoint.position;
                    Vector3 rotationDirection = playerManager.lockOnTransform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.lockOnTransform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.lockOnTransform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.lockOnTransform.rotation = targetRotation;
                    animatorHander.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Damage01", true);
                    //make enemy play animation
                    //do damage
                }
            }
        }
    }
}


