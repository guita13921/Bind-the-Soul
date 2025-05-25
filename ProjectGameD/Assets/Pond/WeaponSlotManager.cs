using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    public class WeaponSlotManager : MonoBehaviour
    {
        PlayerManager playerManager;
        PlayerInventory playerInventory;
        AnimatorHander animatorHander;
        Animator animator;
        QuickSlotUI quickSlotUI;
        PlayerStats playerStats;
        InputHander inputHander;

        [SerializeField] CharacterSoundFXManager characterSoundFXManager;

        [SerializeField] public WeaponItem attackingWeapon;
        [SerializeField] public WeaponHolderSlot leftHandSlot;
        [SerializeField] public WeaponHolderSlot rightHandSlot;
        [SerializeField] public CharacterEffectManager characterEffectManager;
        WeaponHolderSlot backSlot;

        [SerializeField] public PlayerDamageCollider leftHandDamgeCollider;
        [SerializeField] public PlayerDamageCollider righthandDamgeCollider;


        private void Awake()
        {
            characterEffectManager = GetComponentInParent<CharacterEffectManager>();
            animator = GetComponent<Animator>();
            animatorHander = GetComponent<AnimatorHander>();
            quickSlotUI = FindObjectOfType<QuickSlotUI>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerStats = GetComponentInParent<PlayerStats>();
            inputHander = GetComponentInParent<InputHander>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            characterSoundFXManager = GetComponentInParent<CharacterSoundFXManager>();

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

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            //animator.SetBool("isDrawWeapon", true);
            if (weaponItem != null)
            {

                if (isLeft)
                {
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    characterEffectManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                    LoadLeftWeaponDamageCollider();
                    quickSlotUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    animatorHander.PlayTargetAnimation(weaponItem.offHandIdleAniamtion, false, true);

                }
                else /*if (isRight)*/
                {
                    if (inputHander.twohandflag)
                    {
                        backSlot.LoadWeaponModel(weaponItem);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        animatorHander.PlayTargetAnimation("Left Arm Empty", false, true);

                    }
                    else
                    {
                        leftHandSlot.UnloadWeaponAndDestroy();
                    }

                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    characterEffectManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
                    LoadRightWeaponDamageCollider();
                    quickSlotUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    animatorHander.anim.runtimeAnimatorController = weaponItem.weaponController;
                }
            }
        }

        #region Handle Weapon's Damage Colldier

        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamgeCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<PlayerDamageCollider>();
            if (leftHandDamgeCollider != null) leftHandDamgeCollider.currentDamageWeapon = playerInventory.leftWeapon.damage;
            // leftHandDamgeCollider.currentDamageWeapon = playerInventory.leftWeapon.damage;
            // leftHandDamgeCollider.currentDamageWeapon = playerInventory.leftWeapon.damage;
            //            leftHandDamgeCollider.currentDamageWeapon = attackingWeapon.damage;

        }

        private void LoadRightWeaponDamageCollider()
        {
            righthandDamgeCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<PlayerDamageCollider>();
            righthandDamgeCollider.currentDamageWeapon = playerInventory.rightWeapon.damage;

            //righthandDamgeCollider.currentDamageWeapon = attackingWeapon.damage;
        }

        public void OpenRightDamageCollider()
        {
            if (righthandDamgeCollider != null) righthandDamgeCollider.EnableDamageCollider();
            characterSoundFXManager.PlayRandomWeaponWhooshesSoundFX();
            animatorHander.anim.SetBool("isUsingRightHand", true);
        }

        public void OpenLeftDamageCollider()
        {
            if (leftHandDamgeCollider != null) leftHandDamgeCollider.EnableDamageCollider();
            characterSoundFXManager.PlayRandomWeaponWhooshesSoundFX();
            animatorHander.anim.SetBool("isUsingLefthand", true);
        }

        public void CloseRightHandDamgeCollider()
        {
            if (righthandDamgeCollider != null) righthandDamgeCollider.DisableDamageCollider();
        }

        public void CloseLeftHandDamgeCollider()
        {
            if (leftHandDamgeCollider != null) leftHandDamgeCollider.DisableDamageCollider();
        }

        public void OpenDamageCollider()
        {
            if (playerManager.isUsingLefthand)
            {
                leftHandDamgeCollider.EnableDamageCollider();
            }
            else if (playerManager.isUsingRightHand)
            {
                righthandDamgeCollider.EnableDamageCollider();
            }
        }

        public void CloseDamageCollider()
        {
            if (leftHandDamgeCollider != null)
                leftHandDamgeCollider.DisableDamageCollider();

            if (righthandDamgeCollider != null)
                righthandDamgeCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle Weapon's Stamina Drainage


        public void DrainStaminaLightAttack()
        {
            if (attackingWeapon != null)
                playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackStaminaMultiplier));

            animatorHander.anim.SetBool("IsUsingRightHand", true);
        }

        public void DrainStaminaHeavyAttack()
        {
            if (attackingWeapon != null)
                playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackStaminaMultiplier));

            animatorHander.anim.SetBool("IsUsingRightHand", true);
        }

        public void DrainStaminaWeaponArt()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(leftHandSlot.currentWeaponItem.baseStamina * leftHandSlot.currentWeaponItem.WeaponArtStaminaMultiplier));

            animatorHander.anim.SetBool("IsUsingLeftHand", true);
        }

        #endregion


        #region Cancel Aniamtion
        public void ResetInteracting()
        {
            animator.SetBool("isInteracting", false);
        }

        public void SetInteracting()
        {
            animator.SetBool("isInteracting", true);
        }


        #endregion
    }
}

