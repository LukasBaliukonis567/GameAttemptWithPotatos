using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(ProjectileStats))]
public class ProjectileLogic : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask targetLayer;         // Layers this projectile can affect
    public GameObject impactEffect;       // Visual effect on impact
    public GameObject owner;              // Entity who shot the projectile

    // Runtime
    private Vector3 direction;            // Normalized direction
    private bool hasHitTarget = false;    // Prevents double hits
    private ProjectileStats stats;        // Cached reference to projectile stat component
    private Rigidbody2D rb;               // Cached Rigidbody2D
    private float lifetimeTimer = 0f;
    private float tickInterval = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<ProjectileStats>();
    }

    private void Start()
    {
        if (stats == null || stats.Speed <= 0f)
        {
            Debug.LogWarning("Projectile has no stats or speed is zero.");
            Destroy(gameObject);
            return;
        }

        // Set facing direction based on initial direction
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward); // Adjust based on sprite
        }
    }

    private void FixedUpdate()
    {
        if (hasHitTarget || stats == null) return;

        // Move using physics-friendly method
        rb.MovePosition(rb.position + (Vector2)direction * stats.Speed * Time.fixedDeltaTime);

        // Lifetime handling
        lifetimeTimer += Time.fixedDeltaTime;
        if (lifetimeTimer >= stats.Lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Initialize this projectile with direction and metadata.</summary>
    public void Initialize(Vector3 dir, GameObject projectileOwner)
    {
        direction = dir.normalized;
        owner = projectileOwner;

        // Ignore collision with owner
        if (owner != null)
        {
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D ownerCollider = owner.GetComponent<Collider2D>();
            if (myCollider != null && ownerCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, ownerCollider, true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitTarget || other.gameObject == owner) return;
        //if ((targetLayer.value & (1 << other.gameObject.layer)) == 0) return; //TO DO in the future assign layers so that there's no friendly fire, or just use tags idk someone else do this :)

        hasHitTarget = true;

        // Spawn impact effect
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        // Damage primary target
        var targetStats = other.GetComponent<CharacterStatsScript>();
        if (targetStats != null)
        {
            targetStats.TakeDamage(stats.Damage);

            // Apply status effects
            var effectReceiver = other.GetComponent<StatusEffectReceiver>();
            if (effectReceiver != null)
            {
                foreach (var effect in stats.Effects)
                {
                    effectReceiver.ApplyEffect(effect);
                }
            }
        }

        // Splash damage
        if (stats.SplashRadius > 0f)
        {
            Collider2D[] splashHits = Physics2D.OverlapCircleAll(transform.position, stats.SplashRadius);
            foreach (var hit in splashHits)
            {
                if (hit.gameObject == owner || hit.gameObject == other.gameObject) continue;
                if (!hit.CompareTag("Enemy")) continue;

                var splashStats = hit.GetComponent<CharacterStatsScript>();
                if (splashStats != null)
                {
                    splashStats.TakeDamage((int)stats.SplashDamage);
                }
            }
        }

            Destroy(gameObject);
    }

}