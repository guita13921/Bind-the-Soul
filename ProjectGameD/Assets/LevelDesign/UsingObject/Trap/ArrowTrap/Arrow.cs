using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{

    [SerializeField] private float arrowDamageEnemy = 10f; // Damage dealt by the arrow

    private void OnTriggerEnter(Collider other)
    {
        // Check if the arrow hits an object with the "Player" or "Enemy" tag
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {

            // Attempt to get the EnemyHealth component from the collided object
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Calculate damage on the enemy
                enemyHealth.CalculateDamageOld(arrowDamageEnemy);
            }

            // Destroy the arrow after collision
            Destroy(gameObject);
        }
    }
}
