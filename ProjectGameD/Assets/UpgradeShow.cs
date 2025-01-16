using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShow : MonoBehaviour
{
    public EnemySpawnManager enemySpawnManager;
    public GameObject upgradePanel;

    void Start() { }

    void Update()
    {
        if (enemySpawnManager.enemiesRemaining == 0) { }
    }
}
