using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private ParticleSystem gun;
    private Animator anim;
    public float speed = 3;
    private bool isMoving = false;

    // Animation names
    public string walkAnim = "run"; // The name of your walking animation
    public string idleAnim = "idle"; // The name of your idle animation

    // Store initial scale
    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();

        // Save the initial scale of the player
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal movement input
        float x = Input.GetAxisRaw("Horizontal");

        // Determine if the player is moving
        isMoving = Mathf.Abs(x) > 0.01f;

        // Move the character horizontally
        transform.position += new Vector3(x * speed * Time.deltaTime, 0, 0);

        // Flip the character by adjusting the localScale if moving left or right
        if (x < 0)
        {
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z); // Flip left
        }
        else if (x > 0)
        {
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z); // Flip right
        }

        // Play the appropriate movement animation
        if (isMoving)
        {
            anim.Play(walkAnim); // Play walking animation if moving
        }
        else
        {
            anim.Play(idleAnim); // Play idle animation if not moving
        }
    }
}
