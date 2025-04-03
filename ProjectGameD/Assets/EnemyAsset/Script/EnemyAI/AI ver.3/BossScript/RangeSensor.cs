using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSensor : MonoBehaviour
{
    [Header("Range Sensor Settings")]
    [SerializeField] private float minRange = 5f;  // Minimum range (outside melee range)
    [SerializeField] private float maxRange = 20f; // Maximum range for ranged attacks
    [SerializeField] private LayerMask detectionLayer; // LayerMask to filter for player detection
    [SerializeField, Range(-1f, 1f)] private float frontDetectionThreshold = 0.5f; // Threshold to consider the player in front

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


    public bool IsPlayerInRange()
    {
        if (player == null) return false;

        // Check distance between this object and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer > minRange && distanceToPlayer <= maxRange;
    }

    public bool IsPlayerInFront()
    {
        if (player == null) return false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        return dotProduct >= frontDetectionThreshold;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    public bool IsPlayerOutOfRange()
    {
        if (player == null) return false;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer > maxRange;
    }

}
