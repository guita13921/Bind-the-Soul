using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject parentObject;
    public string childToSpawn;
    public CharacterData characterData;

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
            if (characterData.Q1_QKFasterWider)
                copiedObject.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f); // Set scale to (1, 1, 1)
            copiedObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}
