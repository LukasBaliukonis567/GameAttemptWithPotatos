using Unity.VisualScripting;
using UnityEngine;

public class BadGuyLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform target; //The target to move forwar to
    private float speed = 0.0f; //The speed at which the bad guy moves towards the target
    void Start()
    {
        CharacterStatsScript BadGuyStats = GetComponent<CharacterStatsScript>();
        speed = BadGuyStats.speed; // Get the speed from the CharacterStatsScript component
    }

    // Update is called once per frame
    void Update()
    {
        //only seek the player if the target is set

        if (target != null)
        {   
            transform.position = Vector3.MoveTowards(transform.position,
                target.position,
                speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Only trigger if the collided object has the tag "Player"
        {
            MovementScript PlayerHandler = other.GetComponent<MovementScript>();
            CharacterStatsScript PlayerStats = other.GetComponent<CharacterStatsScript>();
            CharacterStatsScript BadGuyStats = GetComponent<CharacterStatsScript>();

            PlayerStats.TakeDamage(BadGuyStats.damage);

            if (PlayerStats.isKillingBlow())
            {
                target = null; // Stop seeking the player after death
            }
        }

    }
}
