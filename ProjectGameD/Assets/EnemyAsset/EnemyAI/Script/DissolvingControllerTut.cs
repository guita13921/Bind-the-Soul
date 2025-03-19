using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    [SerializeField] private VisualEffect VFXgraph;
    private SkinnedMeshRenderer skinnedMesh;
    private float dissolveRate = 0.0125f;
    private float refreshRate = 0.0125f;

    [SerializeField] private Material[] skinnedMaterials;

    // Start is called before the first frame update
    void Start()
    {
        if (skinnedMaterials == null)
        {
            skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                skinnedMaterials = skinnedMesh.materials;
            }
        }
    }

    void StartDeadAnimation()
    {
        //if (skinnedMaterials != null) StartCoroutine(DissolveCo());
        if (VFXgraph != null) VFXgraph.Play();
    }

    public void StartDissolve()
    {
        if (skinnedMaterials != null) StartCoroutine(DissolveCo());
    }

    public void EndDissolve(String enable)
    {
        if (skinnedMaterials != null)
        {
            if (enable == "1")
            {
                StartCoroutine(ReverseDissolveCo());
            }
        }
    }

    IEnumerator DissolveCo()
    {
        // Ensure skinnedMaterials is not null and has at least one element
        if (skinnedMaterials == null || skinnedMaterials.Length == 0)
        {
            Debug.LogWarning("skinnedMaterials is null or empty. Dissolve effect skipped.");
            yield break; // Exit the coroutine
        }

        // Check if the material has the _DissolveAmount property
        if (!skinnedMaterials[0].HasProperty("_DissolveAmount"))
        {
            Debug.LogWarning("Material does not have _DissolveAmount property. Dissolve effect skipped.");
            yield break; // Exit the coroutine
        }

        if (skinnedMaterials[0].GetFloat("_DissolveAmount") <= 0)
        {
            if (VFXgraph != null)
            {
                VFXgraph.Play();
            }
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }



    IEnumerator ReverseDissolveCo()
    {
        // First, check if skinnedMaterials is null or empty
        if (skinnedMaterials == null || skinnedMaterials.Length == 0)
        {
            Debug.LogWarning("skinnedMaterials is null or empty! Reverse dissolve skipped.");
            yield break; // Exit coroutine
        }

        // Check if the material has the _DissolveAmount property
        if (!skinnedMaterials[0].HasProperty("_DissolveAmount"))
        {
            Debug.LogWarning("Material does not have _DissolveAmount property. Dissolve effect skipped.");
            yield break; // Exit coroutine
        }

        if (skinnedMaterials[0].GetFloat("_DissolveAmount") != 0)
        {
            float counter = 1;
            while (skinnedMaterials[0].GetFloat("_DissolveAmount") > 0)
            {
                counter -= dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }


}
