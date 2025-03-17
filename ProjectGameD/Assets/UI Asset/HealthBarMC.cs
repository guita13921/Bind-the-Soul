using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarMC : MonoBehaviour
{
    [SerializeField]
    private Image hpBarFill; // Drag the HPBar image here in the Inspector.

    [SerializeField]
    private Health health;

    private void Start() { }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        hpBarFill.fillAmount = Mathf.Lerp(
            hpBarFill.fillAmount,
            health.currentHealth / health.maxHealth,
            Time.deltaTime * 5
        );
    }
}
