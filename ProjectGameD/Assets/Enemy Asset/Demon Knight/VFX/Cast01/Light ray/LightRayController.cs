using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRayController : MonoBehaviour
{
    public GameObject trailPrefab; 
    public int trailCount;
    public float spawnDelay;
    public float spawnRadius;

    void Start()
    {
        StartCoroutine(SpawnTrails());
    }
    
    IEnumerator SpawnTrails()
    {
        for (int i = 0; i < trailCount; i++)
        {
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset.x, transform.position.y, transform.position.z + randomOffset.y);
            Instantiate(trailPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
