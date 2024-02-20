using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfdestroy : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Play the audio
        audioSource.Play();
    }

    void Update()
    {
        // Check if the audio has finished playing
        if (!audioSource.isPlaying)
        {
            // Destroy this GameObject
            Destroy(gameObject);
        }
    }
}