using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For using the Text component

[System.Serializable]
public class EnemyType
{
    public GameObject prefab; // The enemy prefab
    public int spawnCost; // Points required to spawn this enemy
}

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("Spawn points where enemies will appear.")]
    public List<Transform> spawnPoints;

    [Tooltip("List of enemy types with their associated spawn costs.")]
    public List<EnemyType> enemyTypes;

    [Header("Wave Settings")]
    [Tooltip("Base points available for spawning enemies in a wave.")]
    public int basePointsPerWave;

    [Tooltip("Total number of waves.")]
    public int totalWaves;

    [Tooltip("Objects to activate after the last wave.")]
    public List<GameObject> nextStageObjects;

    [Header("Spawn Timing")]
    [Tooltip("Delay between spawning each enemy.")]
    public float spawnDelay = 5f; // Time delay before spawning the next enemy

    [Header("UI Elements")]
    [Tooltip("Text element to display current wave number.")]
    public Text waveText;

    [Tooltip("Text element to display remaining points for the current wave.")]
    public Text pointsText;

    [SerializeField]
    public int currentWave = 0;

    [SerializeField]
    public int enemiesRemaining;

    [SerializeField]
    public int pointsToSpend; // Remaining points for the wave

    private List<Transform> usedSpawnPoints; // Tracks used spawn points in the current wave

    [SerializeField]
    UpgradeShow upgradeShow;

    private void Start()
    {
            if (upgradeShow == null)
    {
        upgradeShow = FindObjectOfType<UpgradeShow>();
    }
        InitializeNextStageObjects();
        StartNextWave();
    }

    private void Update()
    {
        // Monitor active enemies in the scene
        enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemiesRemaining == 0)
        {
            if (currentWave < totalWaves)
            {
                StartNextWave();
            }
            else
            {
                if(upgradeShow != null){
                    upgradeShow.ShowUpgradeUI();
                }
                EnableNextStageObjects();
            }
        }

        if(upgradeShow != null){
            UpdateUI();
        }
    }

    private void StartNextWave()
    {
        currentWave++;
        pointsToSpend = basePointsPerWave;
        usedSpawnPoints = new List<Transform>(); // Reset the used spawn points for the new wave

        Debug.Log($"Starting Wave {currentWave} with {pointsToSpend} points.");
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

        Debug.Log($"Wave {currentWave} completed spawning!");
    }

    private void SpawnEnemyAtPoint(Transform spawnPoint, EnemyType enemyType)
    {
<<<<<<< HEAD
        GameObject instantiatedObject = Instantiate(enemyType.prefab, spawnPoint.position, Quaternion.identity);
        instantiatedObject.SetActive(true); // Ensure the instantiated object is active

=======
        
        GameObject monster = Instantiate(enemyType.prefab, spawnPoint.position, Quaternion.identity);
        monster.SetActive(true);
>>>>>>> main
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

    private void InitializeNextStageObjects()
    {
        if(nextStageObjects != null){
            foreach (var obj in nextStageObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    private void EnableNextStageObjects()
    {
        if(nextStageObjects != null){
            foreach (var obj in nextStageObjects)
            {
                obj.SetActive(true);
            }
        }

    }

    private void UpdateUI()
    {
        // Update wave and points text
        if (waveText != null)
        {
            waveText.text = $"Wave: {currentWave}/{totalWaves}";
        }

        if (pointsText != null)
        {
            pointsText.text = $"Points: {pointsToSpend}";
        }
    }
}
