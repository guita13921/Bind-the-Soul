using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCD : MonoBehaviour
{
    float CD;
    TMP_Text CDText;

    // Start is called before the first frame update
    void Start()
    {
        CDText = GetComponent<TMP_Text>();
        CDText.text = "ready";
    }

    // Update is called once per frame
    public void CooldownText(float cooldown)
    {
        float roundedCooldown = Mathf.Round(cooldown * 10f) / 10f;

        // Display the cooldown value with only one digit
        CDText.text = roundedCooldown.ToString("F1");
    }

    public void cooldownReady()
    {
        CDText.text = "ready";
    }
}
