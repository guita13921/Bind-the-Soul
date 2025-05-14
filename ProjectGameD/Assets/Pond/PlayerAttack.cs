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


        public EnemyManager enemyCharacterManger;
        public PlayerDamageCollider rightWeapon;

        [Header("Attack Animations")]
        string OH_Light_Attack_1 = "OH_Light_Attack_1";
        string OH_Light_Attack_2 = "OH_Light_Attack_2";
        string OH_Light_Attack_3 = "OH_Light_Attack_3";
        string OH_Light_Attack_4 = "OH_Light_Attack_4";
        string OH_Light_Attack_5 = "OH_Light_Attack_5";
        string OH_Heavy_Attack_1 = "OH_Heavy_Attack_1";
        string OH_Shield_Attack_1 = "OH_Shield_Attack_1";

        string weapon_art = "Weapon_Art";
        string Weapon_Art_Upper = "Weapon_Art_Upper";

        [Header("Throw knife")]
        int maxKnifeCharges = 1;
        public int currentKnifeCharges = 1;

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

        public void HandleArtAction() //Parry
        {
            if (playerInventory.leftWeapon.weaponType == WeaponType.Shield)
            {
                PerformLTWeaponArt(playerInventory.leftWeapon);
            }
            else if (playerInventory.leftWeapon.weaponType == WeaponType.Dagger
                    && (playerManager.cameraHandler.currentLockOnTarget.lockOnTransform != null))
            {
                PerformLTWeaponArt(playerInventory.leftWeapon);
            }
            else
            {
                return;
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
                    lastAttack_current = OH_Light_Attack_4;
                    StartCoroutine(HandleLightLastAttack(lastAttack_current));
                    OnComboFinisherExecuted();

                }

                if (lastAttack == OH_Light_Attack_4)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(OH_Light_Attack_5, true);
                    lastAttack_current = OH_Light_Attack_4;
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

                if (weaponSlotManager.leftHandDamgeCollider != null) weaponSlotManager.leftHandDamgeCollider.currentDamageWeapon = Mathf.RoundToInt(damage);

                if (weaponSlotManager.leftHandSlot.currentWeaponItem.weaponType == WeaponType.Dagger)
                {
                    animatorHander.PlayTargetAnimation(Weapon_Art_Upper, false);
                }
                else
                {
                    animatorHander.PlayTargetAnimation(weapon_art, true);
                }
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
            transform.TransformDirection(Vector3.forward), out hit, 0.7f, backStabLayer))
            {
                enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<EnemyManager>();
                rightWeapon = weaponSlotManager.righthandDamgeCollider;
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
                    criticalDamage = CheckApexDrive(playerManager, criticalDamage);
                    CheckHungeringDrive(playerManager, criticalDamage);
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;

                    animatorHander.anim.SetBool("IsInvulnerable", true);
                    animatorHander.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Back Stabed", true);
                    enemyCharacterManger.canBeRiposted = false;

                    //playerManager.lockOnTransform.position = playerManager.DefaultlockOnTransform.position;
                }
            }
            else if (Physics.Raycast(inputHander.CriticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
            {
                enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<EnemyManager>();
                rightWeapon = weaponSlotManager.righthandDamgeCollider;

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
                    criticalDamage = CheckApexDrive(playerManager, criticalDamage);
                    CheckHungeringDrive(playerManager, criticalDamage);
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;
                    Debug.Log(criticalDamage);

                    // 🎲 Randomized animation index
                    int riposteIndex = UnityEngine.Random.Range(0, 4); // Generates 0 to 3

                    string riposteAnim = "Riposte0" + riposteIndex;
                    string ripostedAnim = "Riposted0" + riposteIndex;

                    animatorHander.anim.SetBool("IsInvulnerable", true);
                    animatorHander.PlayTargetAnimation(riposteAnim, true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation(ripostedAnim, true);
                    enemyCharacterManger.canBeRiposted = false;
                }
            }
        }

        public void AttemptInstantRiposte()
        {
            RaycastHit hit;

            if (Physics.Raycast(inputHander.CriticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
            {
                enemyCharacterManger = hit.transform.gameObject.GetComponentInParent<EnemyManager>();
                rightWeapon = weaponSlotManager.righthandDamgeCollider;

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
                    criticalDamage = CheckApexDrive(playerManager, criticalDamage);
                    CheckHungeringDrive(playerManager, criticalDamage);
                    enemyCharacterManger.pendingCriticalDamage = criticalDamage;
                    //Debug.Log(criticalDamage);

                    // 🎲 Randomized animation index
                    int riposteIndex = UnityEngine.Random.Range(0, 4); // Generates 0 to 3

                    string riposteAnim = "StandRiposte0" + riposteIndex;
                    string ripostedAnim = "StandRiposted0" + riposteIndex;

                    animatorHander.anim.SetBool("IsInvulnerable", true);
                    animatorHander.PlayTargetAnimation(riposteAnim, true);
                    enemyCharacterManger.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation(ripostedAnim, true);
                    enemyCharacterManger.canBeRiposted = false;
                }
            }

        }

        public void PerformThrowingKnife()
        {
            if (currentKnifeCharges <= 0) return;

            if (playerManager.cameraHandler.currentLockOnTarget.lockOnTransform == null) return;

            GameObject knife = Instantiate(
                playerInventory.leftWeapon.ThrowingModelPrefab,
                playerManager.weaponSlotManager.leftHandSlot.transform.position,
                Quaternion.identity);

            Rigidbody rb = knife.GetComponent<Rigidbody>();

            Vector3 direction = (playerManager.cameraHandler.currentLockOnTarget.lockOnTransform.position - playerManager.weaponSlotManager.leftHandSlot.transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion localXRotation = Quaternion.AngleAxis(90f, Vector3.right);
            knife.transform.rotation = lookRotation * localXRotation;

            rb.velocity = direction * 20f;

            Knife knifeScript = knife.GetComponent<Knife>();
            knifeScript.target = playerManager.lockOnTransform;
            knifeScript.ownerAttack = this; // So we can give the charge back later

            currentKnifeCharges--;
        }

        public void RecoverKnifeCharge()
        {
            if (currentKnifeCharges < maxKnifeCharges)
            {
                currentKnifeCharges++;
            }
        }

        private void OnComboFinisherExecuted()
        {
            if (playerManager.playerData.echoQuickstep)
            {
                float staminaToRestore = playerStats.maxStamina * (0.05f + (0.05f * playerStats.playerData.echoQuickstepLevel));
                playerStats.RestoreStamina(staminaToRestore);
                Debug.Log($"Echo of the Quickstep: Restored {staminaToRestore} stamina after combo finisher.");
            }
        }

        public int CheckApexDrive(PlayerManager playerManager, int baseDamage)
        {
            if (playerManager.playerData.echoApexDrive)
            {
                float bonus = 1f + (playerManager.playerData.echoApexDriveLevel * 0.25f);
                float tempDamage;
                tempDamage = baseDamage * bonus;
                Debug.Log($"Echo of the Apex Drive: {bonus * 100}% {tempDamage} damage applied to staggered target.");
                return (int)tempDamage;
            }
            else
            {
                return baseDamage;
            }
        }

        public void CheckHungeringDrive(PlayerManager playerManager, int baseDamage)
        {
            float finalDamage = baseDamage;

            if (playerManager.playerData.echoHungeringDrive)
            {
                float lifestealPercent = 0.02f * playerManager.playerData.echoHungeringDriveLevel;
                float healAmount = finalDamage * lifestealPercent;

                playerManager.playerStats.RestoreHealth((int)healAmount);
                Debug.Log($"Echo of Hungering Drive: Restored {healAmount} HP from special attack.");
            }
        }
    }
}



