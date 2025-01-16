using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShow : MonoBehaviour
{
    public EnemySpawnManager enemySpawnManager;
    public GameObject upgradePanel;

    void Start()
    {
        GameObject vfx = Instantiate(upgradePanel);
    }

    void Update()
    {
        if (enemySpawnManager.enemiesRemaining == 0) { }
    }
}
