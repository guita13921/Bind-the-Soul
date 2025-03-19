using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashFlag : MonoBehaviour
{
    public DashCheck dashCheck; // Assign in inspector

    public float radius = 0f; // Adjust the radius for overlap check

    void Start()
    {
        // Attempt to spawn the object at the specified position

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("CantDash")) // Check for the tag "DD"
            {
                //Debug.Log(" 'DD'.");
                dashCheck.SetCollisionState(true);
                break;
            }
        }
        Destroy(gameObject);
    }
}
