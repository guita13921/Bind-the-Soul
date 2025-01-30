using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonHitbox : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth; // Reference to the main health script
    [SerializeField] private float damageMultiplier = 1.0f; // Default to 100%
    
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // Prevent multiple hits in one frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword")) // Ensure the object is a weapon
        {
            if (hitObjects.Contains(other.gameObject)) return; // Prevent multiple hits
            hitObjects.Add(other.gameObject);

            PlayerWeapon weapon = other.GetComponent<PlayerWeapon>(); // Assuming you have a script for weapon damage
            if (weapon != null)
            {
                float finalDamage = weapon.damage * damageMultiplier;
                enemyHealth.CalculateDamage(finalDamage);
            }
        }
    }

    private void LateUpdate()
    {
        hitObjects.Clear(); // Reset hit list after frame ends
    }
}
