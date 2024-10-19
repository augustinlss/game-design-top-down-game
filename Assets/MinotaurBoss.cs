using System.Collections;
using UnityEngine;

public class MinotaurBoss : MonoBehaviour
{
    public float moveSpeed = 2f;                  // Speed at which the Minotaur moves
    public float dashSpeed = 6f;                  // Speed for dashing
    public float closeRangeDistance = 2f;         // Distance for a close-range attack
    public float heavyAttackCooldown = 5f;        // Cooldown for heavy attack
    public float lightAttackCooldown = 1.5f;      // Cooldown for light attack
    public float dashCooldown = 3f;               // Cooldown for dashing
    public float idleDuration = 2f;               // Duration to remain idle
    public Transform player;                      // Reference to the player's position
    public Vector2 arenaSize = new Vector2(10f, 10f); // Size of the arena

    private Animator anim;
    private bool canAttack = true;                // Attack cooldown control
    private bool canMove = true;                  // Movement control
    private bool isDashing = false;               // Dash state control
    private float heavyAttackTimer;
    private float lightAttackTimer;
    private float dashTimer;
    private BossState currentState;
    private Vector3 originalScale;  
    public GameObject rayParticlePrefab;              // Store original scale for flipping

    private enum BossState
    {
        Idle,
        Moving,
        Chasing,
        Dashing
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        heavyAttackTimer = heavyAttackCooldown;    // Initialize heavy attack timer
        lightAttackTimer = lightAttackCooldown;    // Initialize light attack timer
        dashTimer = dashCooldown;                  // Initialize dash cooldown timer
        currentState = BossState.Idle;             // Start with idle state
        originalScale = transform.localScale;      // Store the original scale for flipping
        StartCoroutine(BossBehaviorLoop());
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Moving:
                RandomMovement();  // Random movement in arena
                break;
            case BossState.Chasing:
                if (distanceToPlayer > closeRangeDistance && !isDashing)
                {
                    MoveTowardsPlayer(distanceToPlayer);  // Chase player for attack
                }
                break;
            case BossState.Dashing:
                // Handle dashing (handled in coroutine)
                break;
        }

        // Handle attacks
        if (canAttack)
        {
            if (distanceToPlayer <= closeRangeDistance)
            {
                StartCoroutine(PerformLightAttack());  // Light attack if close
            }
            else if (heavyAttackTimer <= 0)
            {
                StartCoroutine(PerformHeavyAttack());  // Heavy attack if timer is ready
            }
        }

        // Reduce timers
        heavyAttackTimer -= Time.deltaTime;
        lightAttackTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;
    }

    // Main behavior loop to control state transitions
    private IEnumerator BossBehaviorLoop()
    {
        while (true)
        {
            // Idle for a while
            currentState = BossState.Idle;
            anim.Play("Minotaur_idle");
            canMove = false;
            yield return new WaitForSeconds(idleDuration);

            // Decide the next move (chase, dash, or random movement)
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.4f) // 40% chance of chasing the player
            {
                currentState = BossState.Chasing;
                canMove = true;
                yield return new WaitForSeconds(3f);  // Chase for a few seconds
            }
            else if (randomValue < 0.7f && dashTimer <= 0) // 30% chance of dashing
            {
                currentState = BossState.Dashing;
                yield return StartCoroutine(PerformDash());
            }
            else // 30% chance of random movement
            {
                currentState = BossState.Moving;
                canMove = true;
                yield return new WaitForSeconds(3f);  // Move randomly for a few seconds
            }
        }
    }

    // Method to randomly move around the arena
    private void RandomMovement()
    {
        if (canMove)
        {
            // Move to a random position within the arena bounds
            Vector2 randomPosition = new Vector2(Random.Range(-arenaSize.x / 2, arenaSize.x / 2), Random.Range(-arenaSize.y / 2, arenaSize.y / 2));
            FlipSprite(randomPosition.x);  // Flip based on movement direction
            transform.position = Vector2.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
            anim.Play("Minotaur_run");
        }
    }

    // Method to move towards player
    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        FlipSprite(player.position.x);  // Flip based on player's position

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Set animation to running if moving
        if (distanceToPlayer > closeRangeDistance)
        {
            anim.Play("Minotaur_run");
        }
        else
        {
            anim.Play("Minotaur_idle");
        }
    }

    // Method to flip the sprite based on movement direction
    private void FlipSprite(float targetXPosition)
    {
        if (targetXPosition > transform.position.x)
        {
            // Moving right, face right
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetXPosition < transform.position.x)
        {
            // Moving left, face left
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Coroutine for dashing
    private IEnumerator PerformDash()
    {
        canMove = false;
        isDashing = true;

        // Choose whether to dash toward the player or a random spot
        Vector2 dashTarget;
        float dashChoice = Random.Range(0f, 1f);
        if (dashChoice < 0.5f)
        {
            dashTarget = player.position;  // Dash toward the player
        }
        else
        {
            // Dash toward a random position in the arena
            dashTarget = new Vector2(Random.Range(-arenaSize.x / 2, arenaSize.x / 2), Random.Range(-arenaSize.y / 2, arenaSize.y / 2));
        }

        FlipSprite(dashTarget.x);  // Flip based on dash direction

        // Trigger dash animation
        anim.Play("Minotaur_run");

        // Move toward the dash target
        while (Vector2.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Reset dash
        dashTimer = dashCooldown;
        isDashing = false;
        canMove = true;
    }

    // Coroutine for light attack (melee close-range attack)
    private IEnumerator PerformLightAttack()
    {
        canMove = false;  // Stop movement while attacking
        canAttack = false;

        // Trigger light attack animation
        anim.Play("Minotaur_attack_light");

        yield return new WaitForSeconds(0.5f);  // Simulate attack delay

        if (Vector2.Distance(transform.position, player.position) <= closeRangeDistance)
        {
            Debug.Log("Minotaur performs light attack");
            // Apply damage to player
        }

        yield return new WaitForSeconds(1f);  // Cooldown after attack
        lightAttackTimer = lightAttackCooldown;
        canMove = true;
        canAttack = true;
    }

    // Coroutine for heavy attack (long-range AOE attack)
    private IEnumerator PerformHeavyAttack()
    {
        canMove = false;  // Stop movement while attacking
        canAttack = false;

        // Trigger heavy attack animation
        anim.Play("Minotaur_attack_heavy");

        yield return new WaitForSeconds(0.5f);  // Simulate attack delay

        Debug.Log("Minotaur performs heavy attack");
        FireRaysInAllDirections();

        yield return new WaitForSeconds(1f);  // Cooldown after attack
        heavyAttackTimer = heavyAttackCooldown;
        canMove = true;
        canAttack = true;
    }

    // Method to fire rays/projectiles in multiple directions with particles
    private void FireRaysInAllDirections()
    {
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1, 1).normalized,  // Diagonal up-right
        };

        foreach (Vector2 dir in directions)
        {
            // Raycast to simulate ray hit
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 10f);

            // Instantiate particle system for the ray
            GameObject rayEffect = Instantiate(rayParticlePrefab, transform.position, Quaternion.identity);

            // Align the particle system with the ray direction
            rayEffect.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

            // Destroy the particle effect after its lifetime
            Destroy(rayEffect, 1f);

            // Optionally handle ray hit logic
            if (hit.collider != null)
            {
                Debug.Log("Ray hit: " + hit.collider.name);
                // Apply damage if ray hits player or object
            }

            Debug.DrawRay(transform.position, dir * 10f, Color.red, 1f);
        }
    }

}
