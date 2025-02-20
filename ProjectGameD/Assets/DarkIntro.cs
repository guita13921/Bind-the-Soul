using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkIntro : MonoBehaviour
{
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // Get the Volume component attached to this GameObject
        volume = GetComponent<Volume>();

        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            // Start the coroutine to smoothly transition the color filter from black to white
            StartCoroutine(TransitionColorFilter(Color.black, Color.white, 5f)); // Transition over 5 seconds
        }
        else
        {
            Debug.LogError("Volume or ColorAdjustments not found on " + gameObject.name);
        }
    }

    private IEnumerator TransitionColorFilter(Color startColor, Color endColor, float duration)
    {
        // Start with the current color filter, which is initially black (default for ColorAdjustments)
        colorAdjustments.colorFilter.value = startColor;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Lerp the color filter from the start color to the end color over time
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure the color filter reaches the final target color (white)
        colorAdjustments.colorFilter.value = endColor;
    }
}
