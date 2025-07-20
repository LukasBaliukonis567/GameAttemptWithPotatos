using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Vector2 SpawnLocation;
    public GameObject EnemyPrefab;
    public float SpawnInterval = 8.0f; // Time in seconds between spawns
    public int MaxEnemies = 5; // Maximum number of enemies to spawn
    public int EnemiesSpawned = 0; // Counter for spawned enemies
    public float DistanceFromPlayer = 15.0f; // Distance from player to spawn enemies
    public float SpawnRadius = 35.0f; // Radius around the spawner to spawn enemy at
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        EnemiesSpawned = 0;

        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    private void Update()
    { 

    }

    public IEnumerator SpawnEnemies()
    {
        while (EnemiesSpawned < MaxEnemies)
        {
            yield return new WaitForSeconds(SpawnInterval);

            if (EnemiesSpawned > MaxEnemies)
                yield break;

            Vector2 spawnPos = GetRandomPositionAroundPlayer(SpawnLocation);
            GameObject enemy = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);

            if (enemy != null)
            {
                // assign player transform as target (works for any script that has it)
                var ranger = enemy.GetComponent<RangerLogic>();
                if (ranger) ranger.target = player.transform;

                var badGuy = enemy.GetComponent<BadGuyLogic>();
                if (badGuy) badGuy.target = player.transform;

                Debug.Log($"Spawned enemy at {spawnPos}");
                EnemiesSpawned++;
            }
            else
            {
                Debug.LogError("Failed to spawn enemy prefab.");
            }
        }

        Debug.Log("Maximum number of enemies spawned — stopping spawns.");
    }

    Vector2 GetRandomPositionAroundPlayer(Vector2 SpawnLocation)
    {
        // Pick a random direction
        Vector2 direction = Random.insideUnitCircle.normalized;

        // Pick a random distance between min and max
        float distance = Random.Range(DistanceFromPlayer, SpawnRadius);

        Vector2 playerPosition = player.transform.position;

        // Return the point at that direction and distance
        return playerPosition + direction * distance;
    }
}
