using System.Collections.Generic;
using UnityEngine;
public abstract class Ability : ScriptableObject
{
    public string Name;
    public string Description;
    public float Cooldown;
    public int Level;
    public float Damage;
    public float TravelSpeed;
    public float ActivationRadius = 10.0f;

    public GameObject Owner { get; private set; }

    public void Initialize(GameObject owner)
    {
        Owner = owner;
    }

    // Base logic all abilities share
    public virtual void Use()
    {
        Debug.Log($"Using ability: {Name} by {Owner.name}");
    }
}

public class AbilityHandler : MonoBehaviour
{
    public List<Ability> Abilities = new List<Ability>();
    private Dictionary<Ability, float> cooldownTimers = new Dictionary<Ability, float>();

    void Start()
    {
        // Initialize abilities with this object as the owner
        foreach (var ability in Abilities)
        {
            ability.Initialize(gameObject);
            cooldownTimers[ability] = 0f;
        }
    }

    void Update()
    {
        //////// This is logic to actually use
        //////// Tick down cooldowns
        //////List<Ability> keys = new List<Ability>(cooldownTimers.Keys);
        //////foreach (var ability in keys)
        //////{
        //////    cooldownTimers[ability] -= Time.deltaTime;
        //////}


        // Test logic
        // Cooldown ticking
        List<Ability> keys = new List<Ability>(cooldownTimers.Keys);
        foreach (var ability in keys)
        {
            cooldownTimers[ability] -= Time.deltaTime;
        }

        // TEMP: test fireball with space key
        if (Abilities.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            TryUseAbility(Abilities[0]);
        }
    }

    public void TryUseAbility(Ability ability)
    {
        if (cooldownTimers[ability] <= 0f)
        {
            ability.Use();
            cooldownTimers[ability] = ability.Cooldown;
        }
        else
        {
            Debug.Log($"{ability.Name} is on cooldown for {cooldownTimers[ability]}s.");
        }
    }

}