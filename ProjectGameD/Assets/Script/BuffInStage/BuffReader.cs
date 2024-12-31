using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuffReader : MonoBehaviour
{
    public CharacterData characterData;
    public Transform buffUIPanel; // Parent object for UI buttons
    public GameObject buffButtonPrefab; // Prefab for buff buttons

    private List<Buff> buffs;

    void Start()
    {
        InitializeBuffs();
        ShowRandomAvailableBuffs();
    }

    void InitializeBuffs()
    {
        buffs = new List<Buff>
        {
            new Buff(data => "Vampirism", data => !data.vampirism, data => data.vampirism = true),
            new Buff(
                data => "Barrier Level +1",
                data => data.barrierLV < 3,
                data => data.barrierLV++
            ),
            new Buff(
                data => "Normal Attack Damage Up",
                data => data.normalAttackDamageUpLV < 3,
                data => data.normalAttackDamageUpLV++
            ),
            new Buff(
                data => "Normal Attack can perform a forth attack",
                data => !data.forthNormalAttack,
                data => data.forthNormalAttack = true
            ),
            new Buff(
                data => "Change special attack to Big sword",
                data => data.specialAttack == 0,
                data => data.specialAttack = 1
            ),
            new Buff(
                data => "Change special attack to Red Spin",
                data => data.specialAttack == 0,
                data => data.specialAttack = 2
            ),
            new Buff(
                data => "Change special attack to Light bullet",
                data => data.specialAttack == 0,
                data => data.specialAttack = 3
            ),
            new Buff(
                data => $"Change special attack to {getAddSpecialName((data.specialAttack), 1)}",
                data => !data.specialAdd1 && data.specialAttack != 0,
                data => data.specialAdd1 = true
            ),
            new Buff(
                data => $"Change special attack to {getAddSpecialName((data.specialAttack), 2)}",
                data => !data.specialAdd2 && data.specialAttack != 0,
                data => data.specialAdd2 = true
            ),
            new Buff(
                data => "SpecialDamageUpLV",
                data => data.SpecialDamageUpLV < 3,
                data => data.SpecialDamageUpLV++
            ),
            new Buff(
                data => "specialBiggerLV",
                data => data.specialBiggerLV < 3,
                data => data.specialBiggerLV++
            ),
            new Buff(
                data => "skillDamageUpLV",
                data => data.skillDamageUpLV < 3,
                data => data.skillDamageUpLV++
            ),
            new Buff(
                data => "skillSlowEnemyLV",
                data => data.skillSlowEnemyLV < 3,
                data => data.skillSlowEnemyLV++
            ),
            new Buff(
                data => "skillPoisionEnemyLV",
                data => data.skillPoisionEnemyLV < 3,
                data => data.skillPoisionEnemyLV++
            ),
        };
    }

    private string getAddSpecialName(int specailattack, int type)
    {
        switch (specailattack)
        {
            case 1:

                return type == 1 ? "Big sword now is explode" : "triple the sword";
            case 2:
                return type == 1 ? "aura to damage an enemy" : "aura to slow enemy";
            case 3:
                return type == 1
                    ? "light bullet now has a 20% percent chance if it hit eneymy it will explde"
                    : "change light bullet to light strik";
            default:
                return "Unknown Attack";
        }
    }

    void ShowRandomAvailableBuffs()
    {
        // Clear existing buttons
        foreach (Transform child in buffUIPanel)
        {
            Destroy(child.gameObject);
        }

        // Filter available buffs
        List<Buff> availableBuffs = buffs.FindAll(buff => buff.isAvailable(characterData));

        RectTransform panelRect = buffUIPanel.GetComponent<RectTransform>();
        float panelHeight = panelRect.rect.height; // Get the height of the panel
        float buttonHeight = panelHeight / 4; // Divide space for consistent spacing (3 buttons + some padding)

        // Adjust the starting position to be closer to the top
        float startY = panelHeight / 2 - buttonHeight / 2; // Start from the top, adjusted for button height

        // Shuffle and pick 3 random buffs
        for (int i = 0; i < 3 && availableBuffs.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, availableBuffs.Count);
            Buff selectedBuff = availableBuffs[index];
            availableBuffs.RemoveAt(index);

            // Create a button for the buff
            GameObject buttonObj = Instantiate(buffButtonPrefab, buffUIPanel);

            // Adjust position relative to the panel
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f); // Top-center anchor
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f); // Top-center anchor
            buttonRect.pivot = new Vector2(0.5f, 0.5f); // Top-center pivot

            // Position each button starting from the top, spaced evenly
            buttonRect.anchoredPosition = new Vector2(0, startY - buttonHeight * i); // Space buttons evenly

            Button button = buttonObj.GetComponent<Button>();
            TMPro.TextMeshProUGUI buttonText =
                buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = selectedBuff.name(characterData);
            }
            else
            {
                Debug.LogWarning("Text component not found on the buff button prefab.");
            }

            // Add click event to apply the buff
            button.onClick.AddListener(() =>
            {
                selectedBuff.applyEffect(characterData);
                Debug.Log($"{selectedBuff.name} applied!");
                ShowRandomAvailableBuffs(); // Refresh buff selection
            });
        }
    }

    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload the current active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
