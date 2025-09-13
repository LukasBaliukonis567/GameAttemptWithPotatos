using System.Security.Cryptography;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float abilityInterval = 10.0f;
    public float timeToDefeath = 120.0f; // seconds
    CharacterStatsScript Boss_stats;


    public string[] Abilities = new string[] { "None" };
    public int Ability_Picker(string[] Abilities)
    {
        int randomIndex = Random.Range(0, Abilities.Length);
        return randomIndex;
    }


    public string[] Environmental_abilities = new string[] { "None" };

    public int Environmental_Ability_Picker(string[] Environmental_abilities)
    {
        int randomIndex = Random.Range(0, Environmental_abilities.Length);
        return randomIndex;
    }


    public void Start()
    {
        InvokeRepeating("UseAbility", abilityInterval, abilityInterval);
        Invoke("DefeatBoss", timeToDefeath);
        Boss_stats = GetComponent<CharacterStatsScript>();
    }

    public void DefeatBoss()
    {
       Debug.Log("Boss defeated due to time limit.");
       Boss_stats.TakeDamage(9999999);
    }

    public void UseAbility()
    {
        Invoke(Abilities[Ability_Picker(Abilities)], 0.0f);
    }

    // TO:DO 
    // Make Logic for a "dash"
    // make logic for aoe stomps during the dash
    // make some sort of projectiles omit from the aoe stomps
}