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
    public GameObject HealOrbSpawnerPrefab; // assign this in the Inspector
    public int AmountOfHealOrbs = 1; // Number of heal orbs to spawn on death
    public float chanceToSpawn = 1; // Chance to spawn heal orbs on death (1 in 158)
    public bool bDropXPOnDeath = true; // Whether to drop XP on death
    public bool bIsPlayer = false;



    private bool bPlayerIsInvulnerable = false;
    private bool bHasDied = false;

    void Update()
    {
        if (!bHasDied && isKillingBlow())
        {
            bHasDied = true;

            if (bDropXPOnDeath && XPSpawnerPrefab != null)
            {
                // XP orb
                GameObject xpSpawnerInstance = Instantiate(XPSpawnerPrefab, transform.position, Quaternion.identity);
                SphereSpawner xpSphere = xpSpawnerInstance.GetComponent<SphereSpawner>();

                if (xpSphere != null)
                {
                    xpSphere.SphereSpawnCount = XPPerKill;
                    xpSphere.XPValue = XPValue;
                    xpSphere.SetDeathPoisiton(transform.position);
                    xpSphere.BeginSpawning();
                }

                float randomValue = Random.Range(1, chanceToSpawn);
                if (randomValue == chanceToSpawn && HealOrbSpawnerPrefab != null)
                {
                    // HEAL orb — separate spawner
                    GameObject healSpawnerInstance = Instantiate(HealOrbSpawnerPrefab, transform.position, Quaternion.identity);
                    SphereSpawner healSphere = healSpawnerInstance.GetComponent<SphereSpawner>();

                    if (healSphere != null)
                    {
                        healSphere.SphereSpawnCount = AmountOfHealOrbs;
                        healSphere.amountHealed = 20.0f; // or set as needed
                        healSphere.XPValue = 0.0f;
                        healSphere.SetDeathPoisiton(transform.position);
                        healSphere.SetIsHpOrb();
                        healSphere.BeginSpawning();
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (bPlayerIsInvulnerable && bIsPlayer)
        {
            Debug.Log("Player is invulnerable, no damage taken.");
            return;
        }

        health -= damage;
        Debug.Log($"{(bIsPlayer ? "Player" : gameObject.name)} took {damage} damage. Remaining health: {health}");

        if (health <= 0.0f)
        {
            HandleDeath();
            return;
        }

        // Only players get invulnerability
        if (bIsPlayer)
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

    private void HandleDeath()
    {
        if (bHasDied) return;
        bHasDied = true;

        if (bIsPlayer)
        {
            Debug.Log("Player has died. Game Over or Respawn logic goes here.");
            // TODO: Trigger game over UI, pause game, etc.
            return;
        }


        Destroy(gameObject, 0.2f);

        // ENEMY death logic
        if (bDropXPOnDeath && XPSpawnerPrefab != null)
        {
            GameObject xpSpawnerGO = Instantiate(XPSpawnerPrefab, transform.position, Quaternion.identity);
            SphereSpawner xpSpawner = xpSpawnerGO.GetComponent<SphereSpawner>();
            if (xpSpawner != null)
            {
                xpSpawner.SphereSpawnCount = XPPerKill;
                xpSpawner.XPValue = XPValue;
                xpSpawner.SetDeathPoisiton(transform.position);
                xpSpawner.BeginSpawning(); // Spawns and destroys itself
            }

            // HP ORBS — totally separate object
            if (Random.Range(1, chanceToSpawn) == chanceToSpawn && HealOrbSpawnerPrefab != null)
            {
                GameObject healSpawnerGO = Instantiate(HealOrbSpawnerPrefab, transform.position, Quaternion.identity);
                SphereSpawner healSpawner = healSpawnerGO.GetComponent<SphereSpawner>();
                if (healSpawner != null)
                {
                    healSpawner.SphereSpawnCount = AmountOfHealOrbs;
                    healSpawner.XPValue = 0.0f;
                    healSpawner.SetIsHpOrb();
                    healSpawner.SetDeathPoisiton(transform.position);
                    healSpawner.BeginSpawning(); // no code gets run after this line 
                }
            }

        }
    }
}
