using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

public class SphereSpawner : MonoBehaviour
{
    public float SpawnInterval = 5.0f; // Time in seconds between spawns
    public GameObject SpherePrefab; // Prefab of the sphere to spawn
    bool bEnableTestingOutput = true; // Enable or disable console output for testing
    public float MAX_BOUNDS = 5.0f; // Maximum bounds for spawning spheres
    public float MIN_BOUNDS = -5.0f; // Minimum bounds for spawning spheres
    public float SphereSpawnCount = 0.0f; // Change this to instead count X amount
    public float XPValue = 10.0f; // XP value for each sphere
    public float MAX_BOUNDS_ALT = 2.0f; // Alternate maximum bounds for spawning spheres
    public float MIN_BOUNDS_ALT = -2.0f; // Alternate minimum bounds for spawning spheres

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;

        if(SphereSpawnCount == 0)
            StartCoroutine(SpawnSpheres());
        else
            StartCoroutine(SpawnSpheresWithCount((int)SphereSpawnCount));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnSpheres()
    {
        yield return new WaitForSeconds(SpawnInterval);
        Instantiate(SpherePrefab, GetRandomPosition(), Quaternion.identity);
    }

    IEnumerator SpawnSpheresWithCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(SpherePrefab, GetRandomPosition(), Quaternion.identity);
            yield return new WaitForSeconds(SpawnInterval);
        }
    }


    Vector2 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(MIN_BOUNDS, MAX_BOUNDS);
        float y = UnityEngine.Random.Range(MIN_BOUNDS, MAX_BOUNDS);

        if (bEnableTestingOutput)
        { 
            Vector2 position = new Vector2(x, y);
            Debug.Log($"Spawning sphere at position: {position}");
        }

        return new Vector2(x, y);
    }

    Vector2 GetRandomPositionNearDeath()
    {
        float x = UnityEngine.Random.Range(MIN_BOUNDS_ALT, MAX_BOUNDS_ALT);
        float y = UnityEngine.Random.Range(MIN_BOUNDS_ALT, MAX_BOUNDS_ALT);

        if (bEnableTestingOutput)
        {
            Vector2 position = new Vector2(x, y);
            Debug.Log($"Spawning sphere at position: {position}");
        }

        return new Vector2(x, y);
    }
}
