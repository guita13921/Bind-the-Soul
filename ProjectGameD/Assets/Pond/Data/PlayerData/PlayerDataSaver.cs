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
            playerStats = GetComponent<PlayerStats>();

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

            // Save core passive bonuses
            playerData.flatDamageBonus = playerStats.flatDamageBonus;
            playerData.StaminaRegenBonus = playerStats.StaminaRegenBonus;

            // Save Duelist set bonuses
            playerData.echoCrimsonEdge = playerStats.playerData.echoCrimsonEdge;
            playerData.echoCrimsonEdgeLevel = playerStats.playerData.echoCrimsonEdgeLevel;

            playerData.echoSilverGuard = playerStats.playerData.echoSilverGuard;
            playerData.echoSilverGuardLevel = playerStats.playerData.echoSilverGuardLevel;

            playerData.echoFocusedWill = playerStats.playerData.echoFocusedWill;
            playerData.echoFocusedWillLevel = playerStats.playerData.echoFocusedWillLevel;

            playerData.echoRazorTiming = playerStats.playerData.echoRazorTiming;
            playerData.echoRazorTimingLevel = playerStats.playerData.echoRazorTimingLevel;

            playerData.echoReturningFlow = playerStats.playerData.echoReturningFlow;
            playerData.echoReturningFlowLevel = playerStats.playerData.echoReturningFlowLevel;

            playerData.echoResoluteMind = playerStats.playerData.echoResoluteMind;
            playerData.echoResoluteMindLevel = playerStats.playerData.echoResoluteMindLevel;

            playerData.isFreeDodgeActive = playerStats.playerData.isFreeDodgeActive;
            playerData.duelistSet2Bonus = playerStats.playerData.duelistSet2Bonus;
            playerData.critAttacksRemaining = playerStats.playerData.critAttacksRemaining;
            playerData.duelistSet4Bonus = playerStats.playerData.duelistSet4Bonus;
            playerData.duelistSetCurse = playerStats.playerData.duelistSetCurse;
            playerData.duelistSet4CurseDamageMultiplier = playerStats.playerData.duelistSet4CurseDamageMultiplier;
            playerData.duelistSet4CurseStaminaDamageMultiplie = playerStats.playerData.duelistSet4CurseStaminaDamageMultiplie;

            // Save Titan set bonuses
            playerData.echoStoneborn = playerStats.playerData.echoStoneborn;
            playerData.echoStonebornLevel = playerStats.playerData.echoStonebornLevel;

            playerData.echoIronMaw = playerStats.playerData.echoIronMaw;
            playerData.echoIronMawLevel = playerStats.playerData.echoIronMawLevel;

            playerData.echoUnbrokenWall = playerStats.playerData.echoUnbrokenWall;
            playerData.echoAnchorstep = playerStats.playerData.echoAnchorstep;
            playerData.echoAnvilborn = playerStats.playerData.echoAnvilborn;
            playerData.echoAnvilbornLevel = playerStats.playerData.echoAnvilbornLevel;

            // Save Phantom set bonuses
            playerData.echoFlickerFang = playerStats.playerData.echoFlickerFang;
            playerData.echoFlickerFangLevel = playerStats.playerData.echoFlickerFangLevel;

            playerData.echoQuickstep = playerStats.playerData.echoQuickstep;
            playerData.echoQuickstepLevel = playerStats.playerData.echoQuickstepLevel;

            playerData.echoBladeRush = playerStats.playerData.echoBladeRush;
            playerData.echoBladeRushLevel = playerStats.playerData.echoBladeRushLevel;

            // Save Predator set bonuses
            playerData.echoBloodhound = playerStats.playerData.echoBloodhound;
            playerData.echoBloodhoundLevel = playerStats.playerData.echoBloodhoundLevel;

            playerData.echoFirstFang = playerStats.playerData.echoFirstFang;
            playerData.echoFirstFangLevel = playerStats.playerData.echoFirstFangLevel;

            playerData.echoApexDrive = playerStats.playerData.echoApexDrive;
            playerData.echoApexDriveLevel = playerStats.playerData.echoApexDriveLevel;

            playerData.echoHungeringDrive = playerStats.playerData.echoHungeringDrive;
            playerData.echoHungeringDriveLevel = playerStats.playerData.echoHungeringDriveLevel;

            Debug.Log("PlayerData has been updated from PlayerInventory and PlayerStats.");
        }

    }
}
