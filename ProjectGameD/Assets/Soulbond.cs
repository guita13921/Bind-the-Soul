using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soulbond : MonoBehaviour
{
    public CharacterData characterData;
    public Transform buffUIPanel; // Parent object for UI buttons
    public GameObject buffButtonPrefab; // Prefab for buff buttons

    private List<Buff> buffs;
    public List<Buff> buffForActive;

    //public Animator animator;
    [SerializeField]
    bool isPauseMenu = false;

    void Start()
    {
        InitializeBuffs();
        buffForActive = new List<Buff>();
        ShowAllAvailableBuffs();
    }

    void ShowAllAvailableBuffs()
    {
        // Clear existing buttons
        foreach (Transform child in buffUIPanel)
        {
            if (child.transform.name != "Reset")
                Destroy(child.gameObject);
        }

        // Filter available buffs
        List<Buff> availableBuffs = buffs.FindAll(buff => buff.isAvailable(characterData));

        // Calculate button spacing
        RectTransform panelRect = buffUIPanel.GetComponent<RectTransform>();
        float panelHeight = panelRect.rect.height;
        float buttonHeight = 0; // Adjust this value as needed
        float spacing = 100; // Add spacing between buttons
        float currentYOffset = -spacing; // Start just below the top of the panel

        foreach (var buff in availableBuffs)
        {
            // Create a button for the buff
            GameObject buttonObj = Instantiate(buffButtonPrefab, buffUIPanel);
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

            // Adjust position relative to the panel
            buttonRect.anchoredPosition = new Vector2(0, currentYOffset);
            currentYOffset -= buttonHeight + spacing;

            Transform childd = buttonObj.transform.GetChild(0);
            Toggle button = childd.GetComponent<Toggle>();

            // Set button text
            Transform child1 = childd.transform.GetChild(0);
            TMPro.TextMeshProUGUI MainText = child1.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child2 = childd.transform.GetChild(1);
            TMPro.TextMeshProUGUI DescriptionText = child2.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child3 = childd.transform.GetChild(2);
            TMPro.TextMeshProUGUI TypeText = child3.GetComponent<TMPro.TextMeshProUGUI>();

            if (MainText != null)
            {
                MainText.text = buff.name(characterData);
                DescriptionText.text = buff.description(characterData);
                TypeText.text = buff.type(characterData);
            }
            else
            {
                Debug.LogWarning("Text component not found on the buff button prefab.");
            }
            if (!isPauseMenu)
            {
                button.onValueChanged.AddListener(isOn =>
                {
                    HandleToggleChangeWithLimit(isOn, buttonObj, buff, button);
                });
            }
        }
    }

    public Transform selectedBuffPanel;
    public int maxSelectableBuffs = 0; // Maximum number of selectable buffs
    private int currentSelectedBuffs = 0; // Counter for selected buffs

    void HandleToggleChangeWithLimit(
        bool isOn,
        GameObject buttonObj,
        Buff buff,
        Toggle currentToggle
    )
    {
        maxSelectableBuffs = characterData.deathCount * 2;
        if (isOn)
        {
            if (currentSelectedBuffs < maxSelectableBuffs)
            {
                buttonObj.transform.SetParent(selectedBuffPanel, false);

                buffForActive.Add(buff);
                currentSelectedBuffs++;
                Debug.Log(
                    $"Buff {buff.name(characterData)} added to selected panel. Current count: {currentSelectedBuffs}"
                );

                if (currentSelectedBuffs >= maxSelectableBuffs)
                {
                    DisableAllUnselectedToggles();
                }
            }
            else
            {
                currentToggle.isOn = false;
                Debug.LogWarning($"Cannot select more than {maxSelectableBuffs} buffs.");
            }
        }
        else
        {
            buttonObj.transform.SetParent(buffUIPanel, false);
            currentSelectedBuffs--;
            buffForActive.Remove(buff);
            Debug.Log(
                $"Buff {buff.name(characterData)} removed from selected panel. Current count: {currentSelectedBuffs}"
            );

            EnableAllToggles();
        }
    }

    void DisableAllUnselectedToggles()
    {
        foreach (Transform child in buffUIPanel)
        {
            Toggle toggle = child.GetComponentInChildren<Toggle>();
            if (toggle != null && !toggle.isOn)
            {
                toggle.interactable = false;
            }
        }
    }

    void EnableAllToggles()
    {
        foreach (Transform child in buffUIPanel)
        {
            Toggle toggle = child.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.interactable = true;
            }
        }
    }

    void InitializeBuffs()
    {
        buffs = new List<Buff>
        {
            new Buff(
                data => $"Vampirism (LV.{data.vampirism})",
                data => data.vampirism != 0,
                data => data.vampirism++,
                data => $"Restores player health by {2 + data.vampirism} for every attack",
                data => "Vampirism"
            ),
            new Buff(
                data => $"Iron Body (LV.{data.reduceIncomeDamage})",
                data => data.reduceIncomeDamage != 0,
                data => data.reduceIncomeDamage++,
                data => $"Reduce incoming damage by {5 * (data.reduceIncomeDamage)}%",
                data => "Iron Body"
            ),
            new Buff(
                data => $"Super Special (LV.{data.specialLV})",
                data => data.specialLV != 0,
                data => data.specialLV++,
                data => "Upgrade the special attack to a stronger version",
                data => "Super Special"
            ),
            new Buff(
                data => $"Threshold Revival (LV.{data.healToThreshold})",
                data => data.healToThreshold != 0,
                data => data.healToThreshold++,
                data =>
                    $"Heal the player's health points to the health threshold of {20 * (data.healToThreshold)}% after entering a new stage",
                data => "Threshold Revival"
            ),
            new Buff(
                data => $"Acceleration (LV.{data.QKReduceCooldown})",
                data => data.QKReduceCooldown != 0,
                data => data.QKReduceCooldown++,
                data =>
                    $"Reduce special attack and skill cooldown by {data.QKReduceCooldown } second",
                data => "Acceleration"
            ),
            new Buff(
                data => $"Overgrowth (LV.{data.maxHPIncrease})",
                data => data.maxHPIncrease != 0,
                data => data.HpCalculation(),
                data => $"Increase max health points by {1000 * (data.maxHPIncrease )}",
                data => "Overgrowth"
            ),
            new Buff(
                data => $"Lethal Strike (LV.{data.normalAttackCrit})",
                data => data.normalAttackCrit != 0,
                data => data.normalAttackCrit++,
                data =>
                    $"Normal attack now has a {10 * (data.normalAttackCrit )}% chance to triple the damage",
                data => "Lethal Strike"
            ),
            /*
            new Buff(
                data => $"Electric Trail (LV.{data.dashExplode})",
                data => data.dashExplode != 0,
                data => data.dashExplode++,
                data =>
                    data.dashExplode == 0
                        ? $"When Dash left an electric bomb behind"
                        : "Increase electric bomb damage",
                data => "Passive"
            ),*/
            new Buff(
                data => $"Superspeed (LV.{data.moveFaster})",
                data => data.moveFaster != 0,
                data => data.moveFaster++,
                data => $"Move faster",
                data => "Superspeed"
            ),
            new Buff(
                data => $"Swift Step (LV.{data.ReduceDashCooldown} )",
                data => data.ReduceDashCooldown != 0,
                data => data.ReduceDashCooldown++,
                data => $"Reduce dash cooldown by {15 * (data.ReduceDashCooldown )}%",
                data => "Swift Step"
            ),
            new Buff(
                data => $"Unyielding spirit (LV.{data.reduceIncomeDamageDependOnHP})",
                data => data.reduceIncomeDamageDependOnHP != 0,
                data => data.reduceIncomeDamageDependOnHP++,
                data =>
                    $"If HP is less than 25%, reduce incoming damage by {15 * (data.reduceIncomeDamageDependOnHP )}%",
                data => "Unyielding spirit"
            ),
            new Buff(
                data => $"Last Stand (LV.{data.addDamageDependOnHP})",
                data => data.addDamageDependOnHP != 0,
                data => data.addDamageDependOnHP++,
                data =>
                    $"If HP is less than 25%, increase damage by {15 * (data.addDamageDependOnHP )}%",
                data => "Last Stand"
            ),
            new Buff(
                data => $"Evolution: Red Blade",
                data => data.QSkillType == 1,
                data => data.QSkillType = 1,
                data => $"Change skill to Red Blade",
                data => "Evolution: Red Blade"
            ),
            new Buff(
                data => $"Evolution: Triple Bullet",
                data => data.QSkillType == 2,
                data => data.QSkillType = 2,
                data => $"Triple the bullets that follow the enemy",
                data => "Evolution: Homing Bullet"
            ),
            new Buff(
                data => $"Evolution: Aura",
                data => data.QSkillType == 3,
                data => data.QSkillType = 3,
                data => $"Change skill to an area of effect damage",
                data => "Evolution: Aura"
            ),
            new Buff(
                data => $"Killing Strike",
                data => data.Q1_QKDamageUp == true,
                data => data.Q1_QKDamageUp = true,
                data => $"Increase skill and special attack damage by 25%",
                data => "Killing Strike"
            ),
            new Buff(
                data => $"Maximum Output",
                data => data.Q1_QKFasterWider == true,
                data => data.Q1_QKFasterWider = true,
                data => $"Increase the size of special attack and skill",
                data => "Maximum Output"
            ),
            new Buff(
                data => $"Bloodlust",
                data => data.Q1_QKKillEnemyDamageUp == true,
                data => data.Q1_QKKillEnemyDamageUp = true,
                data => $"Increase the size of special attack and skill",
                data => "Bloodlust"
            ),
            new Buff(
                data => $"Super Lethal",
                data => data.Q2_QKCrit == true,
                data => data.Q2_QKCrit = true,
                data => $"Special attack and skill have a 20% chance to deal triple damage",
                data => "Super Lethal"
            ),
            new Buff(
                data => $"Stackable",
                data => data.Q2_QKStackable == true,
                data => data.Q2_QKStackable = true,
                data => $"Special skill and skill can now be stored with a maximum of 2",
                data => "Evo:Skill"
            ),
            new Buff(
                data => $"Little Bee",
                data => data.Q2_SmallBullet == true,
                data => data.Q2_SmallBullet = true,
                data => $"There is a small bullet after the attack",
                data => "Little Bee"
            ),
            new Buff(
                data => $"Hand breaker",
                data => data.Q3_QKWeak == true,
                data => data.Q3_QKWeak = true,
                data => $"Enemies hit by a special attack or skill will have their attack reduced",
                data => "Hand breaker"
            ),
            new Buff(
                data => $"Froze feet",
                data => data.Q3_QKSlow == true,
                data => data.Q3_QKSlow = true,
                data => $"SLow down enemies hit by a special attack or skill",
                data => "Froze feet"
            ),
            new Buff(
                data => $"Barrier",
                data => data.Q3_Barrier == true,
                data => data.Q3_Barrier = true,
                data =>
                    $"Become invincible for a short amount of time after using a special attack",
                data => "Barrier"
            ),
        };
    }
}
