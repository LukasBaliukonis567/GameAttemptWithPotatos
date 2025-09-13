using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MagnetPickup : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float Radius = 100f;          // how far to search for XP orbs
    public float Speed = 40f;           // pull speed while paused
    public float StopDistance = 0.05f;  // how close to the player before we stop moving an orb
    public float MaxPausedSeconds = 2f; // safety cap so we never stay paused forever

    [Header("Filtering (optional)")]
    public string XPTargetTag = "XP";   // tag used by XP orbs

    private bool _consuming;

    private void Reset()
    {
        // Make sure the pickup collider is a trigger.
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consuming) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttractXPOrbs(other.transform));
        }
    }

    private IEnumerator AttractXPOrbs(Transform player)
    {
        _consuming = true;

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        var move = player.GetComponent<MovementScript>();
        bool hadMovement = move && move.enabled;
        if (move) move.enabled = false;

        var rb = player.GetComponent<Rigidbody2D>();
        Vector2 savedVel = Vector2.zero;
        if (rb)
        {
            savedVel = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
        }

        List<Transform> targets = new List<Transform>();
        GameObject[] all = GameObject.FindGameObjectsWithTag(XPTargetTag);
        Vector2 p = player.position;

        foreach (var go in all)
        {
            if (!go) continue;
            if ((go.transform.position - (Vector3)p).sqrMagnitude <= Radius * Radius)
            {
                targets.Add(go.transform);
            }
        }

        float elapsed = 0f;
        while (targets.Count > 0 && elapsed < MaxPausedSeconds)
        {
            float dt = Time.unscaledDeltaTime;
            elapsed += dt;

            p = player.position;

            for (int i = targets.Count - 1; i >= 0; --i)
            {
                Transform t = targets[i];
                if (!t)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                Vector3 tp = t.position;
                float dist = Vector2.Distance(tp, p);

                if (dist <= StopDistance)
                {
                    targets.RemoveAt(i);
                }
                else
                {
                    Vector3 dir = (p - (Vector2)tp).normalized;
                    t.position += dir * Speed * dt;
                }
            }

            yield return null;
        }

        Time.timeScale = previousTimeScale;

        if (rb) rb.linearVelocity = savedVel;
        if (move && hadMovement) move.enabled = true;

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}

