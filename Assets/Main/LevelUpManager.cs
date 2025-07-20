using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public GameObject LevelUpUI;
    public CharacterStatsScript playerStats;


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

    public void InreaseSpoeed()
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

        float increase = currentAttackCooldown * 0.1f;

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

        LevelUpUI.SetActive(false);
        Time.timeScale = 1f; // Resume game

    }
}
