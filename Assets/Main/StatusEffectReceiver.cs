using UnityEngine;
using System.Collections.Generic;

public class StatusEffectReceiver : MonoBehaviour
{
    private readonly List<ActiveEffect> active = new();

    public void ApplyEffect(StatusEffect eff)
    {
        active.Add(new ActiveEffect(eff.type, eff.duration, eff.magnitude));
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
        readonly EffectType type;
        float remaining;
        readonly float mag;

        public ActiveEffect(EffectType t, float d, float m)
        { type = t; remaining = d; mag = m; }

        public bool Tick(float dt)
        {
            remaining -= dt;
            if (remaining <= 0f)
            {
                // End‑effect cleanup here.
                return true; // remove
            }

            // Continuous effect example
            switch (type)
            {
                case EffectType.Burn:
                case EffectType.Poison:
                    // Damage‑over‑time
                    // GetComponent<CharacterStatsScript>().TakeDamage(Time.deltaTime * mag);
                    break;

                case EffectType.Slow:
                    // Your movement component could read a “slowFactor” held here
                    break;
            }
            return false;
        }
    }
}
