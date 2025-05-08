using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerAttack : CharacterCombatManager
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
        private string lastAttack_current;

        LayerMask backStabLayer = 1 << 12;
        LayerMask riposteLayer = 1 << 13;
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
            if (playerInventory.leftWeapon.weaponType == WeaponType.Shield)
            {
                //Debug.Log(playerInventory.leftWeapon.weaponType);
                PerformLTWeaponArt(playerInventory.leftWeapon);
            }
            else if (playerInventory.leftWeapon.weaponType == WeaponType.StrightSword)
            {

            }
            else
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
                    if (weaponSlotManager.attackingWeapon.weaponType == WeaponType.Hammer)
                    {
                        animatorHander.anim.SetBool("CanDoCombo", false);
                        animatorHander.PlayTargetAnimation(OH_Light_Attack_1, true);
                        lastAttack_current = OH_Light_Attack_1;
                        StartCoroutine(HandleLightLastAttack(lastAttack_current));
                    }
                    else
                    {
                        animatorHander.anim.SetBool("CanDoCombo", false);
                        animatorHander.PlayTargetAnimation(OH_Light_Attack_3, true);
                        lastAttack_current = OH_Light_Attack_3;
                        StartCoroutine(HandleLightLastAttack(lastAttack_current));
                    }
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

            if (inputHander.twohandflag)
            {

            }
            else
            {

            }

            if (weaponSlotManager.attackingWeapon != null)
            {

                float damage = weapon.damage;
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

                currentAttackType = AttackType.light;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0 || weaponSlotManager.righthandDamgeCollider == null)
                return;

            weaponSlotManager.attackingWeapon = weapon;

            if (inputHander.twohandflag)
            {

            }
            else
            {

            }


            if (weaponSlotManager.attackingWeapon != null)
            {
                float damage = weapon.damage;
                weaponSlotManager.righthandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);

                animatorHander.PlayTargetAnimation(OH_Heavy_Attack_1, true);
                lastAttack = OH_Heavy_Attack_1;
                currentAttackType = AttackType.Heavy;

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

        private void PerformLTWeaponArt(WeaponItem weapon)
        {
            if (playerManager.isInteracting)
                return;

            weaponSlotManager.attackingWeapon = weapon;

            if (weaponSlotManager.attackingWeapon != null)
            {
                float damage = weapon.damage;

                if (weaponSlotManager.leftHandDamgeCollider != null)
                    weaponSlotManager.leftHandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);

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
                PlayerDamageCollider rightWeapon = weaponSlotManager.righthandDamgeCollider;
                if (enemyCharacterManger != null)
                {
                    //CHECK FOR TEAM I.D (So you cant back stab friend or yourself ?)

                    playerManager.lockOnTransform.position = enemyCharacterManger.backStabCollider.CriticalDamageStandPosition.position;

                    Vector3 rotationDirection = playerManager.lockOnTransform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.lockOnTransform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.lockOnTransform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.lockOnTransform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiple * rightWeapon.currentDamageWeapon;
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;

                    animatorHander.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Back Stabed", true);

                    //playerManager.lockOnTransform.position = playerManager.DefaultlockOnTransform.position;
                }
            }
            else if (Physics.Raycast(inputHander.CriticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
            {
                CharacterManager enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                PlayerDamageCollider rightWeapon = weaponSlotManager.righthandDamgeCollider;

                if (enemyCharacterManger != null && enemyCharacterManger.canBeRiposted)
                {
                    playerManager.isInvulerable = true;
                    playerManager.lockOnTransform.position = enemyCharacterManger.riposteCollider.CriticalDamageStandPosition.position;

                    Vector3 rotationDirection = playerManager.lockOnTransform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.lockOnTransform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.lockOnTransform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.lockOnTransform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiple * rightWeapon.currentDamageWeapon;
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;
                    Debug.Log(criticalDamage);

                    // 🎲 Randomized animation index
                    int riposteIndex = UnityEngine.Random.Range(0, 4); // Generates 0 to 3

                    string riposteAnim = "Riposte0" + riposteIndex;
                    string ripostedAnim = "Riposted0" + riposteIndex;

                    animatorHander.PlayTargetAnimation(riposteAnim, true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation(ripostedAnim, true);
                }
            }
        }

        public void AttemptInstantRiposte()
        {
            RaycastHit hit;

            if (Physics.Raycast(inputHander.CriticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
            {
                CharacterManager enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                PlayerDamageCollider rightWeapon = weaponSlotManager.righthandDamgeCollider;

                if (enemyCharacterManger != null && enemyCharacterManger.canBeRiposted)
                {
                    playerManager.isInvulerable = true;
                    playerManager.lockOnTransform.position = enemyCharacterManger.riposteCollider.CriticalDamageStandPosition.position;

                    Vector3 rotationDirection = playerManager.lockOnTransform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.lockOnTransform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.lockOnTransform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.lockOnTransform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiple * rightWeapon.currentDamageWeapon;
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;
                    Debug.Log(criticalDamage);

                    // 🎲 Randomized animation index
                    int riposteIndex = UnityEngine.Random.Range(0, 4); // Generates 0 to 3

                    string riposteAnim = "StandRiposte0" + riposteIndex;
                    string ripostedAnim = "StandRiposted0" + riposteIndex;

                    animatorHander.PlayTargetAnimation(riposteAnim, true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation(ripostedAnim, true);
                }
            }

        }
    }
}


