using UnityEngine;

public class EnemyFlashEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    public Material enemyMaterial; // The material using your Shader Graph
    public Color flashColor = Color.red; // Color of the flash
    public float flashIntensity = 1f; // Intensity of the flash
    public float flashDuration = 0.1f; // Duration of each flash

    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashIntensityID = Shader.PropertyToID("_FlashIntensity");
    private float defaultIntensity = 0f;

    private void Start()
    {
        // Set default values for the shader
        enemyMaterial.SetColor(FlashColorID, flashColor);
        enemyMaterial.SetFloat(FlashIntensityID, defaultIntensity);
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        // Turn on the flash
        enemyMaterial.SetFloat(FlashIntensityID, flashIntensity);
        yield return new WaitForSeconds(flashDuration);

        // Turn off the flash
        enemyMaterial.SetFloat(FlashIntensityID, defaultIntensity);
    }
}
