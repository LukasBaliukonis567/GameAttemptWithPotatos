using UnityEngine;

public class BadGuyLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform target; //The target to move forwar to
    public float speed = 4f; //The speed at which the bad guy moves towards the target
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            target.position,
            speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Only trigger if the collided object has the tag "Player"
        {
            //Kill Player on collision
            Destroy(other.gameObject);
            Debug.Log("I died!");

        }

    }
}
