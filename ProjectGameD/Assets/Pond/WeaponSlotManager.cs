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
        [SerializeField] public WeaponItem attackingWeapon;

        [SerializeField] public WeaponHolderSlot leftHandSlot;
        [SerializeField] public WeaponHolderSlot rightHandSlot;
        WeaponHolderSlot backSlot;

        [SerializeField] public DamageCollider leftHandDamgeCollider;
        [SerializeField] public DamageCollider righthandDamgeCollider;

        Animator animator;

        QuickSlotUI quickSlotUI;
        PlayerStats playerStats;
        InputHander inputHander;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            quickSlotUI = FindObjectOfType<QuickSlotUI>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponentInParent<PlayerStats>();
            inputHander = GetComponentInParent<InputHander>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

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
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
                quickSlotUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                #region Handle Left Weapon Idle Animations
                if (weaponItem != null)
                {
                    animator.CrossFade(weaponItem.left_hand_idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                }
                #endregion
            }
            else /*if (isRight)*/
            {
                if (inputHander.twohandflag)
                {
                    animator.CrossFade(weaponItem.th_idle, 0.2f);
                }
                else
                {

                    #region Handle Right Weapon Idle Animations
                    animator.CrossFade("Both Arms Empty", 0.2f);
                    if (weaponItem != null)
                    {

                        animator.CrossFade(weaponItem.right_hand_idle, 0.2f);
                    }
                    else
                    {

                        animator.CrossFade("Right Arm Empty", 0.2f);
                    }
                    #endregion
                }
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
                quickSlotUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
            }
        }

        #region Handle Weapon's Damage Colldier

        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamgeCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            //            leftHandDamgeCollider.currentDamageWeapon = attackingWeapon.damage;

        }

        private void LoadRightWeaponDamageCollider()
        {
            righthandDamgeCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            //righthandDamgeCollider.currentDamageWeapon = attackingWeapon.damage;
        }

        public void OpenRightDamageCollider()
        {
            righthandDamgeCollider.EnableDamageCollider();
        }
        public void OpenLeftDamageCollider()
        {
            leftHandDamgeCollider.EnableDamageCollider();
        }

        public void CloseRightHandDamgeCollider()
        {
            righthandDamgeCollider.DisableDamageCollider();
        }
        public void CloseLeftHandDamgeCollider()
        {
            leftHandDamgeCollider.DisableDamageCollider();
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
            leftHandDamgeCollider.DisableDamageCollider();
            righthandDamgeCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }
        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}

