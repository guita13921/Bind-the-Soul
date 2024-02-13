using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthslider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(healthslider.value != health){
            healthslider.value = health;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            takeDamage(20);
        }
        
        if(healthslider.value != easeHealthSlider.value){
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed*Time.deltaTime);
        }
    }

    void takeDamage(float damage){
        health -= damage;
    }
}
