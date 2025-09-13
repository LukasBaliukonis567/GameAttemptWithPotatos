using UnityEngine;
using System.Collections;   // Only if you’ll use a Coroutine for attacks

[RequireComponent(typeof(CharacterStatsScript))]
public class RangerLogic : MonoBehaviour
{
    [Header("References")]
    public Transform target;           // Player transform (set from spawner, inspector, or finder)

    [Header("Behaviour Flags")]
    public bool seekEnabled = true;    // Quickly toggle AI on/off for debugging

    // Cached components / data
    private CharacterStatsScript _stats;
    private Rigidbody2D _rb;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint; // Optional: empty GameObject where projectile spawns
    private float lastShotTime = 0f;
    public float fireCooldown = 1.0f; // seconds between shots

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsScript>();
        _rb = GetComponent<Rigidbody2D>();          // Optional; recommended for physics movement
    }

    private void Update()
    {
        if (!seekEnabled || target == null) return;

        float sqrDist = (target.position - transform.position).sqrMagnitude;
        float sqrAttackRange = _stats.attackRange * _stats.attackRange;

        if (sqrDist > sqrAttackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            StopMovement();
            HandleInRange();
        }
    }

    /// <summary>Moves ranger toward the player at ranger speed, facing its target.</summary>
    private void MoveTowardsTarget()
    {
        Vector2 dir = (target.position - transform.position);
        if (dir.sqrMagnitude < 0.001f) return; // avoid jitter when too close

        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector2 newPos = _rb.position + dir * _stats.speed * Time.deltaTime;
        _rb.MovePosition(newPos);
    }

    /// <summary>Runs exactly once per frame while the ranger is in range.</summary>
    private void HandleInRange()
    {
        if (Time.time < lastShotTime + fireCooldown) return; // wait for cooldown

        if (projectilePrefab != null && target != null)
        {
            Vector3 spawnPos = firePoint ? firePoint.position : transform.position;
            Vector3 dir = (target.position - spawnPos).normalized;

            // create the projectile *at* the fire‑point immediately
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            ProjectileLogic logic = projectile.GetComponent<ProjectileLogic>();
            if (logic != null)
            {
                logic.Initialize((target.position - firePoint.position).normalized, gameObject);
            }

            lastShotTime = Time.time;
        }
    }

    // --------------------------------------------------------------------
    // Optionally handle collision damage separately via OnTriggerEnter2D
    // --------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var playerStats = other.GetComponent<CharacterStatsScript>();
        if (playerStats == null) return;

        // Deal contact damage
        playerStats.TakeDamage(_stats.damage);

        if (playerStats.isKillingBlow())
        {
            // Don’t keep chasing a corpse
            target = null;
        }
    }

    private void StopMovement()
    {
        _rb.linearVelocity = Vector2.zero; // hard stop
    }
}
