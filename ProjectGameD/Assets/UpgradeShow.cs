using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShow : MonoBehaviour
{
    public EnemySpawnManager enemySpawnManager;
    public GameObject upgradePanel;
    private bool show = true;

    void Update()
    {
        if (enemySpawnManager.currentWave == enemySpawnManager.totalWaves)
        {
            if (
                enemySpawnManager.enemiesRemaining == 0
                && enemySpawnManager.pointsToSpend == 0
                && show
            )
            {
                GameObject vfx = Instantiate(upgradePanel);
                show = false;
            }
        }
    }
}
