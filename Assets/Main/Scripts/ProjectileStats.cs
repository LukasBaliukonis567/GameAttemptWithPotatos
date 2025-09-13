using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileStats : MonoBehaviour
{
    [Tooltip("Plug in the ScriptableObject that defines this projectile.")]
    public ProjectileData data;

    /*  Expose handy shortcuts so other scripts don’t need to keep the full
        data reference.  We copy immutable stats to fields here so we can
        tweak them at runtime (e.g. buffs) without touching the SO instance. */
    public float Speed { get; private set; }
    public int Damage { get; private set; }
    public float Lifetime { get; private set; }
    public float Gravity { get; private set; }
    public bool CanPierce { get; private set; }
    public int MaxPierce { get; private set; }
    public float SplashRadius { get; private set; }
    public float SplashDamage { get; private set; }
    public StatusEffect[] Effects { get; private set; }

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"{name}: No ProjectileData assigned!", this);
            return;
        }

        // Copy once; you can still modify per‑instance at runtime if needed
        Speed = data.speed;
        Damage = data.damage;
        Lifetime = data.lifetime;
        Gravity = data.gravity;
        CanPierce = data.canPierce;
        MaxPierce = data.maxPierceTargets;
        SplashRadius = data.splashRadius;
        SplashDamage = data.splashDamage;
        Effects = data.onHitEffects;
    }
}
