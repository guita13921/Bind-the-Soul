using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingControllerTut : MonoBehaviour
{
    public VisualEffect VFXgraph;
    public SkinnedMeshRenderer skinnedMesh;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    [SerializeField] private Material[] skinnedMaterials;
    // Start is called before the first frame update
    void Start()
    {
        if(skinnedMesh != null){
            skinnedMaterials = skinnedMesh.materials;
        }
        
    }

    // Update is called once per frame
    void StartDeadAnimation()
    {
        StartCoroutine(DissolveCo());
    }

    IEnumerator DissolveCo()
    {
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
