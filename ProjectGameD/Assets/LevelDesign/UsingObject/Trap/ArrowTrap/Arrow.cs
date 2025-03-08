using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollision : MonoBehaviour
{

    [SerializeField] private float arrowDamageEnemy = 10f; // Damage dealt by the arrow

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.CalculateDamageTrap(arrowDamageEnemy);
            }
            Destroy(gameObject);
        }
    }
}
