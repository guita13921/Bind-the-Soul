using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disPlayragDoll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
                // Assuming ragdoll components are attached to the same GameObject
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = true; // Disables physics simulation
        }
        
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) {
            col.enabled = false; // Disables collisions
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
