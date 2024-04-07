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
    Health PaHP;

    // Start is called before the first frame update
    void Start()
    {
        PaHP = GetComponentInParent<Health>();
        maxHealth = PaHP.maxHealth;
        health = PaHP.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health != PaHP.currentHealth)
        {
            health = PaHP.currentHealth;
        }
        if (healthslider.value != health)
        {
            healthslider.value = health;
        }

        if (healthslider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(
                easeHealthSlider.value,
                health,
                lerpSpeed * Time.deltaTime
            );
        }
    }
}
