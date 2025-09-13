using UnityEngine;
using System.Collections.Generic;

public class StatusEffectReceiver : MonoBehaviour
{
    [Header("Damage-over-time")]
    [Tooltip("How often DoT effects (Burn/Poison) apply damage.")]
    [Min(0.05f)] public float dotTickInterval = 1.0f; // seconds

    private readonly List<ActiveEffect> _active = new();
    private CharacterStatsScript _stats; // cached

    private void Awake()
    {
        _stats = GetComponent<CharacterStatsScript>();
        if (!_stats)
            Debug.LogWarning($"{name}: StatusEffectReceiver has no CharacterStatsScript on the same object.");
    }

    public void ApplyEffect(StatusEffect eff)
    {
        if (eff.duration <= 0f) return;

        _active.Add(new ActiveEffect(
            receiver: this,
            type: eff.type,
            duration: eff.duration,
            magnitude: eff.magnitude,
            tickInterval: dotTickInterval));

        Debug.Log($"{name} got {eff.type} for {eff.duration:0.##}s");
    }

    private void Update()
    {
        for (int i = _active.Count - 1; i >= 0; --i)
        {
            if (_active[i].Tick(Time.deltaTime))
                _active.RemoveAt(i);
        }
    }

    private class ActiveEffect
    {
        private readonly EffectType _type;
        private readonly StatusEffectReceiver _receiver;
        private readonly float _magnitude;     // For DoT: damage per tick; for Slow: % or factor you decide
        private readonly float _tickInterval;  // seconds between DoT ticks
        private float _remaining;
        private float _tickTimer;

        public ActiveEffect(StatusEffectReceiver receiver, EffectType type, float duration, float magnitude, float tickInterval)
        {
            _receiver = receiver;
            _type = type;
            _remaining = duration;
            _magnitude = magnitude;
            _tickInterval = Mathf.Max(0.05f, tickInterval);
            _tickTimer = 0f;

            // TODO: If you have immediate on-apply logic (e.g., set a slow flag), do it here.
            // Example: if (_type == EffectType.Slow) receiver.SetSlowFactor(_magnitude);
        }

        /// <returns>true if the effect is finished and should be removed</returns>
        public bool Tick(float dt)
        {
            _remaining -= dt;
            _tickTimer += dt;

            switch (_type)
            {
                case EffectType.Burn:
                case EffectType.Poison:
                    // Apply damage in discrete ticks
                    while (_tickTimer >= _tickInterval)
                    {
                        _tickTimer -= _tickInterval;
                        if (_receiver._stats != null)
                            _receiver._stats.TakeDamage(_magnitude);
                    }
                    break;

                case EffectType.Slow:
                    // If you implement slow multipliers, keep them active while _remaining > 0
                    break;

                case EffectType.Stun:
                case EffectType.Freeze:
                case EffectType.Knockback:
                    // Implement as needed
                    break;
            }

            if (_remaining <= 0f)
            {

                return true;
            }

            return false;
        }
    }
}
