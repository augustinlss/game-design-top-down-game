using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinotaurController : MonoBehaviour
{
    public enum BossPhase { Intro, Phase1, Phase2, Phase3, Defeated }
    public enum BossState { Idle, Chasing, Attacking }

    [Header("Boss Stats")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float jumpForce = 10f;

    [Header("Attack Parameters")]
    public float meleeRange = 2f;
    public float projectileSpeed = 10f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Heavy Attack Parameters")]
    public ParticleSystem rockParticleSystem;
    public float rockParticleSpeed = 8f;
    public int rockParticleCount = 15;
    public float rockSpreadAngle = 45f;
    public float heavyAttackWindUpTime = 1.5f;
    public float heavyAttackRecoveryTime = 1f;

    [Header("Phase Thresholds")]
    public float phase2Threshold = 0.6f; // 60% health
    public float phase3Threshold = 0.3f; // 30% health

    [Header("Idle Behavior")]
    public float idleDuration = 2f;
    public float chaseDuration = 4f;
    public float detectionRange = 10f;

    [Header("Animation")]
    public Animator animator;

    public GameObject meleeCollider;
    private BossPhase currentPhase = BossPhase.Intro;
    private BossState currentState = BossState.Idle;
    private Rigidbody2D rb;
    private Transform player;
    private bool isGrounded;
    private Vector2 movementDirection;
    private float stateTimer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        StartCoroutine(BossBehavior());
    }

    private IEnumerator BossBehavior()
    {
        yield return StartCoroutine(IntroSequence());

        while (currentPhase != BossPhase.Defeated)
        {
            switch (currentPhase)
            {
                case BossPhase.Phase1:
                    yield return StartCoroutine(Phase1Behavior());
                    break;
                case BossPhase.Phase2:
                    yield return StartCoroutine(Phase2Behavior());
                    break;
                case BossPhase.Phase3:
                    yield return StartCoroutine(Phase3Behavior());
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(DefeatSequence());
    }

    private IEnumerator Phase1Behavior()
    {
        // Basic attack pattern
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(MeleeAttack());
            yield return new WaitForSeconds(0.5f);
        }

        yield return StartCoroutine(JumpAttack());
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.3f);
        }

        // Add heavy attack to Phase 1
        yield return StartCoroutine(HeavyAttack());
    }

    private IEnumerator Phase2Behavior()
    {
        yield return StartCoroutine(DashAttack());
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(MeleeAttack());
            yield return new WaitForSeconds(0.2f);
        }

        yield return StartCoroutine(JumpAttack());
        yield return StartCoroutine(HeavyAttack());
    }

    private IEnumerator Phase3Behavior()
    {
        yield return StartCoroutine(DashAttack());

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(JumpAttack());
        yield return StartCoroutine(MeleeAttack());
        yield return StartCoroutine(HeavyAttack());
        yield return StartCoroutine(UltimateAttack());
    }


    private IEnumerator HeavyAttack()
    {
        currentState = BossState.Attacking;
        
        animator.SetTrigger("HeavyAttackTrigger");

        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        rb.velocity = Vector2.zero; 
        
        yield return new WaitForSeconds(heavyAttackWindUpTime);

        // Attack phase
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", true);

        if (Vector2.Distance(transform.position, player.position) <= meleeRange)
        {
            // Deal heavy melee damage to player
        }

        EmitRockParticles();
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", true);
        
        yield return new WaitForSeconds(heavyAttackRecoveryTime);

        currentState = BossState.Chasing;
        animator.SetBool("isIdle", false);
    }




    private void Update()
    {
        UpdatePhase();
        UpdateState();
        UpdateAnimation();
    }

    private void UpdatePhase()
    {
        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage <= phase3Threshold && currentPhase != BossPhase.Phase3)
        {
            currentPhase = BossPhase.Phase3;
            // Trigger phase 3 transition effects or animations
        }
        else if (healthPercentage <= phase2Threshold && currentPhase != BossPhase.Phase2)
        {
            currentPhase = BossPhase.Phase2;
            // Trigger phase 2 transition effects or animations
        }
    }

    private void UpdateState()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            if (currentState == BossState.Idle)
            {
                currentState = BossState.Chasing;
                stateTimer = chaseDuration;
            }
            else
            {
                currentState = BossState.Idle;
                stateTimer = idleDuration;
            }
        }

        switch (currentState)
        {
            case BossState.Idle:
                rb.velocity = Vector2.zero;
                break;
            case BossState.Chasing:
                ChasePlayer();
                break;
            case BossState.Attacking:
                // Handled in specific attack coroutines
                break;
        }
    }

    private void ChasePlayer()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            movementDirection = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(movementDirection.x * moveSpeed, rb.velocity.y);

            // Flip the boss sprite to face the player
            if (movementDirection.x > 0)
            {
                transform.localScale = new Vector3(-5, 5, 1);
                FlipParticleSystemDirection(false);
            }
            else if (movementDirection.x < 0)
            {
                transform.localScale = new Vector3(5, 5, 1);
                FlipParticleSystemDirection(true);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

     private void FlipParticleSystemDirection(bool facingRight)
    {
        if (rockParticleSystem != null)
        {
            // Access the shape module of the particle system
            var shape = rockParticleSystem.shape;
            
            // Flip the Y rotation based on the Minotaur's facing direction
            shape.rotation = new Vector3(shape.rotation.x, facingRight ? -90f : 90f, shape.rotation.z);
        }
    }


    private void UpdateAnimation()
    {
        animator.SetBool("isIdle", currentState == BossState.Idle);
        animator.SetBool("isRunning", currentState == BossState.Chasing);
        animator.SetBool("isAttacking", currentState == BossState.Attacking);
    }


    private IEnumerator IntroSequence()
    {
        // Play intro animation or effects
        animator.SetTrigger("IntroTrigger");
        yield return new WaitForSeconds(3f);
        currentPhase = BossPhase.Phase1;
    }

    private IEnumerator MeleeAttack()
    {
        currentState = BossState.Attacking;
        animator.SetTrigger("MeleeAttackTrigger");
        if (Vector2.Distance(transform.position, player.position) <= meleeRange)
        {
            // Perform melee attack animation
            meleeCollider.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(0.1f);
            meleeCollider.SetActive(false);
            // Deal damage to player if still in range
        }
        currentState = BossState.Chasing;
    }


    private IEnumerator JumpAttack()
    {
        currentState = BossState.Attacking;
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
            // Perform ground slam when landing
            rb.AddForce(Vector2.down * jumpForce * 1.5f, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(0.5f);
        currentState = BossState.Chasing;
    }

    private IEnumerator DashAttack()
    {
        currentState = BossState.Attacking;
        animator.SetTrigger("DashAttackTrigger");
        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;
        currentState = BossState.Chasing;
    }

    private IEnumerator SummonMinions()
    {
        currentState = BossState.Attacking;
        animator.SetTrigger("SummonMinionsTrigger");
        // Spawn minion enemies to assist the boss
        // Implementation depends on your minion system
        yield return new WaitForSeconds(1f);
        currentState = BossState.Chasing;
    }

    private void EmitRockParticles()
    {
        if (rockParticleSystem != null)
        {
            // Set up the particle system
            var main = rockParticleSystem.main;
            main.startSpeed = rockParticleSpeed;

            var emission = rockParticleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, rockParticleCount));

            var shape = rockParticleSystem.shape;

            // Emit particles
            rockParticleSystem.Play();
        }
        else
        {
            Debug.LogWarning("Rock Particle System is not assigned!");
        }
    }

    private IEnumerator UltimateAttack()
    {
        currentState = BossState.Attacking;
        animator.SetTrigger("UltimateAttackTrigger");
        // Perform a powerful, screen-wide attack
        // This could be a combination of multiple attack types or a unique ability
        yield return new WaitForSeconds(2f);
        currentState = BossState.Chasing;
    }

    private IEnumerator DefeatSequence()
    {
        currentState = BossState.Idle;
        animator.SetTrigger("DefeatTrigger");
        // Play defeat animation or effects
        yield return new WaitForSeconds(3f);
        // Trigger level completion or next stage
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("HitTrigger");
        if (currentHealth <= 0)
        {
            currentPhase = BossPhase.Defeated;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void StopALL() {
        StopAllCoroutines();
    }

}