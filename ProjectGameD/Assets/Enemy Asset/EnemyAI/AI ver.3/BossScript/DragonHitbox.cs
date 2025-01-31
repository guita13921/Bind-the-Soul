using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonHitbox : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth; // Reference to the main health script
    [SerializeField] private float damageMultiplier; // Default to 100%

    private static bool damageAppliedThisFrame = false; // Ensure only one damage instance per frame

    private void OnTriggerEnter(Collider other)
    {
        if (damageAppliedThisFrame) return; // Prevent further damage in this frame

        if (other.CompareTag("PlayerSword")) // Ensure the object is a weapon
        {
            PlayerWeapon weapon = other.GetComponent<PlayerWeapon>(); // Assuming you have a script for weapon damage
            if (weapon != null)
            {
                float finalDamage = weapon.damage * damageMultiplier;
                enemyHealth.CalculateDamage(finalDamage);
                
                damageAppliedThisFrame = true; // Mark that damage has been applied for this frame
            }
        }
    }

    private void LateUpdate()
    {
        damageAppliedThisFrame = false; // Reset the flag at the end of the frame
    }
}
