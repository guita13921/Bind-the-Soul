using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuffReader : MonoBehaviour
{
    public CharacterData characterData;
    public Transform buffUIPanel; // Parent object for UI buttons
    public GameObject buffButtonPrefab; // Prefab for buff buttons

    private List<Buff> buffs;

    public Animator animator;
    public EventSystem eventSystem;

    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem != null)
        {
            Debug.Log("EventSystem found: " + eventSystem.name);
        }

        InitializeBuffs();
        ShowRandomAvailableBuffs();
    }

    public void ResetBuff()
    {
        InitializeBuffs();
        ShowRandomAvailableBuffs();
    }

    void InitializeBuffs()
    {
        buffs = new List<Buff>
        {
            new Buff(
                data => $"Vampirism (LV.{data.vampirism} -> LV.{data.vampirism + 1})",
                data => data.vampirism < 3,
                data => data.vampirism++,
                data => "Restores player health for every attack",
                data => "Normal Attack"
            ),
            new Buff(
                data => $"Barrier (LV.{data.barrierLV} -> LV.{data.barrierLV + 1})",
                data => data.barrierLV < 3,
                data => data.barrierLV++,
                data =>
                    data.barrierLV == 0
                        ? "Receive a barrier that can absorb one attack"
                        : "The barrier can absorb one additional attack",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Increase damage (LV.{data.normalAttackDamageUpLV} -> LV.{data.normalAttackDamageUpLV + 1})",
                data => data.normalAttackDamageUpLV < 3,
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
                data =>
                    $"Greatsword Slash (LV.{data.specialAttack} -> LV.{data.specialAttack + 1})",
                data => data.specialAttack == 0,
                data => data.specialAttack = 1,
                data =>
                    "Transforms the special attack into a powerful forward strike with a large sword",
                data => "Speical attack"
            ),
            new Buff(
                data => "Crimson Spin",
                data => data.specialAttack == 0,
                data => data.specialAttack = 2,
                data => "Transforms the special attack into a powerful spin attack",
                data => "Speical attack"
            ),
            new Buff(
                data => "Light bullet",
                data => data.specialAttack == 0,
                data => data.specialAttack = 3,
                data =>
                    "Transforms the special attack into a light bullet that automatically attacks",
                data => "Speical attack"
            ),
            new Buff(
                data => getAddSpecialName(data.specialAttack, 1).name,
                data => !data.specialAdd1 && data.specialAttack != 0,
                data => data.specialAdd1 = true,
                data => getAddSpecialName(data.specialAttack, 1).description,
                data => "Speical attack"
            ),
            new Buff(
                data => getAddSpecialName(data.specialAttack, 2).name,
                data => !data.specialAdd2 && data.specialAttack != 0,
                data => data.specialAdd2 = true,
                data => getAddSpecialName(data.specialAttack, 2).description,
                data => "Speical attack"
            ),
            new Buff(
                data =>
                    $"Increase damage (LV.{data.SpecialDamageUpLV} -> LV.{data.SpecialDamageUpLV + 1})",
                data => data.SpecialDamageUpLV < 3,
                data => data.SpecialDamageUpLV++,
                data => "Increases the damage of special attacks",
                data => "Speical attack"
            ),
            // new Buff(
            //     data => "Bigger",
            //     data => data.specialBiggerLV < 3,
            //     data => data.specialBiggerLV++,
            //     data => "Increases the size of special attacks",
            //     data => "Speical attack"
            // ),
            new Buff(
                data =>
                    $"Increase Damage (LV.{data.skillDamageUpLV} -> LV.{data.skillDamageUpLV + 1})",
                data => data.skillDamageUpLV < 3,
                data => data.skillDamageUpLV++,
                data => "Increases the damage of skill ",
                data => "Skill"
            ),
            new Buff(
                data => $"Slow (LV.{data.skillSlowEnemyLV} -> LV.{data.skillSlowEnemyLV + 1})",
                data => data.skillSlowEnemyLV < 3,
                data => data.skillSlowEnemyLV++,
                data => "skill now slow enemy",
                data => "Skill"
            ),
            new Buff(
                data =>
                    $"Poision (LV.{data.skillPoisionEnemyLV} -> LV.{data.skillPoisionEnemyLV + 1})",
                data => data.skillPoisionEnemyLV < 3,
                data => data.skillPoisionEnemyLV++,
                data => "skill now poison enemy",
                data => "Skill"
            ),
        };
    }

    private (string name, string description) getAddSpecialName(int specailattack, int type)
    {
        switch (specailattack)
        {
            case 1:
                if (type == 1)
                    return (
                        "Crack explosion",
                        "The Greatsword Slash now creates a crack that explodes on impact"
                    );
                else
                    return ("Triple attack", "The special attack will triple in power");
            case 2:
                if (type == 1)
                    return ("Red aura", "Creates an aura around you that damages enemies in range");
                else
                    return ("Blue aura", "Creates an aura around you that slow enemies in range");
            case 3:
                if (type == 1)
                    return (
                        "Light explode",
                        "The light bullet now has a 20% chance to explode on impact with enemies."
                    );
                else
                    return ("Light strike", "The light bullet is transformed into a light strike");
            default:
                return ("Unknown Attack", "Description not available");
        }
    }

    void ShowRandomAvailableBuffs()
    {
        // Clear existing buttons
        foreach (Transform child in buffUIPanel)
        {
            if (child.transform.name != "Reset")
                Destroy(child.gameObject);
        }

        // Filter available buffs
        List<Buff> availableBuffs = buffs.FindAll(buff => buff.isAvailable(characterData));

        RectTransform panelRect = buffUIPanel.GetComponent<RectTransform>();
        float panelHeight = panelRect.rect.height; // Get the height of the panel
        float buttonHeight = panelHeight / 4; // Divide space for consistent spacing

        float[] yOffsets = { 0, buttonHeight, -buttonHeight };

        for (int i = 0; i < 3 && availableBuffs.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, availableBuffs.Count);
            Buff selectedBuff = availableBuffs[index];
            availableBuffs.RemoveAt(index);

            // Create a button for the buff
            GameObject buttonObj = Instantiate(buffButtonPrefab, buffUIPanel);
            Transform childd = buttonObj.transform.GetChild(0);

            // Adjust position relative to the panel
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

            buttonRect.anchoredPosition = new Vector2(0, yOffsets[i]);

            Button button = childd.GetComponent<Button>();

            Transform child1 = childd.transform.GetChild(0);
            TMPro.TextMeshProUGUI MainText = child1.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child2 = childd.transform.GetChild(1);
            TMPro.TextMeshProUGUI DescriptionText = child2.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child3 = childd.transform.GetChild(2);
            TMPro.TextMeshProUGUI TypeText = child3.GetComponent<TMPro.TextMeshProUGUI>();

            if (MainText != null)
            {
                MainText.text = selectedBuff.name(characterData);
                DescriptionText.text = selectedBuff.description(characterData);
                TypeText.text = selectedBuff.type(characterData);
            }
            else
            {
                Debug.LogWarning("Text component not found on the buff button prefab.");
            }

            // Add click event to apply the buff
            button.onClick.AddListener(() =>
            {
                selectedBuff.applyEffect(characterData);
                animator.Play("Out");

                Debug.Log($"{selectedBuff.name} applied!");
                eventSystem.enabled = false;
            });
        }
    }

    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                characterData.ResetToDefault();
            }
            // Reload the current active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
