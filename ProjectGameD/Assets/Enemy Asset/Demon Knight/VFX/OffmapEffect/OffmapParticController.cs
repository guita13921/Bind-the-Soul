using UnityEngine;

public class ExpandingRingHitbox : MonoBehaviour
{
    public SphereCollider hitboxCollider;
    public ParticleSystem particleEffect;
    public float growthSpeed = 1f; // Speed at which the ring expands
    public float ringThickness; // How thick the damageable ring is
    public float maxRadius = 5f; // Maximum expansion size

    private float currentRadius = 0f;

    void Start()
    {
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<SphereCollider>();

        if (particleEffect == null)
            particleEffect = GetComponent<ParticleSystem>();

        if (hitboxCollider == null || particleEffect == null)
        {
            Debug.LogError("Missing required components!");
            enabled = false;
            return;
        }

        hitboxCollider.isTrigger = true; // Ensure it's a trigger
        hitboxCollider.radius = 0f; // Start at zero
    }

    void Update()
    {
        if (particleEffect.isPlaying)
        {
            currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, growthSpeed * Time.deltaTime);
            hitboxCollider.radius = currentRadius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // Player is inside the ring but not the center
            if (distance >= currentRadius - ringThickness && distance <= currentRadius)
            {
                Debug.Log("Player hit by expanding ring!");
                // Apply damage to the player here
            }
        }
    }
}
