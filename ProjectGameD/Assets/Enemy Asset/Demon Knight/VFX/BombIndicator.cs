using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombIndicator : MonoBehaviour
{
    public RectTransform attackIndicator; // Ensure this is the RectTransform of your UI image.
    public RectTransform attackIndicatorFinish; // Target scale indicator.

    public void ShowIndicator(int AttackTimeFrame)
    {
        if (attackIndicator != null)
        {
            attackIndicator.gameObject.SetActive(true); // Enable the object4
            StartCoroutine(ScaleIndicatorY(AttackTimeFrame)); // Start the animation
        }
    }

    private IEnumerator ScaleIndicatorY(int frameCount)
    {
        Vector3 originalScale = attackIndicator.localScale;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);
        attackIndicatorFinish.localScale = new Vector3(1f, 1f, 1f);

        int elapsedFrames = 0;
        attackIndicator.localScale = new Vector3(0, 0, 0);

        while (elapsedFrames < frameCount)
        {
            elapsedFrames++;
            float progress = Mathf.Clamp01((float)elapsedFrames / frameCount);

            // Lerp scale between 0 and target
            attackIndicator.localScale = Vector3.Lerp(
                new Vector3(0, 0, 0),
                targetScale,
                progress
            );

            yield return null; // Wait for the next frame
        }
    }


        public void HideIndicator()
    {
        if (attackIndicator != null)
        {
            Vector3 originalScale = attackIndicator.localScale;
            attackIndicator.localScale = new Vector3(0, 0, 0);
            attackIndicatorFinish.localScale = new Vector3(0, 0, 0);
        }
    }
}
