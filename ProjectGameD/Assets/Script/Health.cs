using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    EnemyAIPartol enemy;

    void Start()
    {
        currentHealth =1;
        currentHealth = maxHealth;
        enemy = GetComponent<EnemyAIPartol>();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            enemy.isDead = true;
        }


    }
}
