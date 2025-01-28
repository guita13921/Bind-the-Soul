using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSensor : MonoBehaviour
{
    [Header("Range Sensor Settings")]
    [SerializeField] private float minRange = 5f;  // Minimum range (outside melee range)
    [SerializeField] private float maxRange = 20f; // Maximum range for ranged attacks
    [SerializeField] private LayerMask detectionLayer; // LayerMask to filter for player detection

    [SerializeField] private Transform player;

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
    /// Checks if the player is within the range sensor range.
    /// </summary>
    /// <returns>True if the player is within range, otherwise false.</returns>
    public bool IsPlayerInRange()
    {
        if (player == null) return false;

        // Check distance between this object and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer > minRange && distanceToPlayer <= maxRange;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection range in the editor for visualization
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    public bool IsPlayerOutOfRange()
    {
        if (player == null) return false;

        // Check distance between this object and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer > maxRange;
    }

}
