using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    public float rotationSpeed = 5f; // Speed of rotation during dashing

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isDashing;
    private bool isAttacking;
    private float dashTimeCounter;

    public float dashStrikeDistance = 15f;
    public float dashStrikeCooldown = 1f;
    private float dashStrikeCooldownTimer = 0f;
    private bool isDashStriking = false;

    public GameObject dashStrikeParticleEffect;
    public GameObject attackCollider;

    public GameObject minotaurBoss;
    public GameObject trail;
    public float dashStrikeSpeed;

    public int maxStamina = 100;
    public int currentStamina;
    public float staminaRegenRate = 10f;  
    public int dashStaminaCost = 20;      
    public int dashStrikeStaminaCost = 30;
    public Slider staminaSlider;

    private float staminaRegenAccumulator = 0f;
    
    public TutorialManager tutorialManager;
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Jump effect object
    public GameObject jumpEffectObject; // Assign this in the inspector
    private Animator jumpEffectAnimator; // Animator for the jump effect

    public GameObject particleSys;
    public GameObject minotaurCollider;

    private Quaternion originalRotation; // Store the original rotation of the player
    private Quaternion targetRotation;   // Target rotation when dashing in the air
    private bool isRotating;             // Flag to track whether the player is rotating

    private DashStrikeCollider strikeCollider;

    public AudioClip attackSound;
    public AudioClip dashSound;
    public AudioClip dashAttackSound;

    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        strikeCollider = GetComponentInChildren<DashStrikeCollider>();

        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;

        // Store the player's original rotation to reset after dashing
        originalRotation = transform.rotation;

        // Get the jump effect's animator component
        if (jumpEffectObject != null)
        {
            jumpEffectAnimator = jumpEffectObject.GetComponent<Animator>();
            jumpEffectObject.SetActive(false); // Ensure it starts hidden
        }
    }

    private void Update()
    {
        if (!isDashing && !isDashStriking)
        {
            Move();
            FlipPlayer();
        }

        // Smoothly rotate the player if currently rotating
        if (isRotating)
        {
            SmoothRotateToTarget();
        }

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump = true; // Allow double jump after the first jump
            }
            else if (canDoubleJump)
            {
                Jump();
                TriggerDoubleJumpEffect(); // Trigger jump effect on double jump
                canDoubleJump = false; // Disable further double jumps until grounded
            }
        }

        // Dashing (only if enough stamina)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isDashStriking && currentStamina >= dashStaminaCost)
        {
            StartDash();
        }

        if (isDashing)
        {
            ContinueDash();
        }

        // DashStrike initiation (only if enough stamina)
        if (Input.GetMouseButtonDown(0) && !isGrounded && !canDoubleJump && isDashing && dashStrikeCooldownTimer <= 0 && currentStamina >= dashStrikeStaminaCost)
        {
            StartDashStrike();
        }

        // Perform the DashStrike movement
        if (isDashStriking)
        {
            PerformDashStrike();
        }

        // Update DashStrike cooldown
        if (dashStrikeCooldownTimer > 0)
        {
            dashStrikeCooldownTimer -= Time.deltaTime;
        }

        // Stamina regeneration (when not dashing or dash striking)
        if (!isDashing && !isDashStriking && currentStamina < maxStamina)
        {
            RegenerateStamina();
        }

        // Attacking
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartAttack();
        }

        // Update stamina slider
        staminaSlider.value = currentStamina;
    }

    private void RegenerateStamina()
    {
        // Accumulate stamina regeneration over time
        staminaRegenAccumulator += staminaRegenRate * Time.deltaTime;

        // If we have accumulated at least 1 stamina point, add it to current stamina
        if (staminaRegenAccumulator >= 1f)
        {
            int staminaToAdd = Mathf.FloorToInt(staminaRegenAccumulator);
            currentStamina += staminaToAdd;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);  // Ensure stamina doesn't exceed max

            // Subtract the added stamina from the accumulator
            staminaRegenAccumulator -= staminaToAdd;
        }

        // Update stamina slider
        staminaSlider.value = currentStamina;
    }

    private void StartDashStrike()
    {
        isDashStriking = true;
        strikeCollider.isDashing = true;
        isDashing = false; // Stop dashing and begin dash strike
        dashStrikeCooldownTimer = dashStrikeCooldown;
        currentStamina -= dashStrikeStaminaCost;
        if (trail != null)
        {
            PlaySound(dashAttackSound);
            trail.SetActive(true); // Show the trail
        }

        Debug.Log("Dash Strike initiated");
    }

    // Perform DashStrike movement over time
    private void PerformDashStrike()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = minotaurBoss.transform.position;

        // Move the player towards the target
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, dashStrikeSpeed * Time.deltaTime);

        // If the player reaches the target, stop the DashStrike
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            isDashStriking = false; // Stop Dash Strike
            strikeCollider.isDashing = false;
            Debug.Log("Dash Strike complete");
            if (trail != null)
            {
                StartCoroutine(stoptrail());
            }
        }
    }

    IEnumerator stoptrail()
    {
        yield return new WaitForSeconds(1);
        trail.SetActive(false);
    }



    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (Mathf.Abs(moveInput) > 0.1f && isGrounded)
        {
            if (!isAttacking)
            {
                animator.Play("run");
            }
        }
        else if (isGrounded)
        {
            if (!isAttacking)
            {
                animator.Play("idle");
            }
        }
    }

    private void FlipPlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(3, 3, 1); // Facing right
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-3, 3, 1); // Facing left
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeCounter = dashTime;

        currentStamina -= dashStaminaCost;

        // Check if the player is in the air (not grounded) and rotate smoothly
        if (!isGrounded)
        {
            // Determine the target rotation based on the direction the player is facing
            float rotationAngle = (transform.localScale.x > 0) ? -45f : 45f; // -45 for right, 45 for left
            targetRotation = Quaternion.Euler(0, 0, rotationAngle);
            isRotating = true; // Start rotating towards the target rotation
        }

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * dashSpeed, rb.velocity.y);
        animator.Play("Run"); // Replace this with dash animation if you have one
    }

    private void ContinueDash()
    {
        if (dashTimeCounter > 0)
        {
            dashTimeCounter -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
            ResetRotation(); // Reset the rotation when dash ends
        }
    }

    private void SmoothRotateToTarget()
    {
        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Stop rotating once the rotation is close enough to the target
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }

    private void ResetRotation()
    {
        // Smoothly rotate back to the original rotation
        targetRotation = originalRotation;
        isRotating = true; // Start rotating back to the original rotation
    }

    private void StartAttack()
    {
        isAttacking = true;
        PlaySound(attackSound);
        attackCollider.SetActive(true);
        animator.SetBool("isAttacking", true);
        animator.Play("attack");
        Invoke("StopAttack", 0.3f);
    }

    private void StopAttack()
    {
        isAttacking = false;
        attackCollider.SetActive(false);
        animator.SetBool("isAttacking", false);
    }

    private void TriggerDoubleJumpEffect()
    {
        if (jumpEffectObject != null)
        {
            // Make the jump effect object visible
            jumpEffectObject.SetActive(true);

            // Optionally hide the effect after the animation finishes
            Invoke("HideJumpEffect", 0.3f); // Adjust the time to match the animation length
        }
    }

    private void HideJumpEffect()
    {
        jumpEffectObject.SetActive(false); // Hide the jump effect object
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false;
            isDashStriking = false;
            ResetRotation(); // Reset rotation when grounded
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void Die() {
        tutorialManager.DeathHandler();
        transform.GetComponent<PlayerController>().enabled = false;
        minotaurBoss.GetComponent<MinotaurController>().enabled = false;
        particleSys.SetActive(false);
        minotaurCollider.GetComponent<BoxCollider2D>().enabled = false;
        animator.Play("player_death");
    }

    public void StopALL() {
        StopAllCoroutines();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
