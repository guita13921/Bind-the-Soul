using UnityEngine;

public class OffmapParticleController : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public ParticleSystem particleEffect; // Renamed from 'particleSystem' to avoid conflicts
    public float growthSpeed = 1f; // Speed of radius growth
    private float maxRadius;
    
    void Start()
    {
        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();

        if (particleEffect == null)
            particleEffect = GetComponent<ParticleSystem>();

        if (sphereCollider == null || particleEffect == null)
        {
            Debug.LogError("Missing SphereCollider or ParticleSystem!");
            enabled = false;
            return;
        }

        // Get the max possible radius from ParticleSystem bounds
        maxRadius = particleEffect.main.startSize.constantMax * 2f;
    }

    void Update()
    {
        if (particleEffect.isPlaying)
        {
            sphereCollider.radius = Mathf.MoveTowards(sphereCollider.radius, maxRadius, growthSpeed * Time.deltaTime);
        }
    }
}
