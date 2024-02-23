using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    [SerializeField] EnemyAI2 enemy;

    void Start()
    {
        currentHealth =1;
        currentHealth = maxHealth;
        enemy = GetComponent<EnemyAI2>();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            enemy.isDead = true;
        }


    }
}
