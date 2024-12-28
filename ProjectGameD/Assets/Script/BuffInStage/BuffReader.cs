using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            new Buff("Vampirism", data => !data.vampirism, data => data.vampirism = true),
            new Buff("Barrier Level +1", data => data.barrierLV < 5, data => data.barrierLV++),
            new Buff(
                "Normal Attack Damage Up",
                data => data.normalAttackDamageUpLV < 3,
                data => data.normalAttackDamageUpLV++
            ),
            new Buff(
                "Special Bigger LV +1",
                data => data.specialBiggerLV < 5,
                data => data.specialBiggerLV++
            ),
            // Add more buffs here
        };
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

        // Shuffle and pick 3 random buffs
        System.Random random = new System.Random();
        for (int i = 0; i < 3 && availableBuffs.Count > 0; i++)
        {
            int index = random.Next(availableBuffs.Count);
            Buff selectedBuff = availableBuffs[index];
            availableBuffs.RemoveAt(index);

            // Create a button for the buff
            GameObject buttonObj = Instantiate(buffButtonPrefab, buffUIPanel);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            if (buttonText != null)
                buttonText.text = selectedBuff.name;

            // Add click event to apply the buff
            button.onClick.AddListener(() =>
            {
                selectedBuff.applyEffect(characterData);
                Debug.Log($"{selectedBuff.name} applied!");
                ShowRandomAvailableBuffs(); // Refresh buff selection
            });
        }
    }
}
