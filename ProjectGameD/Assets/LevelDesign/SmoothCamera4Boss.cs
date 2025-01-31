using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera4Boss : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform player; // Player's transform
    [SerializeField] private Transform boss;   // Boss's transform
    [SerializeField] private Transform pivot;  // Pivot (parent of the camera)

    [SerializeField] private float smoothTime = 0.3f; // Smoothing factor for camera movement
    [SerializeField] private Camera camera;          // Reference to the Camera component

    [Header("Zoom Settings")]
    [SerializeField] private float minSize = 5f;     // Minimum orthographic size (for orthographic camera) or FOV (for perspective camera)
    [SerializeField] private float maxSize = 15f;    // Maximum orthographic size or FOV
    [SerializeField] private float zoomSpeed = 0.3f; // Smoothing speed for zooming

    private Vector3 _currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null || boss == null || pivot == null || camera == null) return;

        // Calculate the midpoint between the player and the boss
        Vector3 midpoint = (player.position + boss.position) / 2f;
        
        // Add local axis Z offset
        midpoint += pivot.forward * -30f;

        // Move the pivot smoothly to the midpoint
        pivot.position = Vector3.SmoothDamp(
            pivot.position,
            midpoint,
            ref _currentVelocity,
            smoothTime
        );

        // Calculate the distance between the player and the boss
        float playerBossDistance = Vector3.Distance(player.position, boss.position);

        // Adjust the camera size based on the distance
        float targetSize = Mathf.Clamp(playerBossDistance, minSize, maxSize);

        // Smoothly interpolate the camera's size (orthographic or FOV)
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
}
