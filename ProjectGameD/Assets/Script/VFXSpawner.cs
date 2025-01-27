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
        Transform childTransform = parentObject.transform.Find(childToSpawn);

        if (childTransform != null)
        {
            Vector3 worldPosition = childTransform.position;
            Quaternion worldRotation = childTransform.rotation;

            GameObject copiedObject = Instantiate(
                childTransform.gameObject,
                worldPosition,
                worldRotation
            );

            copiedObject.transform.SetParent(null);
            copiedObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}
