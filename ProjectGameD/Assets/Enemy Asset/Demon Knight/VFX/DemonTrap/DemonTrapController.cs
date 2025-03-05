using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonTrapController : MonoBehaviour{

    [Header("Spawn Configuration")]
    [Tooltip("Spawn points where enemies will appear.")]
    public List<Transform> spawnPoints;
    public GameObject DemonTrap;

    public void SpawnDemonTrap()
    {
        for (int i = 0; i < spawnPoints.Count; i++) 
        {
            Instantiate(DemonTrap, spawnPoints[i].position, Quaternion.identity);
        }
    }

}
