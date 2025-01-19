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

    //public Animator animator;

    void Start()
    {
        InitializeBuffs();
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
        float buttonHeight = 300; // Adjust this value as needed
        float spacing = 20f; // Add spacing between buttons
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
            Button button = childd.GetComponent<Button>();

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

            // Add click event to apply the buff
            button.onClick.AddListener(() =>
            {
                buff.applyEffect(characterData);
                Debug.Log($"{buff.name(characterData)} applied!");
            });
        }
    }

    void InitializeBuffs()
    {
        buffs = new List<Buff>
        {
            new Buff(
                data => $"Vampirism (LV.{data.vampirism}",
                data => data.vampirism > 0,
                data => data.vampirism++,
                data => "Restores player health for every attack",
                data => "Normal Attack"
            ),
            new Buff(
                data => $"Barrier (LV.{data.barrierLV}",
                data => data.barrierLV > 0,
                data => data.barrierLV++,
                data =>
                    data.barrierLV == 0
                        ? "Receive a barrier that can absorb one attack"
                        : "The barrier can absorb one additional attack",
                data => "Passive"
            ),
            new Buff(
                data => $"Increase damage (LV.{data.normalAttackDamageUpLV}",
                data => data.normalAttackDamageUpLV > 0,
                data => data.normalAttackDamageUpLV++,
                data => "Increases the damage of normal attacks",
                data => "Normal Attack"
            ),
            new Buff(
                data => "Forth attack",
                data => data.forthNormalAttack,
                data => data.forthNormalAttack = true,
                data => "Adds a new attack to the normal attack combo",
                data => "Normal Attack"
            ),
            new Buff(
                data => $"Greatsword Slash (LV.{data.specialAttack}",
                data => data.specialAttack == 1,
                data => data.specialAttack = 1,
                data =>
                    "Transforms the special attack into a powerful forward strike with a large sword",
                data => "Speical attack"
            ),
            new Buff(
                data => "Crimson Spin",
                data => data.specialAttack == 2,
                data => data.specialAttack = 2,
                data => "Transforms the special attack into a powerful spin attack",
                data => "Speical attack"
            ),
            new Buff(
                data => "Light bullet",
                data => data.specialAttack == 3,
                data => data.specialAttack = 3,
                data =>
                    "Transforms the special attack into a light bullet that automatically attacks",
                data => "Speical attack"
            ),
            new Buff(
                data => "Triple attack",
                data => data.specialAdd1,
                data => data.specialAttack = 3,
                data => "The special attack will triple in power",
                data => "Speical attack"
            ),
            new Buff(
                data => "Explosion",
                data => data.specialAdd2,
                data => data.specialAttack = 3,
                data => "The special attack will also explod",
                data => "Speical attack"
            ),
            new Buff(
                data => "Red Aura",
                data => data.specialAdd3,
                data => data.specialAttack = 3,
                data => "Creates an aura around you that damages enemies in range",
                data => "Speical attack"
            ),
            new Buff(
                data => "Blue Aura",
                data => data.specialAdd3,
                data => data.specialAttack = 4,
                data => "Creates an aura around you that slow enemies in range",
                data => "Speical attack"
            ),
            new Buff(
                data => "Light explode",
                data => data.specialAdd3,
                data => data.specialAttack = 5,
                data => "THe Special attack has a 20% chance to explode on impact with enemies.",
                data => "Speical attack"
            ),
            new Buff(
                data => "Light strike",
                data => data.specialAdd3,
                data => data.specialAttack = 6,
                data => "Add a additional light strike that random on enemy",
                data => "Speical attack"
            ),
            new Buff(
                data => $"Increase damage (LV.{data.SpecialDamageUpLV}",
                data => data.SpecialDamageUpLV > 0,
                data => data.SpecialDamageUpLV++,
                data => "Increases the damage of special attacks",
                data => "Speical attack"
            ),
            new Buff(
                data => $"Increase Damage (LV.{data.skillDamageUpLV}",
                data => data.skillDamageUpLV > 0,
                data => data.skillDamageUpLV++,
                data => "Increases the damage of skill ",
                data => "Skill"
            ),
            new Buff(
                data => $"Slow (LV.{data.skillSlowEnemyLV}",
                data => data.skillSlowEnemyLV > 0,
                data => data.skillSlowEnemyLV++,
                data => "skill now slow enemy",
                data => "Skill"
            ),
            new Buff(
                data => $"Poision (LV.{data.skillPoisionEnemyLV}",
                data => data.skillPoisionEnemyLV > 0,
                data => data.skillPoisionEnemyLV++,
                data => "skill now poison enemy",
                data => "Skill"
            ),
        };
    }
}
