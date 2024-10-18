using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private bool canMove = true;  // Flag to control movement

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            body.velocity = new Vector2(horizontal, vertical);
        }
        else
        {
            // Stop movement when canMove is false
            body.velocity = Vector2.zero;
        }
    }

    public void SetMovement(bool enabled)
    {
        canMove = enabled;
    }

    // Optional: Handle collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            // Logic for what happens when colliding with an NPC
            Debug.Log("Collided with NPC!");
            // You can also disable movement or trigger a dialogue here
            // SetMovement(false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            // Enable movement again when exiting collision
            SetMovement(true);
        }
    }
}
