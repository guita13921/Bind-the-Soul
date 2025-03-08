using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    private VisualEffect VFXgraph;
    private SkinnedMeshRenderer skinnedMesh;
    private float dissolveRate = 0.0125f;
    private float refreshRate = 0.0125f;

    [SerializeField] private Material[] skinnedMaterials;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        if(skinnedMesh != null){
            skinnedMaterials = skinnedMesh.materials;
        }
    }

    void StartDeadAnimation()
    {
        StartCoroutine(DissolveCo());
    }

    public void StartDissolve(){
        StartCoroutine(DissolveCo());
    }

    public void EndDissolve(String enable){
        if(enable == "1"){
            StartCoroutine(ReverseDissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        //Debug.Log(skinnedMaterials[0].GetFloat("_DissolveAmount"));
        if(skinnedMaterials[0].GetFloat("_DissolveAmount") <= 0){
            if(VFXgraph != null)
            {
                VFXgraph.Play();
            }
            if(skinnedMaterials.Length > 0)
            {
                float counter = 0;
                while(skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
                {
                    counter += dissolveRate;
                    for(int i=0; i<skinnedMaterials.Length; i++)
                    {
                        skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                    }
                    yield return new WaitForSeconds(refreshRate);
                }
            }
        }
    }

    IEnumerator ReverseDissolveCo()
    {
        if (skinnedMaterials == null || skinnedMaterials.Length == 0)
        {
            Debug.LogError("skinnedMaterials is null or empty!");
            yield break;
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
