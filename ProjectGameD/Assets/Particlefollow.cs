using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particlefollow : MonoBehaviour
{
    public Transform target; // The target you want the particle to follow
    private Vector3 offset; // To store the initial position offset

    private void Start()
    {
        // Calculate the initial offset between the particle and the target
        offset = transform.position - target.position;
    }

    private void Update()
    {
        // Set the particle's position to follow the target but keep the offset
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;

        // Ensure the particle system doesn't rotate with the target by resetting rotation
        transform.rotation = Quaternion.identity; // Keeps the rotation fixed
    }
}
