using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject parentObject;
    public string childToSpawn;

    void Start()
    {
        Transform childTransform = parentObject.transform.Find(childToSpawn); // Replace with the actual child name

        if (childTransform != null)
        {
            // Get the world position and rotation of the child object
            Vector3 worldPosition = childTransform.position;
            Quaternion worldRotation = childTransform.rotation;

            // Instantiate the child object in world space (at the same position and rotation)
            GameObject copiedObject = Instantiate(
                childTransform.gameObject,
                worldPosition,
                worldRotation
            );

            // Optionally, you can detach the copied object from any parent if you don't want it to have a parent
            copiedObject.transform.SetParent(null); // This removes the new object from any hierarchy parent
            copiedObject.SetActive(true); // Make sure the object is active
        }
        Destroy(gameObject);
    }
}
