using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil;

public class StatusEffectReceiver : MonoBehaviour
{
    private readonly List<ActiveEffect> active = new();

    public void ApplyEffect(StatusEffect eff, CharacterStatsScript targetStats, float tickInterval)
    {
        active.Add(new ActiveEffect(eff.owner, targetStats, eff.type, eff.duration, eff.magnitude, tickInterval));
        // Do immediate response (slow speed, start DoT, etc.)
        Debug.Log($"{name} got {eff.type} for {eff.duration}s");
    }

    private void Update()
    {
        for (int i = active.Count - 1; i >= 0; --i)
        {
            if (active[i].Tick(Time.deltaTime))
                active.RemoveAt(i);
        }
    }

    private class ActiveEffect
    {
        private readonly EffectType type;
        private readonly StatusEffectReceiver owner;
        private readonly CharacterStatsScript target;
        private float remaining;
        private readonly float mag;
        private float tickTimer;

        public ActiveEffect(StatusEffectReceiver owner, CharacterStatsScript target, EffectType t, float d, float m, float tickInterval)
        {
            this.owner = owner;
            this.target = target;
            type = t;
            remaining = d;
            mag = m;
            this.tickTimer = tickInterval; // apply immediately on first frame
        }

        public bool Tick(float dt)
        {
            remaining -= dt;
            if (remaining <= 0f)
            {
                // End‑effect cleanup here.
                return true; // remove
            }
            tickTimer -= dt;

            // Continuous effect example
            if (tickTimer <= 0.0f)
            {
                switch (type)
                {
                    case EffectType.Burn:
                    case EffectType.Poison:
                        // Damage‑over‑time
                        if (target != null)
                            target.TakeDamage(mag);

                        break;

                    case EffectType.Slow:
                        // Your movement component could read a “slowFactor” held here
                        break;
                }
            }
            return false;
        }
    }
}
