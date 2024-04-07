using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawn : MonoBehaviour
{
    public List<GenerateEnme> spawnPostion = new List<GenerateEnme>();
    public List<int> alreadyRandom = new List<int>();
    public bool isRandomize;
    public int BaseNumberOfPointToSpawn;
    public int NumberOfWaveToSpawn;
    public int numberOfMonster;
    public List<GameObject> nextstage;

    void Start()
    {
        nextstage[0].SetActive(false);
        nextstage[1].SetActive(false);
        SelectSpawn();
    }

    void Update()
    {
        numberOfMonster = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (NumberOfWaveToSpawn != 0 && numberOfMonster == 0){
            alreadyRandom.Clear();
            SelectSpawn();
            NumberOfWaveToSpawn -= 1;
        }else if(NumberOfWaveToSpawn == 0 && numberOfMonster == 0){
            nextstage[0].SetActive(true);
            nextstage[1].SetActive(true);
        }
    }

    public void SelectSpawn()
    {
        int Now_BaseNumberOfPointToSpawn = BaseNumberOfPointToSpawn;
        int index = isRandomize ? Random.Range(0, spawnPostion.Count) : 0;
        while (Now_BaseNumberOfPointToSpawn > 0)
        {
            if (alreadyRandom.Contains(index))
            {
                index = Random.Range(0, spawnPostion.Count);
            }
            else
            {
                spawnPostion[index].SpawnObject();
                alreadyRandom.Add(index);
                Now_BaseNumberOfPointToSpawn -= 1;
            }
        }
    }
}
