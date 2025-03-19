using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noiseoutDisableSelf : MonoBehaviour
{
    void Start()
    {
        Invoke("DisableSelf", 1.5f); // Calls DisableSelf after 3 seconds
    }

    void DisableSelf()
    {
        gameObject.SetActive(false); // Disables the GameObject
    }
}
