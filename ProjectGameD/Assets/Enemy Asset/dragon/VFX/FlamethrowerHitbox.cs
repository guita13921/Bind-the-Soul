using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerHitbox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damagePerSecond = 10f; // Damage per second while player is in the hitbox
    [SerializeField] private float hitboxDuration = 6f; // Duration of the hitbox (should match the VFX duration)
    [SerializeField] private LayerMask playerLayer; // Layer mask for the player

    private bool isActive = false;
    private float activeTime = 0f;
    private BoxCollider boxCollider; // Reference to the BoxCollider component

    private void Awake()
    {
        // Get the BoxCollider component
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider component is missing on the FlamethrowerHitbox GameObject!");
        }
    }

    private void Update()
    {
        if (isActive)
        {
            activeTime += Time.deltaTime;

            // Check if the hitbox duration has expired
            if (activeTime >= hitboxDuration)
            {
                DeactivateHitbox();
            }

            // Apply damage to the player if they are within the hitbox
            Collider[] hitPlayers = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, transform.rotation, playerLayer);
            foreach (Collider player in hitPlayers)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    public void ActivateHitbox()
    {
        isActive = true;
        activeTime = 0f;
        Debug.Log("Flamethrower Hitbox Activated!");
    }

    public void DeactivateHitbox()
    {
        isActive = false;
        Debug.Log("Flamethrower Hitbox Deactivated!");
    }

}