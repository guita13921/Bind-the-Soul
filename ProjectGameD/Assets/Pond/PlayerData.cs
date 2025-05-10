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
        public bool echoRazorTiming;      // Whether the effect is active
        public int echoRazorTimingLevel;
        public bool echoReturningFlow;
        public int echoReturningFlowLevel;
        public bool isFreeDodgeActive;
        public bool echoResoluteMind;
        public int echoResoluteMindLevel;


        private void OnEnable()
        {
            healthLevel = 20;
            staminaLevel = 10;
            maxHealth = SetMaxHealthFromHealthLevel();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            bloodPactDamageModify = false;
            hasMomentum = false;
            momentumActive = false;

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
