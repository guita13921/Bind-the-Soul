using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthslider;
    public Slider easeHealthSlider;
    public float maxHealth;
    public float health;
    private float lerpSpeed = 5f;
    EnemyHealth PaHP;

    // Start is called before the first frame update
    void Start()
    {
        PaHP = GetComponentInParent<EnemyHealth>();
        maxHealth = PaHP.GetMaxHealth();
        health = PaHP.GetCurrentHealth();

        healthslider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health != PaHP.GetCurrentHealth())
        {
            health = PaHP.GetCurrentHealth();
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
