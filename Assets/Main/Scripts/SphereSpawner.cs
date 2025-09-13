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
    private Vector2 _deathPosition = new Vector2(0, 0); // Position where the player dies
    public bool bHpOrb = false;
    public float amountHealed = 20.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SpawnSpheres()
    {
        yield return new WaitForSeconds(SpawnInterval);
        Instantiate(SpherePrefab, GetRandomPositionNearDeath(), Quaternion.identity);
    }

    public void SpawnSpheresWithCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(SpherePrefab, GetRandomPositionNearDeath(), Quaternion.identity);

            //yield return new WaitForSeconds(SpawnInterval); // only spawn the amount once
        }
        Destroy(gameObject); // clean up the spawner
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
        float x = UnityEngine.Random.Range(_deathPosition.x - MIN_BOUNDS_ALT, _deathPosition.x + MAX_BOUNDS_ALT);
        float y = UnityEngine.Random.Range(_deathPosition.y - MIN_BOUNDS_ALT, _deathPosition.y - MAX_BOUNDS_ALT);

        if (bEnableTestingOutput)
        {
            Vector2 position = new Vector2(x, y);
            Debug.Log($"Spawning sphere at position: {position}");
        }

        return new Vector2(x, y);
    }

    public void SetDeathPoisiton(Vector2 pos)
    {
        _deathPosition = pos;
    }

    public void BeginSpawning()
    {

        if (SpherePrefab == null)
        {
            Debug.LogError($"{name}: SpherePrefab is NOT assigned!", this);
        }
        else
        {
            Debug.Log($"{name}: SphereSpawner starting. SpherePrefab = {SpherePrefab.name}", this);
        }

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;

        if (SphereSpawnCount == 0)
        {
            StartCoroutine(SpawnAndDestroy()); // replaces SpawnSpheres
        }
        else
        {
            SpawnSpheresWithCount((int)SphereSpawnCount);
            Destroy(gameObject); // clean up after use
        }
    }


    public void SetIsHpOrb()
    {
        bHpOrb = true;
        XPValue = 0.0f;
    }

    public bool GetIsHpOrb()
    {
        return bHpOrb;
    }

    private IEnumerator SpawnAndDestroy()
    {
        yield return new WaitForSeconds(SpawnInterval);
        Instantiate(SpherePrefab, GetRandomPositionNearDeath(), Quaternion.identity);
        Destroy(gameObject);
    }
}
