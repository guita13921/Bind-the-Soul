using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GenerateEnme : MonoBehaviour
{
    public List<GameObject> objectToSpawn = new List<GameObject>();
    public bool isRandomize;

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnObject(){
        int index = isRandomize ? Random.Range(0, objectToSpawn.Count) : 0;
        if(objectToSpawn.Count > 0){
            Instantiate(objectToSpawn[index], transform.position, transform.rotation);
        }
    }
}
