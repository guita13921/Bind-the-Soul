using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target; // Player or object to follow
    [SerializeField] private float smoothTime = 0.3f; // Smoothing factor for camera movement
    [SerializeField] private Vector3 offset = new Vector3(8.5f, 8.5f, -10); // Camera's offset relative to the player
    private Vector3 _currentVelocity = Vector3.zero;

    [Header("Object Fading")]
    public ObjFadeing _fader; // Reference to object fading script (optional)

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _currentVelocity,
            smoothTime
        );

        // Make the camera always look at the target
        transform.LookAt(target);

        // Check for obstacles between the camera and the player and handle fading
        HandleObstacleFading();
    }

    /// <summary>
    /// Checks for obstacles between the camera and the player and applies fading to objects in the way.
    /// </summary>
    private void HandleObstacleFading()
    {
        if (target == null) return;

        // Direction from the camera to the player
        Vector3 directionToPlayer = target.position - transform.position;

        // Perform a raycast to detect obstacles between the camera and the player
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, directionToPlayer.magnitude))
        {
            if (hit.collider != null && hit.collider.gameObject != target.gameObject)
            {
                // If an obstacle is detected, try to apply fading
                _fader = hit.collider.GetComponent<ObjFadeing>();
                if (_fader != null)
                {
                    _fader.timeRemaining = 0.1f; // Set fade timer
                    _fader.DoFade = true;       // Trigger fade
                }
            }
        }
    }
}
