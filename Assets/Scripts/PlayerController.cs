using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 5f;
    public float sprintStaminaCost = 10f;

    public float jumpForce = 7f;
    public bool isGrounded = true;

    private Rigidbody2D rb;
    private Vector2 movement;

    private bool isSprinting;
    private float currentSpeed;

    public float treeOpacityOnCollision = 0.5f;
    private SpriteRenderer treeSprite;
    private SpriteRenderer lowerTreeSprite;
    private SpriteRenderer upperTreeSprite;

    private Animator animator; // Animator reference

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // Sprinting logic
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f)
        {
            isSprinting = true;
            currentSpeed = sprintSpeed;
            stamina -= sprintStaminaCost * Time.deltaTime;
        }
        else
        {
            isSprinting = false;
            currentSpeed = moveSpeed;

            if (stamina < maxStamina)
                stamina += staminaRegenRate * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (movement.x > 0) // Moving right
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement.x < 0) // Moving left
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }

        bool isMoving = movement.magnitude > 0; 
        animator.SetBool("isSprinting", isMoving && isSprinting); 
        animator.SetBool("isWalking", isMoving && !isSprinting); 
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    void Jump()
    {
        //trigger animation for jumping
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tree"))
        {
            lowerTreeSprite = collision.gameObject.transform.Find("Lower").GetComponent<SpriteRenderer>();
            upperTreeSprite = collision.gameObject.transform.Find("Upper").GetComponent<SpriteRenderer>();

            if (lowerTreeSprite != null && upperTreeSprite != null)
            {
                ChangeTreeOpacity(lowerTreeSprite, treeOpacityOnCollision);
                ChangeTreeOpacity(upperTreeSprite, treeOpacityOnCollision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tree") && lowerTreeSprite != null && upperTreeSprite != null)
        {
            ChangeTreeOpacity(lowerTreeSprite, 1f);
            ChangeTreeOpacity(upperTreeSprite, 1f);
        }
    }

    void ChangeTreeOpacity(SpriteRenderer spriteRenderer, float opacity)
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = opacity;
        spriteRenderer.color = spriteColor;
    }
}
 