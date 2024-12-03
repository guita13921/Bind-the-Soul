using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;  // Range at which the enemy starts chasing the player
    public float attackRange = 2f;      // Range at which the enemy attacks the player
    public float moveSpeed = 3f;        // Speed at which the enemy moves

    protected NavMeshAgent agent;
    protected Animator animator;
    protected float distanceToPlayer;

    protected enum State
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    [SerializeField] protected State currentState;

    // Called on object initialization
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;
        currentState = State.Idle;
    }

    // Update called once per frame
    protected virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(player.position, transform.position);

        HandleState();
    }

    // Manage state transitions
    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chasing:
                ChaseState();
                break;
            case State.Attacking:
                AttackState();
                break;
            case State.Dead:
                DeadState();
                break;
        }
    }

    // Default behavior for Idle state
    protected virtual void IdleState()
    {
        if (distanceToPlayer < detectionRange)
        {
            currentState = State.Chasing;
            animator.SetBool("isWalking", true);
        }
    }

    // Default behavior for Chasing state
    protected virtual void ChaseState()
    {
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false);
        }
    }

    // Default behavior for Attacking state
    protected virtual void AttackState()
    {
        // Here, you can override this in child classes to implement different attack patterns
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
        }
    }

    // Default behavior for Dead state
    protected virtual void DeadState()
    {
        // Death logic (e.g., disable AI, play death animation, etc.)
        animator.SetTrigger("Die");
        agent.isStopped = true;
    }

    // Call this when the enemy dies
    public void Die()
    {
        currentState = State.Dead;
        // You can trigger additional logic like playing death animations, etc.
    }
}
