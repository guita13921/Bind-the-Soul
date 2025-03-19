using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawning : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("Spawn points where enemies will appear.")]
    public List<Transform> spawnPoints;

    [Tooltip("List of enemy types with their associated spawn costs.")]
    public List<EnemyType> enemyTypes;

    [Header("Spawn Timing")]
    [Tooltip("Delay between spawning each enemy.")]
    public float spawnDelay; // Time delay before spawning the next enemy

    [SerializeField]
    public int pointsToSpend; // Remaining points for the wave

    private List<Transform> usedSpawnPoints; // Tracks used spawn points in the current wave


    public void SpawnEenemy()
    {
        pointsToSpend = 5;
        usedSpawnPoints = new List<Transform>(); // Reset the used spawn points for the new wave
        StartCoroutine(SpawnWaveEnemies());
    }

    private IEnumerator SpawnWaveEnemies()
    {
        while (pointsToSpend > 0 && usedSpawnPoints.Count < spawnPoints.Count)
        {
            Transform spawnPoint = GetUnusedSpawnPoint();
            if (spawnPoint == null)
            {
                Debug.LogWarning("No unused spawn points available.");
                break;
            }

            EnemyType selectedEnemy = GetRandomEnemyType(pointsToSpend);
            if (selectedEnemy != null)
            {
                SpawnEnemyAtPoint(spawnPoint, selectedEnemy);
                pointsToSpend -= selectedEnemy.spawnCost;

                // Add delay before the next spawn
                yield return new WaitForSeconds(spawnDelay);
            }
            else
            {
                Debug.LogWarning("No more spawnable enemies within the budget.");
                break; // No more spawnable enemies within budget
            }
        }
    }

    private void SpawnEnemyAtPoint(Transform spawnPoint, EnemyType enemyType)
    {
        Instantiate(enemyType.prefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Spawned {enemyType.prefab.name} at {spawnPoint.position}");
        usedSpawnPoints.Add(spawnPoint); // Mark this spawn point as used
    }

    private Transform GetUnusedSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        availablePoints.RemoveAll(usedSpawnPoints.Contains);

        if (availablePoints.Count > 0)
        {
            return availablePoints[Random.Range(0, availablePoints.Count)];
        }

        return null; // No unused spawn points left
    }

    private EnemyType GetRandomEnemyType(int maxCost)
    {
        List<EnemyType> affordableEnemies = enemyTypes.FindAll(e => e.spawnCost <= maxCost);
        return affordableEnemies.Count > 0
            ? affordableEnemies[Random.Range(0, affordableEnemies.Count)]
            : null;
    }
}
