using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteVFX : MonoBehaviour
{
     private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        float duration = mainModule.duration ; // Calculate total duration
        Invoke("DestroyGameObject", duration); // Destroy the GameObject after the duration
    }

    void DestroyGameObject()
    {
        Destroy(gameObject); // Destroy the GameObject containing the Particle System
    }
}
