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
        public string lastAttack2;

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
            else
            {
                //                Debug.Log("✅ พบ AnimatorHander ใน " + gameObject.name);
            }
        }
        private IEnumerator HandleLightLastAttack()
        {
            yield return null;
            lastAttack = lastAttack2;

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
            }

            animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            lastAttack = weapon.OH_Light_Attack_1;
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
            }

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


