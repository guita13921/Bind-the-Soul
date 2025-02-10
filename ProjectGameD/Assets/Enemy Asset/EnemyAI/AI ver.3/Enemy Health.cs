using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //[SerializeField]
    //private float Health;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;
    EnemyWeapon enemyWeapon1;
    public Rigidbody rb;

    void Start()
    {
        enemyWeapon1 = GetComponentInChildren<EnemyWeapon>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (currentHealth <= 0)
        {
            // Freeze the Rigidbody by setting its velocity and angular velocity to zero, and freezing the position and rotation
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }


    public void SetState(int IN_Health)
    {
        //Health = IN_Health; it seem that you never used this value
        maxHealth = IN_Health;
        currentHealth = IN_Health;
    }

    public void CalculateDamage(float playerWeaponDamage, bool isQK, bool Q3_reduceDamage)
    {
        currentHealth -= playerWeaponDamage;
        if (Q3_reduceDamage && isQK)
        {
            enemyWeapon1.reduceDamageTimer = 5f;
            enemyWeapon1.reducedDamage = true;
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
    }

    public void CalculateDamageOld(float playerWeaponDamage)
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
