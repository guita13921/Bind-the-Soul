using UnityEngine;
using System.Collections;

public class SwapMesh : MonoBehaviour
{
    public GameObject next; // Drag the next object prefab (e.g., banana) into this in the Inspector.
    public GameObject self; // The current object to be destroyed.
    public float delay = 1.0f; // Time in seconds before the swap happens.
    public GameObject dustEffect; // Explosion or dust effect prefab.
    public Transform dustPoint;

    void OnTriggerEnter(Collider other)
    {
        if (next != null)
        {
            if (other.gameObject.CompareTag("PlayerSword")) // Check for the specific tag.
            {
                StartCoroutine(SwapAfterDelay(dustPoint));
            }
        }
        else
        {
            // Do nothing if the "next" object is not assigned.
        }
    }

    IEnumerator SwapAfterDelay(Transform dustPoint)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay.

        Vector3 position = transform.position;
        Vector3 DustPoint = dustPoint.position;
        Quaternion DustPoint_rotation = dustPoint.rotation;
        Quaternion rotation = transform.rotation;

        // Instantiate the dust effect at the object's position and rotation.
        if (dustEffect != null)
        {
            Instantiate(dustEffect, DustPoint, DustPoint_rotation);
        }

        Instantiate(next, position, rotation); // Spawn the next object (e.g., banana).
        Destroy(gameObject); // Destroy the current object.
        Destroy(self); // Destroy the "self" object if it's separate.
    }
}
