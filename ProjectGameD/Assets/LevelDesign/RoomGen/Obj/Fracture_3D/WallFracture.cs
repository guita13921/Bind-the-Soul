using UnityEngine;
using System.Collections;

public class WallFracture : MonoBehaviour
{
    public GameObject fracturedWallPrefab; // Assign fractured version of the wall
    public float explosionForce = 500f;
    public float explosionRadius = 3f;
    public Vector3 forceDirectionA = Vector3.forward; // Force direction for one side
    public Vector3 forceDirectionB = Vector3.back; // Force direction for opposite side
    public int hitThreshold = 5; // Number of hits required to break
    public float shakeAmount = 0.05f; // Shake intensity
    public float shakeDuration = 0.2f; // Duration of each shake
    public float hitCooldown = 0.2f; // Prevents multiple hits at once

    private int hitCount = 0;
    private bool isDestroyed = false;
    private bool canBeHit = true;
    private Vector3 originalPosition;
    private Vector3 lastHitPosition; // Stores last impact point

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed || !canBeHit) return;

        if (other.CompareTag("PlayerSword"))
        {
            hitCount++;
            canBeHit = false;
            StartCoroutine(HitCooldown());

            lastHitPosition = other.transform.position; // Store last hit position

            // Shake effect
            StartCoroutine(ShakeWall());

            if (hitCount >= hitThreshold)
            {
                FractureWall();
            }
        }
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(hitCooldown);
        canBeHit = true;
    }

    IEnumerator ShakeWall()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            transform.position = originalPosition + (Random.insideUnitSphere * shakeAmount);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }

    void FractureWall()
    {
        isDestroyed = true;

        // Determine which side was hit
        Vector3 hitDirection = (lastHitPosition - transform.position).normalized;
        Vector3 chosenForceDirection = Vector3.zero;

        if (Vector3.Dot(hitDirection, transform.right) > 0)
        {
            // Right side hit
            chosenForceDirection = forceDirectionA;
        }
        else
        {
            // Left side hit
            chosenForceDirection = forceDirectionB;
        }

        // Spawn fractured wall
        GameObject fracturedWall = Instantiate(fracturedWallPrefab, transform.position, transform.rotation);

        // Apply force to each fractured piece
        foreach (Rigidbody rb in fracturedWall.GetComponentsInChildren<Rigidbody>())
        {
            if (rb != null)
            {
                Vector3 direction = (rb.transform.position - lastHitPosition).normalized;
                rb.AddForce((direction + chosenForceDirection) * explosionForce);
            }
        }

        // Destroy original wall
        Destroy(gameObject);
    }
}
