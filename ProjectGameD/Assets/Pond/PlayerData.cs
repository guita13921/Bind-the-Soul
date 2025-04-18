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

        [Header("BloodPactPowerUp")]
        public bool bloodPactDamageModify;

        [Header("MomentumPowerUp")]
        public bool momentumActive;
        public bool hasMomentum;

        [Header("Damage Bonus")]
        public int flatDamageBonus;

        [Header("Stamina Bonus")]
        public float StaminaRegenBonus;

        private void OnEnable()
        {
            healthLevel = 10;
            staminaLevel = 10;
            maxHealth = SetMaxHealthFromHealthLevel();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            bloodPactDamageModify = false;
            hasMomentum = false;
            momentumActive = false;
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
        }
    }
}