using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class Selectbufftext : MonoBehaviour
{
    public TextMeshProUGUI buffText; // Reference to the TMP text component
    public CharacterData characterData;
    void Start()
    {
        // Automatically find the TextMeshPro component if not assigned
        if (buffText == null)
        {
            buffText = GetComponent<TextMeshProUGUI>();
            SetBuffText();
        }

    }

    // Example function to set text
    public void SetBuffText( )
    {
        if (buffText != null)
        {
            buffText.text = $"Selected Buffs (Max:{characterData.deathCount *2})";
        }
        else
        {
            Debug.LogError("buffText is not assigned!");
        }
    }
}
