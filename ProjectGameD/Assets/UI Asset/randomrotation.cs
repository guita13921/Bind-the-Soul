using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomrotation : MonoBehaviour
{
    void Start()
    {
        // Generate a random rotation
        float randomX = Random.Range(0f, 360f);
        float randomY = Random.Range(0f, 360f);
        float randomZ = Random.Range(0f, 360f);

        // Apply the random rotation to the object's transform
        transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);
    }

    // Update is called once per frame
    void Update() { }
}
