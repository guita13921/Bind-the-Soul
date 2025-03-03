using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontAttackController : MonoBehaviour

{
    public GameObject trailPrefab; 
    public Transform player;
    public int trailCount;
    public float trailSpeed;
    public float spawnDelay;
    public float vfxSpawnInterval;
    public float spawnRadius;
    
    void Start()
    {
        StartCoroutine(SpawnTrails());
    }
    
    IEnumerator SpawnTrails()
    {
        for (int i = 0; i < trailCount; i++)
        {
            Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            GameObject trail = Instantiate(trailPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveTrail(trail));
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    IEnumerator MoveTrail(GameObject trail)
    {
        while (trail != null && player != null)
        {
            trail.transform.position = Vector3.Lerp(trail.transform.position, player.position, trailSpeed * Time.deltaTime);
            SpawnVFX(trail.transform.position);
            yield return new WaitForSeconds(vfxSpawnInterval);
        }
    }
    
    void SpawnVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(trailPrefab, position, Quaternion.identity);
        Destroy(vfx, 1f);
    }
}
