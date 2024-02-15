using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashTest : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Color newEmissionColor = Color.red;
    public Material newMaterial;
    private Material[] originalMaterials;
    private bool isOriginalMaterial = true;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not assigned!");
            return;
        }

        // Store the original materials of the SkinnedMeshRenderer
        originalMaterials = skinnedMeshRenderer.materials;
    }

    void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle between original material and new material
            if (isOriginalMaterial)
            {
                ChangeMaterial(newMaterial);
            }
            else
            {
                RestoreOriginalMaterial();
            }
            isOriginalMaterial = !isOriginalMaterial;
        }
    }

    void ChangeMaterial(Material material)
    {
        Material[] materials = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            materials[i] = material;
        }
        skinnedMeshRenderer.materials = materials;
    }

    void RestoreOriginalMaterial()
    {
        skinnedMeshRenderer.materials = originalMaterials;
    }
}