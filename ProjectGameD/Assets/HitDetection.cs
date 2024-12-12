using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public GameObject hitVFX;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            // Spawn VFX at the point of collision
            GameObject vfx = Instantiate(hitVFX, this.transform.position, Quaternion.identity);
            Debug.Log("ff");
        }
    }
}
