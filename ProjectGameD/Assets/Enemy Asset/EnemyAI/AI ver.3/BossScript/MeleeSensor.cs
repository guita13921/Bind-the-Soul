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

    public bool IsPlayerInRange()
    {
        if (player == null) return false;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= detectionRadius;
    }

    public bool IsPlayerInFront()
    {
        if (player == null) return false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        return dotProduct >= frontDetectionThreshold;
    }

    public bool IsPlayerBehind()
    {
        if (player == null) return false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        return dotProduct <= -frontDetectionThreshold;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * detectionRadius);
    }
}
