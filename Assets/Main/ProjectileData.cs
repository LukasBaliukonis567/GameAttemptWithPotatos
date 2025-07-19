using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Combat/Projectile Data", fileName = "NewProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Core Stats")]
    [Min(0)] public float speed = 10f;
    [Min(0)] public int damage = 5;
    [Min(0)] public float lifetime = 5f;       // seconds before auto‑despawn
    [Min(0)] public float gravity = 0f;       // 0 = straight line, >0 = arcing
    public bool canPierce = false;
    [Min(0)] public int maxPierceTargets = 1; // only if canPierce

    [Header("Splash / Explosion")]
    [Min(0)] public float splashRadius = 0f;    // 0 => no AoE
    [Min(0)] public float splashDamage = 0f;

    [Header("On‑Hit Status Effects")]
    public StatusEffect[] onHitEffects;         // array shown in Inspector
}

/* ────────────────────────────────────────────────────────────────────── *
 *  A simple serialisable “effect bundle”. You can add more fields later *
 * ────────────────────────────────────────────────────────────────────── */
[Serializable]
public struct StatusEffect
{
    public EffectType type;
    [Min(0)] public float duration;   // How long it lasts on the target
    [Min(0)] public float magnitude;  // eg. damage per second, % slow, etc.
}

public enum EffectType { Burn, Slow, Poison, Stun, Freeze, Knockback }

