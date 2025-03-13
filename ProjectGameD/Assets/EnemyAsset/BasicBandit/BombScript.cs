using System.Collections;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] private float countdownTime = 3f; // Countdown before explosion if it hits the ground
    [SerializeField] private GameObject explosionEffect; // Explosion VFX prefab
    [SerializeField] private float explosionRadius = 5f; // Radius of the explosion
    [SerializeField] private float explosionForce = 500f; // Explosion force

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Explode(); // Explode instantly if it hits the player
        }
        else
        {
            Debug.Log("HIT GROUND");
            StartCoroutine(CountdownToExplosion()); // Start countdown if it hits the ground or other objects
        }
    }

    private IEnumerator CountdownToExplosion()
    {
        yield return new WaitForSeconds(countdownTime);

        if (!hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // Instantiate explosion effect
        if (explosionEffect)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f); 
        }

        // Apply explosion force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Damage player or other damageable objects
            if (nearbyObject.CompareTag("Player"))
            {
                // Call player damage function here, e.g., nearbyObject.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
            }
        }

        // Destroy the bomb object itself after explosion
        Destroy(gameObject); 
    }
}
