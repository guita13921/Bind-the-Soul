using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArrowCollision : MonoBehaviour
{
    public GameObject hitEffectPrefab; // Assign your hit effect prefab in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Instantiate the hit effect at the arrow's position
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the arrow game object
            Destroy(gameObject);
        }
    }
}
