using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class fadeText : MonoBehaviour

{
     public TextMeshProUGUI uiText; // Assign your TextMeshProUGUI component
    public float fadeSpeed = 1f; // Speed at which text fades out
    private float alpha = 1f;
    private bool isFading = false;
    void Update()
    {
        if (Input.anyKey)
        {
            // Player pressed a key, make text fully visible
            alpha = 1f;
            isFading = false;
        }
        else
        {
            // No input, start fading out
            isFading = true;
        }

        if (isFading)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha); // Ensure alpha stays between 0 and 1
        }

        // Apply the alpha to the text color
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, alpha);
    }
}
