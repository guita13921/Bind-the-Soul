using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    [CreateAssetMenu(fileName = "New PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [Header("Health Stats")]
        public int healthLevel;
        public int maxHealth;
        public int currentHealth;

        [Header("Stamina Stats")]
        public int staminaLevel;
        public float maxStamina;
        public float currentStamina;

        [Header("Weapon Slots")]
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2];

        [Header("Inventory")]
        public List<WeaponItem> weaponInventory = new List<WeaponItem>();

        [Header("Gold")]
        public int goldCount;

        [Header("Power-Up Flags")]
        public bool bloodPactDamageModify;
        public bool momentumActive;
        public bool hasMomentum;

        [Header("Passive Bonuses")]
        public int flatDamageBonus;
        public float StaminaRegenBonus;

        [Header("Duelist Set Bonuses & Effects")]
        public bool echoCrimsonEdge;
        public int echoCrimsonEdgeLevel;

        public bool echoSilverGuard;
        public int echoSilverGuardLevel;

        public bool echoFocusedWill;
        public int echoFocusedWillLevel;

        public bool echoRazorTiming;
        public int echoRazorTimingLevel;

        public bool echoReturningFlow;
        public int echoReturningFlowLevel;
        public bool isFreeDodgeActive;

        public bool echoResoluteMind;
        public int echoResoluteMindLevel;

        public bool duelistSet2Bonus;
        public int critAttacksRemaining;

        public bool duelistSet4Bonus = false;
        public bool duelistSetCurse = false;
        public float duelistSet4CurseDamageMultiplier = 1.15f;
        public float duelistSet4CurseStaminaDamageMultiplie = 1.15f;


        [Header("Titan Set Bonuses & Effects")]
        public bool echoStoneborn;
        public int echoStonebornLevel;

        public bool echoIronMaw;
        public int echoIronMawLevel;

        public bool echoUnbrokenWall = false;

        public bool echoAnchorstep = false;

        public bool echoAnvilborn = false;
        public int echoAnvilbornLevel = 0;

        [Header("Phantom Set Bonuses & Effects")]
        public bool echoFlickerFang = false;
        public int echoFlickerFangLevel = 0;

        public bool echoQuickstep = false;
        public int echoQuickstepLevel = 0;

        public bool echoBladeRush = false;
        public int echoBladeRushLevel = 0;

        [Header("Predator Set Bonuses & Effects")]
        public bool echoBloodhound = false;
        public int echoBloodhoundLevel = 0;

        public bool echoFirstFang = false;
        public int echoFirstFangLevel = 0;

        public bool echoApexDrive = false;
        public int echoApexDriveLevel = 0;

        public bool echoHungeringDrive = false;
        public int echoHungeringDriveLevel = 0;


        private void OnEnable()
        {
            healthLevel = 20;
            staminaLevel = 20;
            maxHealth = SetMaxHealthFromHealthLevel();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            bloodPactDamageModify = false;
            hasMomentum = false;
            momentumActive = false;

            //Duelist Set Bonuses & Effects
            echoCrimsonEdge = false;
            echoCrimsonEdgeLevel = 0;
            echoSilverGuard = false;
            echoSilverGuardLevel = 0;
            echoFocusedWill = false;
            echoFocusedWillLevel = 0;
            echoRazorTiming = false;
            echoRazorTimingLevel = 0;
            echoReturningFlow = false;
            echoReturningFlowLevel = 0;
            isFreeDodgeActive = false;
            echoResoluteMind = false;
            echoResoluteMindLevel = 0;
            duelistSet2Bonus = false;
            critAttacksRemaining = 0;
            duelistSet4Bonus = false;
            duelistSetCurse = false;
            duelistSet4CurseStaminaDamageMultiplie = 1f;


            //Titan Set Bonuses & Effects
            echoStoneborn = false;
            echoStonebornLevel = 0;
            echoIronMaw = false;
            echoIronMawLevel = 0;
            echoUnbrokenWall = false;
            echoAnchorstep = false;
            echoAnvilborn = false;
            echoAnvilbornLevel = 0;

            //Phantom Set Bonuses & Effects
            echoFlickerFang = false;
            echoFlickerFangLevel = 0;
            echoQuickstep = false;
            echoQuickstepLevel = 0;
            echoBladeRush = false;
            echoBladeRushLevel = 0;

            //Predator Set Bonuses & Effects
            echoBloodhound = false;
            echoBloodhoundLevel = 0;
            echoFirstFang = false;
            echoFirstFangLevel = 0;
            echoApexDrive = false;
            echoApexDriveLevel = 0;
            echoHungeringDrive = false;
            echoHungeringDriveLevel = 0;




            flatDamageBonus = 0;
            StaminaRegenBonus = 0;

        }

        private int SetMaxHealthFromHealthLevel()
        {
            return healthLevel * 10;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            return staminaLevel * 10f;
        }

        public void Initialize()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            flatDamageBonus = 0;
            StaminaRegenBonus = 0;
        }

    }
}
