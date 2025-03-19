using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShow : MonoBehaviour
{
    public EnemySpawnManager enemySpawnManager;
    public GameObject upgradePanel;
    private bool show = true;
    public GameObject soundtrackOutro;
    public GameObject currentSoundtrack;

    // void Update()
    // {
    //     Debug.Log(enemySpawnManager.enemiesRemaining + " " + enemySpawnManager.pointsToSpend);
    //     if (
    //         enemySpawnManager.currentWave == enemySpawnManager.totalWaves
    //         && enemySpawnManager.enemiesRemaining == 0
    //         && enemySpawnManager.pointsToSpend == 0
    //         && show
    //     )
    //     {
    //         GameObject vfx = Instantiate(upgradePanel);
    //         show = false;
    //     }
    // }

    public void ShowUpgradeUI()
    {
        if (show)
        {
            if (currentSoundtrack != null)
            {
                AudioSource audioSource = currentSoundtrack.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    //audioSource.volume = 0f; // Set volume to zero
                     audioSource.Stop(); // Uncomment if you want to stop the sound instead
                }
            }

            GameObject vfx = Instantiate(upgradePanel);

            show = false;
            if (soundtrackOutro != null)

            Instantiate(soundtrackOutro);

        }
    }
}
