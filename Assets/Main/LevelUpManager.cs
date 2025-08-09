using System.Linq;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public GameObject LevelUpUI;
    public CharacterStatsScript playerStats;
    public static LevelUpManager Instance { get; private set; }
    void Awake() => Instance = this;
    public void IncreaseAttackDamage()
    {
        float currentDamage = playerStats.damage;
        const float multiplier = 1.364f;

        float calculatedNewDamage = currentDamage * multiplier;

        playerStats.damage = calculatedNewDamage;
        CloseUI();
    }

    public void IncreaseArmor()
    {
        float currentArmor = playerStats.armor;

        const int increase = 5;

        float calculatedNewArmor = currentArmor + increase;

        playerStats.armor = calculatedNewArmor;
        CloseUI();
    }

    public void IncreaseSpeed()
    {
        float currentSpeed = playerStats.speed;

        float increase = currentSpeed * 0.1f;

        float calculatedNewSpeed = currentSpeed + increase;

        playerStats.speed = calculatedNewSpeed;
        CloseUI();
    }

    public void IncreaseAttackSpeed()
    {
        float currentAttackCooldown = playerStats.attackCooldown;

        float increase = currentAttackCooldown * 0.9f;

        float calculatedNewAttackSpeed = currentAttackCooldown + increase;

        playerStats.attackCooldown = calculatedNewAttackSpeed;
        CloseUI();
    }

    public void IncreaseHealth()
    {
        float currentHealth = playerStats.health;

        float increase = currentHealth * 0.1f;

        float calculatedNewHealth = currentHealth + increase;

        playerStats.health = calculatedNewHealth;
        CloseUI();
    }

    public void CloseUI()
    {
        Time.timeScale = 1f;
    }

    public string[] RollLevelUpOptions()
    {
        string[] allOptions = { "damage", "armor", "speed", "health", "attackSpeed" };
        return allOptions.OrderBy(x => Random.value).Take(3).ToArray();
    }

    public void ApplyUpgrade(string upgrade)
    {
        switch (upgrade.ToLowerInvariant())
        {
            case "damage":
                IncreaseAttackDamage();
                break;
            case "armor":
                IncreaseArmor();
                break;
            case "speed":
                IncreaseSpeed(); // Typo in method name! See below
                break;
            case "health":
                IncreaseHealth();
                break;
            case "attackspeed":
                IncreaseAttackSpeed();
                break;
            default:
                Debug.LogWarning($"Unknown upgrade type: {upgrade}");
                CloseUI(); // fallback just in case
                break;
        }
    }

}
