// RicochetProjectile.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(ProjectileStats), typeof(Collider2D))]
public class RicochetProjectile : MonoBehaviour
{
    [Header("Runtime (debug)")]
    public GameObject owner;
    public Transform currentTarget;

    private Rigidbody2D _rb;
    private ProjectileStats _stats;   // uses Speed/Damage/Lifetime/Effects, etc.
    private float _lifeTimer;
    private Vector2 _travelDir;       // used when no target (straight)
    private int _bouncesUsed;
    private int _maxBounces;
    private bool _requireKillToBounce;
    private float _acquireRadius;
    private LayerMask _targetMask;

    private GameObject _lastHit;
    private readonly HashSet<GameObject> _hitThisFrame = new();

    public void Initialize(GameObject owner, Vector3 initialDir, Transform initialTarget,
                           int maxBounces, bool requireKillToBounce, float acquireRadius, LayerMask targetMask)
    {
        this.owner = owner;
        _travelDir = initialDir.normalized;
        currentTarget = initialTarget;
        _maxBounces = Mathf.Max(0, maxBounces);
        _requireKillToBounce = requireKillToBounce;
        _acquireRadius = acquireRadius;
        _targetMask = targetMask;

        // Ignore owner collision
        var myCol = GetComponent<Collider2D>();
        var ownerCol = owner ? owner.GetComponent<Collider2D>() : null;
        if (myCol && ownerCol) Physics2D.IgnoreCollision(myCol, ownerCol, true);

        FaceDir(_travelDir);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<ProjectileStats>();
    }

    private void Start()
    {
        if (!_stats || _stats.Speed <= 0f)
        {
            Debug.LogWarning($"{name}: Missing ProjectileStats or Speed <= 0.");
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!_stats) return;

        // Lifetime
        _lifeTimer += Time.fixedDeltaTime;
        if (_lifeTimer >= _stats.Lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Clear per-frame re-hit guard
        _hitThisFrame.Clear();

        // Homing movement if we have a target. Otherwise fly straight.
        Vector2 pos = _rb.position;
        Vector2 step;

        if (currentTarget != null)
        {
            Vector2 toTarget = (Vector2)currentTarget.position - pos;
            if (toTarget.sqrMagnitude < 0.001f)
            {
                // Already at target; just continue straight to avoid NaNs
                step = _travelDir * _stats.Speed * Time.fixedDeltaTime;
            }
            else
            {
                _travelDir = toTarget.normalized;
                step = _travelDir * _stats.Speed * Time.fixedDeltaTime;
                FaceDir(_travelDir);
            }
        }
        else
        {
            step = _travelDir * _stats.Speed * Time.fixedDeltaTime;
        }

        _rb.MovePosition(pos + step);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other || other.gameObject == owner) return;
        if (_hitThisFrame.Contains(other.gameObject)) return; // avoid double-processing
        _hitThisFrame.Add(other.gameObject);

        // Only care about enemies (by tag) and optional layer mask
        if (!other.CompareTag("Enemy")) return;
        if (((1 << other.gameObject.layer) & _targetMask.value) == 0) return;

        var targetStats = other.GetComponent<CharacterStatsScript>();
        if (targetStats == null) return;

        // Deal damage
        targetStats.TakeDamage(_stats.Damage);

        // Apply status effects (mirrors existing ProjectileLogic).
        var eff = other.GetComponent<StatusEffectReceiver>();
        if (eff != null && _stats.Effects != null)
        {
            foreach (var e in _stats.Effects)
                eff.ApplyEffect(e);
        }

        bool killed = targetStats.isKillingBlow(); // health <= 0 after TakeDamage

        // Decide to bounce
        if (_requireKillToBounce && !killed)
        {
            Destroy(gameObject);
            return;
        }

        _bouncesUsed++;
        if (_bouncesUsed > _maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        // Acquire next target near current position, excluding the thing we just hit
        _lastHit = other.gameObject;
        currentTarget = FindClosestEnemyFrom(transform.position, _acquireRadius, _targetMask, _lastHit);

        if (currentTarget == null)
        {
            // No next target: continue straight or die; we’ll destroy to make it crisp.
            Destroy(gameObject);
            return;
        }

        // Update direction immediately toward new target
        _travelDir = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        FaceDir(_travelDir);
    }

    private void FaceDir(Vector2 dir)
    {
        if (dir.sqrMagnitude < 1e-6f) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Transform FindClosestEnemyFrom(Vector2 origin, float maxRange, LayerMask mask, GameObject exclude)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform best = null;
        float bestDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (!e) continue;
            if (exclude && e == exclude) continue;
            if (owner && e == owner) continue;
            if (((1 << e.layer) & mask.value) == 0) continue;

            float d = Vector2.Distance(origin, (Vector2)e.transform.position);
            if (d < bestDist && d <= maxRange)
            {
                bestDist = d;
                best = e.transform;
            }
        }
        return best;
    }
}