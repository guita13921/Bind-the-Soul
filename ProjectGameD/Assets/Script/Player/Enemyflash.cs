using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyflash : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material newMaterial;
    private Material[] originalMaterials;
    private bool isOriginalMaterial = true;
    public Animator animator; // Reference to the Animator component on another object

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
      
         if(animator.GetCurrentAnimatorStateInfo(0).IsName("Hit1"))
        {
            // Toggle between original material and new material
                ChangeMaterial(newMaterial);
            
            

        }else{
            RestoreOriginalMaterial();
        }




    }


    void ChangeMaterial(Material material)
    {
        Material[] materials = new Material[originalMaterials.Length];
        materials[0] = material;
        skinnedMeshRenderer.materials = materials;
    }

    void RestoreOriginalMaterial()
    {
        skinnedMeshRenderer.materials = originalMaterials;
    }
}