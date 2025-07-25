using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementScript : MonoBehaviour
{
    public const float diagononalSpeed = 3.5f;
    private float pointsPickedUp = 0.0f;
    private int currentLevel = 0;

    public LevelUpUI LevelUpTrigger;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        CharacterStatsScript PlayerStats = GetComponent<CharacterStatsScript>();



        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rigidbody2D.position += (Vector2.up + Vector2.right) * Time.deltaTime * diagononalSpeed;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rigidbody2D.position += (Vector2.up + Vector2.left) * Time.deltaTime * diagononalSpeed;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            rigidbody2D.position += (Vector2.down + Vector2.right) * Time.deltaTime * diagononalSpeed;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rigidbody2D.position += (Vector2.down + Vector2.left) * Time.deltaTime * diagononalSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rigidbody2D.position += Vector2.up * Time.deltaTime * PlayerStats.speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rigidbody2D.position += Vector2.down * Time.deltaTime * PlayerStats.speed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rigidbody2D.position += Vector2.left * Time.deltaTime * PlayerStats.speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidbody2D.position += Vector2.right * Time.deltaTime * PlayerStats.speed;
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Overlapping with: " + other.gameObject.name);
        SphereSpawner sphere = other.GetComponent<SphereSpawner>();
        if (sphere != null && !sphere.GetIsHpOrb())
        {
            pointsPickedUp += sphere.XPValue;
            Debug.Log($"Trigger overlap! XP: {sphere.XPValue}");

            int nextLevelXP = LevelAndXPManagerScript.Instance.GetXPForNextLevelThreshold(currentLevel);

            if (pointsPickedUp >= nextLevelXP)
            {

                if (LevelUpManager.Instance == null)
                {
                    Debug.LogError("Error: 205 \n big oopsie happened ");
                    return;
                }

                currentLevel++;
                var upgrades = LevelUpManager.Instance.RollLevelUpOptions();
                LevelUpTrigger.ShowLevelUpScreen(upgrades);

                Debug.Log($"Level up! New level: {currentLevel}");
            }

            Destroy(other.gameObject);
            Debug.Log($"Total points picked up: {pointsPickedUp}");
        }
        if (sphere != null && sphere.GetIsHpOrb())
        {
            CharacterStatsScript ps = GetComponent<CharacterStatsScript>();
            ps.health += sphere.amountHealed;
            Debug.Log($"Picked up health orb! Health increased by: {sphere.amountHealed}");
            Destroy(other.gameObject);
        }

        BadGuyLogic Opponent = other.GetComponent<BadGuyLogic>();
        RangerLogic RangedOpponent = other.GetComponent<RangerLogic>();
        ProjectileStats projectileStats = other.GetComponent<ProjectileStats>();

        CharacterStatsScript playerStats = GetComponent<CharacterStatsScript>();
        if (Opponent != null || RangedOpponent != null)
        {
            playerStats.TryDealContactDamage(other.gameObject); 
        }

    }
}
