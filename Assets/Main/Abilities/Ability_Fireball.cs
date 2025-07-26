using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Fireball")]
public class Ability_Fireball : Ability
{
    public GameObject ProjectilePrefab;

    public override void Use()
    {
        base.Use();

        if (ProjectilePrefab && Owner)
        {
            Vector3 spawnPos = Owner.transform.position;

            // ✅ Use the function to get the nearest enemy position
            Vector2 targetPos = CalculateClosestEnemy(ActivationRadius); // pass the max range
            Vector3 dir;

            if (targetPos == Vector2.zero)
            {
                // No enemy found — shoot straight ahead
                dir = Owner.transform.up;
            }
            else
            {
                // Point towards the enemy
                dir = ((Vector3)targetPos - spawnPos).normalized;
            }

            // ✅ Spawn projectile and initialize with calculated direction
            GameObject proj = Instantiate(ProjectilePrefab, spawnPos, Quaternion.identity);
            ProjectileLogic logic = proj.GetComponent<ProjectileLogic>();
            if (logic != null)
                logic.Initialize(dir, Owner);
        }
    }

    public Vector2 CalculateClosestEnemy(float maxRange)
    {
        Vector2 calculatedLocation = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        // Get player/owner position
        Vector2 myPos = Owner != null ? (Vector2)Owner.transform.position : Vector2.zero;

        // Find all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(myPos, enemy.transform.position);
            if (dist < closestDistance && dist <= maxRange)
            {
                closestDistance = dist;
                calculatedLocation = enemy.transform.position;
            }
        }

        return calculatedLocation;
    }
}
