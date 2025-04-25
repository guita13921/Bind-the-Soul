using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerDataSaver : MonoBehaviour
    {
        private PlayerInventory playerInventory;
        private PlayerStats playerStats;
        public PlayerData playerData; // Assign in Inspector

        private void Awake()
        {
            playerInventory = GetComponent<PlayerInventory>();

            // Optional: auto-load ScriptableObject from Resources
            // playerData = Resources.Load<PlayerData>("YourPlayerDataAssetName");
        }

        private void OnDestroy()
        {
            SavePlayerData();
        }

        private void OnApplicationQuit()
        {
            SavePlayerData();
        }

        private void SavePlayerData()
        {
            if (playerInventory == null || playerData == null)
            {
                Debug.LogWarning("PlayerInventory or PlayerData reference missing in PlayerDataSaver.");
                return;
            }

            // Save health & stamina
            playerData.currentHealth = playerStats.currentHealth;
            playerData.currentStamina = playerStats.currentStamina;

            // Save weapon slots
            playerData.weaponsInRightHandSlots = playerInventory.weaponsInRightHandSlots;
            playerData.weaponsInLeftHandSlots = playerInventory.weaponsInLeftHandSlots;

            // Save inventory
            playerData.weaponInventory = new List<WeaponItem>(playerInventory.weaponInventory);

            // Save gold
            playerData.goldCount = playerStats.goldCount;

            // Save power-up flags
            playerData.bloodPactDamageModify = playerStats.playerData.bloodPactDamageModify;
            playerData.hasMomentum = playerStats.playerData.hasMomentum;
            playerData.momentumActive = playerStats.playerData.momentumActive;

            playerData.flatDamageBonus = playerStats.flatDamageBonus;
            playerData.StaminaRegenBonus = playerStats.StaminaRegenBonus;

            Debug.Log("PlayerData has been updated from PlayerInventory and PlayerStats.");
        }
    }
}
