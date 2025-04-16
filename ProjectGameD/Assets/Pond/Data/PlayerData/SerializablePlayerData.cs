using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    [System.Serializable]
    public class SerializablePlayerData
    {
        public int healthLevel;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel;
        public float maxStamina;
        public float currentStamina;

        public WeaponItem[] weaponsInRightHandSlots;
        public WeaponItem[] weaponsInLeftHandSlots;
        public List<WeaponItem> weaponInventory;
    }

}