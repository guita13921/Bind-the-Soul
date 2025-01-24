using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSensor : MonoBehaviour
{
    [Header("Melee Sensor Settings")]
    [SerializeField] private float detectionRadius = 5f; // The radius of the melee range
    [SerializeField] private LayerMask detectionLayer;    // LayerMask to filter for player detection
    [SerializeField, Range(-1f, 1f)] private float frontDetectionThreshold = 0.5f; // Threshold to consider the player in front

    private Transform player;

    private void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
        }
    }

    /// <summary>
    /// Checks if the player is within the melee sensor range.
    /// </summary>
    /// <returns>True if the player is within range, otherwise false.</returns>
    public bool IsPlayerInRange()
    {
        if (player == null) return false;

        // Check distance between this object and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer <= detectionRadius;
    }

    /// <summary>
    /// Checks if the player is in front of the boss.
    /// </summary>
    /// <returns>True if the player is in front of the boss, otherwise false.</returns>
    public bool IsPlayerInFront()
    {
        if (player == null) return false;

        // Get the direction vector to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the dot product between the forward direction and the direction to the player
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        // If the dot product is above the threshold, the player is in front
        return dotProduct >= frontDetectionThreshold;
    }

    /// <summary>
    /// Checks if the player is behind the boss.
    /// </summary>
    /// <returns>True if the player is behind the boss, otherwise false.</returns>
    public bool IsPlayerBehind()
    {
        if (player == null) return false;

        // Get the direction vector to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the dot product between the forward direction and the direction to the player
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        // If the dot product is below the negative threshold, the player is behind
        return dotProduct <= -frontDetectionThreshold;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw the forward direction of the object
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * detectionRadius);

        // Draw the backwards direction for reference
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * detectionRadius);
    }
}
