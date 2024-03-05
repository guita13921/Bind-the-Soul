using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testfade : MonoBehaviour
{
    // Start is called before the first frame update
    
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
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Calculate the amount to fade per frame
        float fadeAmount = 1 / fadeDuration;
        // Initialize the current alpha value
        float currentAlpha = 1f;

        // Fade out loop
        while (currentAlpha > 0)
        {
            // Reduce alpha based on the fade amount
            currentAlpha -= fadeAmount * Time.deltaTime;
            // Update the material color with the new alpha value
            material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            // Wait for the next frame
            yield return null;
        }

        // Once fully faded out, you can deactivate or destroy the object
        gameObject.SetActive(false);
        // Or you can destroy it
        // Destroy(gameObject);
    }

}
