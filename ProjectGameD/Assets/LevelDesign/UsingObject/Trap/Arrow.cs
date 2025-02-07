using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject hitEffectPrefab; // Assign your hit effect prefab in the Inspector

    [SerializeField]
    private float arrowDamage = 10f; // Damage dealt by the arrow

    private void OnTriggerEnter(Collider other)
    {
        // Check if the arrow hits an object with the "Player" or "Enemy" tag
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Instantiate the hit effect at the arrow's position
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Attempt to get the EnemyHealth component from the collided object
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Calculate damage on the enemy
                enemyHealth.CalculateDamageOld(arrowDamage);
                Debug.Log(
                    $"{other.gameObject.name} took {arrowDamage} damage. Remaining health: {enemyHealth.GetCurrentHealth()}"
                );
            }

            // Destroy the arrow after collision
            Destroy(gameObject);
        }
    }
}
