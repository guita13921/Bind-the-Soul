using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0, 1, 0); // Axis of rotation (default: Y-axis)
    [SerializeField] private float rotationSpeed = 10f; // Speed of rotation (degrees per second)

    private void Update()
    {
        // Rotate the object around the specified axis at the specified speed
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
