using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float Health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    void Start()
    {
        maxHealth = Health;
        currentHealth = maxHealth;
    }

    public void CalculateDamage(float playerWeaponDamage){
        currentHealth -= playerWeaponDamage;
    }

    public float GetMaxHealth(){
        return maxHealth;
    }

    public float GetCurrentHealth(){
        return currentHealth;
    }
}
