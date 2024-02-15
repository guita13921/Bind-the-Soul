using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    float damage; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    EnemyAIPartol enemy;

    void Start()
    {
        currentHealth = maxHealth;
        enemy = GetComponent<EnemyAIPartol>();
        damage = 10; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            enemy.isDead = true;
        }
        else { }
    }
}
