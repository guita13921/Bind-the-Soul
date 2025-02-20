using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlackCircleIntro : MonoBehaviour
{
    private Volume volume;
    private Vignette vignette;

    void Start()
    {
        // Get the Volume component from this GameObject
        volume = GetComponent<Volume>();

        if (volume != null && volume.profile.TryGet<Vignette>(out vignette))
        {
            StartCoroutine(AnimateVignette(0.8f, 4f, 0f, 0.5f)); // Increase to 0.7 in 3 sec, then decrease to 0 in 0.5 sec
        }
        else
        {
            Debug.LogError("Volume or Vignette not found on " + gameObject.name);
        }
    }

    private IEnumerator AnimateVignette(float targetIntensity, float increaseDuration, float resetIntensity, float decreaseDuration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsedTime = 0f;

        // Increase intensity to targetIntensity over increaseDuration
        while (elapsedTime < increaseDuration)
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / increaseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = targetIntensity; // Ensure exact target value

        // Wait for a moment before decreasing (if needed)
        yield return new WaitForSeconds(0.1f); // Optional delay before fading out

        elapsedTime = 0f;
        startIntensity = vignette.intensity.value;

        // Decrease intensity to resetIntensity over decreaseDuration
        while (elapsedTime < decreaseDuration)
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, resetIntensity, elapsedTime / decreaseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = resetIntensity; // Ensure it reaches exactly 0
    }
}
