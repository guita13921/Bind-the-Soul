using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float Health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public void SetState(int IN_Health){
        Health = IN_Health;
        maxHealth = IN_Health;
        currentHealth = IN_Health;
    }

    public void CalculateDamage(float playerWeaponDamage)
    {
        currentHealth -= playerWeaponDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed maxHealth
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth); // Ensure health is within valid range
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
