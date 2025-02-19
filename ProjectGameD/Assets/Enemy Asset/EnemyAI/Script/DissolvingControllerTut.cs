using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    public VisualEffect VFXgraph;
    public SkinnedMeshRenderer skinnedMesh;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.0125f;

    [SerializeField] private Material[] skinnedMaterials;
    // Start is called before the first frame update
    void Start()
    {
        if(skinnedMesh != null){
            skinnedMaterials = skinnedMesh.materials;
        }
        
    }

    void StartDeadAnimation()
    {
        StartCoroutine(DissolveCo());
    }

    void StartDissolve(){
        StartCoroutine(DissolveCo());
    }

    void EndDissolve(String enable){
        if(enable == "1"){
            StartCoroutine(ReverseDissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        Debug.Log(skinnedMaterials[0].GetFloat("_DissolveAmount"));
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
        Debug.Log(skinnedMaterials[0].GetFloat("_DissolveAmount"));
        if(skinnedMaterials[0].GetFloat("_DissolveAmount") != 0){
            if (skinnedMaterials.Length > 0)
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
}
