using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerflashtest : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Color newEmissionColor = Color.red;
    public Material newMaterial ;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not assigned!");
            return;
        }

        Material[] materials = skinnedMeshRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = newMaterial; // Assigning the "AA" material
        }
        skinnedMeshRenderer.materials = materials;


    
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            
        }
    }

  
}
