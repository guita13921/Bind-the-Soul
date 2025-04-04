using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Ref.")]
    [SerializeField] EnemyAI3 enemyAI3;
    [SerializeField] EnemyRange02 enemyRange02;

    [Header("HealthSetting")]
    [SerializeField] public float maxHealth;
    [SerializeField] public  float currentHealth;
    EnemyWeapon enemyWeapon1;

    private bool NoDamage;

    private bool death = false;
    [SerializeField] GameObject  deathSound;
    [SerializeField] bool warrior = false;
    private bool secondDeath = false;

    void Start()
    {
        enemyAI3 = GetComponent<EnemyAI3>();
        enemyRange02 = GetComponent<EnemyRange02>();
        enemyWeapon1 = GetComponentInChildren<EnemyWeapon>();
    }
void Update()
{
    if (currentHealth <= 0)
    {
        if (!death)
        {
            if (deathSound != null)
                Instantiate(deathSound);
            death = true;
        }
        else if (warrior) // Second phase logic
        {
            if (!secondDeath)
            {
                if (deathSound != null)
                    Instantiate(deathSound);
                secondDeath = true;
            }
        }
    }
}


    public void SetState(float IN_Health)
    {
        //Health = IN_Health; it seem that you never used this value
        maxHealth = IN_Health;
        currentHealth = IN_Health;
    }

    public void CalculateDamage(float playerWeaponDamage, bool isQK, bool Q3_reduceDamage)
    {
        if(enemyAI3 != null){
            NoDamage = enemyAI3.GetIsSpawning();
        }

        if(enemyRange02 != null){
            NoDamage = enemyRange02.GetIsSpawning();
        }

        if(!NoDamage){
            currentHealth -= playerWeaponDamage;
            if (Q3_reduceDamage && isQK)
            {
                enemyWeapon1.reduceDamageTimer = 5f;
                enemyWeapon1.reducedDamage = true;
            }
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        }
    }

    public void CalculateDamageTrap(float playerWeaponDamage)
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

    public void RestoreFullHealth(){
        currentHealth = maxHealth;
    }
}
