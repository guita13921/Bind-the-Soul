using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class FrontAttackDamage : MonoBehaviour
{
    public float damageWindowStart = 0.0f;  // When damage starts
    public float damageWindowDuration = 0.1f;  // How long damage is applied
    public SphereCollider sphereCollider;
    public MeshRenderer mesh;

    private bool canDamage = false;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        StartCoroutine(DamageTiming());
        sphereCollider.enabled = false;
    }

    private IEnumerator DamageTiming()
    {
        yield return new WaitForSeconds(damageWindowStart);  // Wait for the damage window
        sphereCollider.enabled = true;
        if (mesh != null)
        {
            mesh.enabled = true;
        }
        yield return new WaitForSeconds(damageWindowDuration);  // Wait for damage duration
        sphereCollider.enabled = false;
        if (mesh != null)
        {
            mesh.enabled = false;
        }
    }
}
