using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletdestroy : MonoBehaviour
{
    public GameObject explode;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(explode, transform.position, transform.rotation);
            Debug.Log("haha");

            Destroy(transform.parent.gameObject);
        }
    }
}
