using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff02Controller : MonoBehaviour
{
    public Material material; // Assign your material in the Inspector
    public float fadeDuration = 2f; // Time to reach full opacity
    private float currentOpacity = 0f;
    private float elapsedTime = 0f;
    private bool isFading = true;

    void Start()
    {
        if (material == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
                material = renderer.material;
        }

        if (material != null)
            material.SetFloat("_Opacity", 0f); // Ensure it starts at 0
    }

    void Update()
    {
        if (isFading && material != null)
        {
            elapsedTime += Time.deltaTime;
            currentOpacity = Mathf.Clamp01(elapsedTime / fadeDuration);
            material.SetFloat("_Opacity", currentOpacity);

            if (currentOpacity >= 1f)
                isFading = false; // Stop fading when fully opaque
        }
    }
}

