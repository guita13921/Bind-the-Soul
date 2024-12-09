using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : BaseEnemyAI
{
    public Transform[] patrolPoints;    // Array of patrol points for the thug to move to
    private int currentPatrolIndex = 0; // Index to track current patrol point
    private bool isPatrolling = true;   // Flag to check if thug is patrolling
    private float patrolWaitTime = 2f;  // Time to wait at a patrol point
    private float patrolTimer = 0f;     // Timer to wait before moving to next patrol point

    private float health = 100f; // Health of the Wretched Thug

    private float attackMoveDistance = 1f; // How far the skeleton moves forward during the attack
    private float attackMoveDuration = 0.5f; // Duration of the forward movement during the attack
    private float attackMoveTime = 0f; // Timer for the forward movement during the attack

    [SerializeField] private float attackCooldown = 5f; // Cooldown time between attacks
    [SerializeField] private float attackCooldownTimer = 0f; // Timer to track attack cooldown

    protected override void Start()
    {
        base.Start();  // Calls the Start method of the base class
    }

    protected override void Update()
    {
        base.Update();  // Calls the Update method of the base class

        // If the enemy is in idle or chasing state, patrol or chase
        if (currentState == State.Idle || currentState == State.Chasing)
        {
            if (isPatrolling)
            {
                PatrolState();
            }
        }
        else if (currentState == State.Attacking)
        {
            // Handle attack movement and cooldown
            HandleAttackMovement();
            HandleAttackCooldown();
        }
    }

    protected override void IdleState()
    {
        base.IdleState();  // Calls the base class logic for IdleState

        // If in idle state and the player is close, start chasing
        if (distanceToPlayer < detectionRange)
        {
            currentState = State.Chasing;
            animator.SetBool("isWalking", true);
        }
    }

    protected override void ChaseState()
    {
        base.ChaseState();  // Calls the base class logic for ChaseState

        // If the player gets within attack range, switch to Attacking
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false);
        }
    }

    protected override void AttackState()
    {
        base.AttackState();  // Calls the base class logic for AttackState

        // If the player moves out of attack range, switch to chasing again
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            animator.SetBool("isWalking", true);
        }
    }

    protected override void DeadState()
    {
        base.DeadState();  // Calls the base class logic for DeadState
        agent.isStopped = true;
        gameObject.SetActive(false);  // Optionally disable the gameObject after death
    }

    // Patrol logic to move between points
    private void PatrolState()
    {
        if (patrolPoints.Length == 0) return;

        // Move the thug to the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;  // Loop through patrol points
            }
        }
    }

    // Handle the forward movement during attack
    private void HandleAttackMovement()
    {
        // Rotate towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 500f); // Smooth rotation

        // Increment the timer for attack movement
        attackMoveTime += Time.deltaTime;

        // Move the skeleton forward for a short duration during the attack
        if (attackMoveTime < attackMoveDuration)
        {
            Vector3 forwardMovement = transform.forward * (attackMoveDistance / attackMoveDuration) * Time.deltaTime;
            transform.position += forwardMovement; // Move the skeleton forward slightly
        }

        // After the attack movement is complete, reset the attack movement timer and handle cooldown
        if (attackMoveTime >= attackMoveDuration)
        {
            attackMoveTime = 0f; // Reset the timer
            currentState = State.Chasing; // Return to chasing state after attack movement
            animator.SetBool("isWalking", true);
        }
    }

    // Handle the cooldown after an attack
    private void HandleAttackCooldown()
    {
        attackCooldownTimer += Time.deltaTime;

        // Once the cooldown is over, the enemy can attack again
        if (attackCooldownTimer >= attackCooldown)
        {
            attackCooldownTimer = 0f; // Reset the cooldown timer
            currentState = State.Chasing; // Return to the chasing state after cooldown
        }
    }

    // Take damage and check if the thug dies
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
}
