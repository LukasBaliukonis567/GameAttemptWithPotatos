// Ability_Ricochet.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ricochet")]
public class Ability_Ricochet : Ability
{
    public GameObject ProjectilePrefab;
    public int MaxBounces = 3;
    public float AcquireRadius = 25f;
    public bool BounceOnlyOnKill = true;
    public LayerMask TargetMask = ~0; // optional filter; defaults to "everything"

    public override void Use()
    {
        base.Use();

        if (!ProjectilePrefab || !Owner) return;

        Vector3 spawnPos = Owner.transform.position;
        Transform firstTarget = FindClosestEnemyFrom(spawnPos, AcquireRadius, TargetMask, null);

        Vector3 initialDir = firstTarget
            ? (firstTarget.position - spawnPos).normalized
            : Owner.transform.up; // fallback: straight ahead

        GameObject proj = Instantiate(ProjectilePrefab, spawnPos, Quaternion.identity);

        // Ensure the projectile has our ricochet logic
        var ric = proj.GetComponent<RicochetProjectile>();
        if (!ric) ric = proj.AddComponent<RicochetProjectile>();

        ric.Initialize(
            owner: Owner,
            initialDir: initialDir,
            initialTarget: firstTarget,
            maxBounces: MaxBounces,
            requireKillToBounce: BounceOnlyOnKill,
            acquireRadius: AcquireRadius,
            targetMask: TargetMask
        );
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

            // optional layer mask check
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
