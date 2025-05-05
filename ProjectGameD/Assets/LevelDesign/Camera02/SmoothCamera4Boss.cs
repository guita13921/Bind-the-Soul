using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera4Boss : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform player; // Player's transform
    [SerializeField] private Transform boss;   // Boss's transform (can be null if no boss exists)
    [SerializeField] private Transform pivot;  // Pivot (parent of the camera)

    [SerializeField] private float smoothTime = 0.3f; // Smoothing factor for camera movement
    [SerializeField] private new Camera camera;          // Reference to the Camera component

    [Header("Zoom Settings")]
    [SerializeField] private float minSize = 5f;     // Minimum orthographic size (for orthographic camera) or FOV (for perspective camera)
    [SerializeField] private float maxSize = 15f;    // Maximum orthographic size or FOV
    [SerializeField] private float zoomSpeed = 0.3f; // Smoothing speed for zooming

    [Header("Object Fading")]
    public ObjFadeing _fader; // Reference to object fading script (optional)


    private Vector3 _currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        // If player or pivot is missing, exit the function
        if (player == null || pivot == null || camera == null) return;

        Vector3 targetPosition;
        HandleObstacleFading();
        // Calculate the midpoint: If the boss exists, use the midpoint between player and boss
        if (boss != null)
        {
            // Calculate the midpoint between the player and the boss
            targetPosition = (player.position + boss.position) / 2f;

            // Add local axis Z offset
            targetPosition += pivot.forward * -30f;

            // Calculate the distance between the player and the boss
            float playerBossDistance = Vector3.Distance(player.position, boss.position);

            // Adjust the camera size based on the distance
            float targetSize = Mathf.Clamp(playerBossDistance, minSize, maxSize);

            // Smoothly interpolate the camera's size (orthographic or FOV)
            AdjustCameraSize(targetSize);
        }
        else
        {
            // If no boss, focus only on the player
            targetPosition = player.position;

            // Add a default offset to ensure the camera doesn't stick directly to the player
            targetPosition += pivot.forward * -30f;

            // Set the camera size to the minimum size when no boss is present
            AdjustCameraSize(minSize);
        }

        // Move the pivot smoothly to the target position
        pivot.position = Vector3.SmoothDamp(
            pivot.position,
            targetPosition,
            ref _currentVelocity,
            smoothTime
        );
    }

    private void AdjustCameraSize(float targetSize)
    {
        if (camera.orthographic)
        {
            // Adjust orthographic size for an orthographic camera
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
        else
        {
            // Adjust field of view (FOV) for a perspective camera
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetSize, Time.deltaTime * zoomSpeed);
        }
    }

    private void HandleObstacleFading()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float maxDistance = directionToPlayer.magnitude; // Raycast range

        // Perform a raycast that hits all obstacles between the camera and the player
        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer.normalized, maxDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != player.gameObject)
            {
                ObjFadeing fader = hit.collider.GetComponent<ObjFadeing>();
                if (fader != null)
                {
                    fader.timeRemaining = 0.1f;
                    fader.DoFade = true;
                }
            }
        }

        // Optional: Handle small objects near the player
        Collider[] nearPlayerObjects = Physics.OverlapSphere(player.position, 1.0f); // Small range near the player
        foreach (Collider col in nearPlayerObjects)
        {
            if (col.gameObject != player.gameObject)
            {
                ObjFadeing fader = col.GetComponent<ObjFadeing>();
                if (fader != null)
                {
                    fader.timeRemaining = 0.1f;
                    fader.DoFade = true;
                }
            }
        }
    }

}
