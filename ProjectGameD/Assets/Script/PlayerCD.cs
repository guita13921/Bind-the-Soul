using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerCD : MonoBehaviour


{

    float CD;
    TMP_Text CDText;

    // Start is called before the first frame update
    void Start()
    {
        CDText = GetComponent<TMP_Text>();
        CDText.text ="ready";
    }

    // Update is called once per frame
    public void CooldownText(float cooldown){
        

      float roundedCooldown = Mathf.Round(cooldown * 10f) / 10f;

        // Display the cooldown value with only one digit
        CDText.text = roundedCooldown.ToString("F1"); // "F1" format specifier ensures one digit after the decimal point



    }

    public void cooldownReady(){
        CDText.text = "ready";
    }
}
