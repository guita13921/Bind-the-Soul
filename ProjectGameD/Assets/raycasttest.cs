using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    public float rayDistance = 10f;  // The maximum distance for the raycast
    public LayerMask layerMask;      // Layer mask to filter objects detected by the raycast

    // Update is called once per frame
    void Update()
    {
        // Define the origin (this object's position) and direction (forward direction of the object)
                Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Calculate the endpoint directly
        Vector3 endPoint = origin + direction * rayDistance;

        // Log the endpoint
        Debug.Log("Raycast endpoint: " + endPoint);

        // Draw the full ray length in the Scene view to visualize the raycast path
        Debug.DrawLine(origin, endPoint, Color.blue);
    }
}
