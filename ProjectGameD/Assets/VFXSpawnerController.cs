using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpawnerController : MonoBehaviour
{
    public GameObject spawnedVFX;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = spawnedVFX.transform.localScale;
        StartCoroutine(ReduceScaleOverTime());
    }

    private IEnumerator ReduceScaleOverTime()
    {
        // Wait for 2 seconds before starting the scale reduction
        yield return new WaitForSeconds(2f);

        float duration = 1f; // Time to reduce scale
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float scaleFactor = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spawnedVFX.transform.localScale = initialScale * scaleFactor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spawnedVFX.transform.localScale = Vector3.zero; // Ensure scale is fully zero
        Destroy(spawnedVFX); // Optionally destroy after shrinking
    }
}
