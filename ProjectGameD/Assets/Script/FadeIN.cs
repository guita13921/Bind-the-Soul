using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIN : MonoBehaviour
{
    public float fadeDuration = 2f; // Duration of the fading effect in seconds

    private Material material; // Material of the object
    private Color originalColor; // Original color of the object

    void Start()
    {
        // Get the material of the object
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        // Store the original color
        originalColor = material.color;
        // Start the fading coroutine
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Calculate the amount to fade per frame
        float fadeAmount = 1 / fadeDuration;
        // Initialize the current alpha value
        float currentAlpha = 0f;

        // Fade in loop
        while (currentAlpha < 1)
        {
            // Increase alpha based on the fade amount
            currentAlpha += fadeAmount * Time.deltaTime;
            // Update the material color with the new alpha value
            material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            // Wait for the next frame
            yield return null;
        }
    }
}