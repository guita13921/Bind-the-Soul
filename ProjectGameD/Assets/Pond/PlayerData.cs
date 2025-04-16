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

        private void OnEnable()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentHealth = maxHealth;
            currentStamina = maxStamina;
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