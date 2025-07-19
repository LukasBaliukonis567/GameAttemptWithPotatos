using UnityEngine;

public class CharacterStatsScript : MonoBehaviour
{
    public float health = 100f;
    public float coins = 0f;
    public float damage = 10f;
    public float speed = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float armor = 0f;
    public float invulnerabilityDuration = 1.2f;
    public float XPValue = 10.0f; // XP value for each sphere
    public float XPPerKill = 2.0f; // XP orb count per kill 
    public GameObject XPSpawnerPrefab; // assign this in the Inspector
    public bool bDropXPOnDeath = true; // Whether to drop XP on death



    private bool bPlayerIsInvulnerable = false;
    private bool bHasDied = false;

    void Update()
    {
        if (!bHasDied && isKillingBlow())
        {
            bHasDied = true; // Prevent multiple death calls
            if (bDropXPOnDeath && XPSpawnerPrefab != null)
            {
                GameObject spawnerInstance = Instantiate(XPSpawnerPrefab, transform.position, Quaternion.identity);
                SphereSpawner sphere = spawnerInstance.GetComponent<SphereSpawner>();

                if (sphere != null)
                {
                    sphere.SphereSpawnCount = XPPerKill;
                    sphere.XPValue = XPValue;
                    sphere.SetDeathPoisiton(transform.position);
                    sphere.BeginSpawning(); // call your custom start logic
                }
            }
            Destroy(gameObject, 0.2f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (bPlayerIsInvulnerable)
        {
            Debug.Log("Player is invulnerable, no damage taken.");
            return;
        }

        health -= damage;
        Debug.Log($"Player took {damage} damage. Remaining health: {health}");

        if (health <= 0.0f)
        {
            // Killing blow — player dies
            return;
        }

        // Start invulnerability
        StartCoroutine(InvulnerabilityCooldown());
    }

    private System.Collections.IEnumerator InvulnerabilityCooldown()
    {
        bPlayerIsInvulnerable = true;
        Debug.Log("Player is now invulnerable.");
        yield return new WaitForSeconds(invulnerabilityDuration);
        bPlayerIsInvulnerable = false;
        Debug.Log("Player is no longer invulnerable.");
    }

    public bool isKillingBlow()
    {
        return health <= 0.0f;
    }

    public void TryDealContactDamage(GameObject other)
    {
        CharacterStatsScript targetStats = other.GetComponent<CharacterStatsScript>();
        if (targetStats == null || targetStats == this) return;

        // Deal damage to the other entity
        targetStats.TakeDamage(this.damage);
        Debug.Log($"{gameObject.name} dealt {damage} contact damage to {other.name}");
    }
}
