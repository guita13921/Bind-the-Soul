using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : MonoBehaviour
{
    public float speed = 1.0f; // Speed of movement
    public float changeInterval = 2.0f; // Time before changing direction

    private Vector3 targetPosition;

    void Start()
    {
        SetNewTarget();
        InvokeRepeating(nameof(SetNewTarget), changeInterval, changeInterval);
    }

    void Update()
    {
        // Move towards the target position smoothly
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * speed
        );
    }

    void SetNewTarget()
    {
        // Generate a random position near the current position
        float range = 2.0f; // Adjust range for larger/smaller movement
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range, range),
            Random.Range(-range, range)
        );

        targetPosition = transform.position + randomOffset;
    }
}
