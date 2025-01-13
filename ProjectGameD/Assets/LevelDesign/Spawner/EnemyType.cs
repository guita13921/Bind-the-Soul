using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    public int spawnCost;
}