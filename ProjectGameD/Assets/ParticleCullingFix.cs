using UnityEngine;

public class ParticleCullingFix : MonoBehaviour
{
    void Start()
    {
        // Get the ParticleSystemRenderer
        var particleSystem = GetComponent<ParticleSystem>();
        var renderer = particleSystem.GetComponent<Renderer>();

        // Manually expand the bounds
        renderer.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 1000)); // Adjust size as needed
    }
}
