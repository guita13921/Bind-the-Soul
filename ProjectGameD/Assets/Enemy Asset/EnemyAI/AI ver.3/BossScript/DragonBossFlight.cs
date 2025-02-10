using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossFlight : MonoBehaviour
{
    [Header("Fire Rain Settings")]
    public GameObject fireRainPrefab; // Prefab for fire rain
    public int fireRainNearCount = 2; // Number of fire rains near the player
    public int fireRainOuterCount = 3; // Number of fire rains in the outer range
    public float fireRainNearRadius = 3f; // Radius for near fire rain
    public float fireRainOuterRadius = 10f; // Radius for outer fire rain

    [Header("Dive Bomb Settings")]
    public GameObject meteorPrefab; // Prefab for meteors
    public int meteorCount = 10; // Number of meteors
    public float meteorRadius = 8f; // Radius for meteors around the player

    [Header("FireTornado Settings")]
    public GameObject FireTornadoPrefab; // Prefab for lava pits
    public int FireTornadoCount = 3; // Number of lava pits
    public float FireTornadoRadius = 7f; // Radius for lava pits around the player

    [Header("Player Reference")]
    public Transform player; // Reference to the player's position

    private void Start()
    {
        if (!player)
        {
            player = GameObject.FindWithTag("Player").transform; // Find the player by tag
        }
    }

    public void PerformFireRain()
    {
        if (player == null || fireRainPrefab == null) return;

        // Spawn fire rains near the player
        for (int i = 0; i < fireRainNearCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionNearPlayer(fireRainNearRadius);
            Instantiate(fireRainPrefab, spawnPosition, Quaternion.identity);
        }

        // Spawn fire rains in the outer range
        for (int i = 0; i < fireRainOuterCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionNearPlayer(fireRainOuterRadius, fireRainNearRadius);
            Instantiate(fireRainPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void PerformDiveBomb()
    {
        if (player == null || meteorPrefab == null) return;

        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionNearPlayer(meteorRadius);
            Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void PerformFireTornado()
    {
        if (player == null || FireTornadoPrefab == null) return;

        for (int i = 0; i < FireTornadoCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionNearPlayer(FireTornadoRadius);
            GameObject tornado = Instantiate(FireTornadoPrefab, spawnPosition, Quaternion.identity);

            // Start the tornado scaling coroutine
            StartCoroutine(ScaleTornado(tornado));
        }
    }

    /// <summary>
    /// Scales the tornado over time: Y-axis maxes at 1, while X and Z scale to 2.
    /// </summary>
    /// <param name="tornado">The tornado GameObject to scale.</param>
    private IEnumerator ScaleTornado(GameObject tornado)
    {
        if (tornado == null) yield break;

        // Phase 1: Increase Y scale from 0 to 1 over 3 seconds
        float duration = 3f;
        float elapsedTime = 0f;
        Vector3 initialScale = new Vector3(1f, 0f, 1f);
        Vector3 targetScale = new Vector3(2f, 1f, 2f); // Y-axis maxes at 1, X/Z scale to 2

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            tornado.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is exactly the target scale
        tornado.transform.localScale = targetScale;
    }


    private Vector3 GetRandomPositionNearPlayer(float radius, float minRadius = 0f)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Random angle in radians
        float distance = Random.Range(minRadius, radius); // Random distance within range
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;
        return player.position + offset;
    }
}
