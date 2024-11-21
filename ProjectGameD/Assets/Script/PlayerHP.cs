using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    // Start is called before the first frame update
    float HP;
    TMP_Text HPText;

    public Health health;
    void Start()
    {
        HPText=GetComponent<TMP_Text>();
        HPText.text = health.currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
                HPText.text = "HP:"+health.currentHealth.ToString();
                if(health.currentHealth <0){
                    HPText.text = "HP:0";
                }
    }
}
