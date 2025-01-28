using UnityEngine;
using UnityEngine.AI;

public class BossRotationWithAnimation : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed; // Movement speed
    [SerializeField] private float rotationThreshold; // Angle (in degrees) to start turning
    [SerializeField] private float stopDistance; // Stop moving when close to the player

    [Header("References")]
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private Animator animator; // Animator for controlling animations
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for movement

    private void Start()
    {
        if (!player) player = GameObject.FindWithTag("Player").transform; // Find player if not set
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

        agent.updateRotation = false; // Disable NavMeshAgent's automatic rotation
    }

    private void Update()
    {
        if (player == null) return;

        // Get direction and angle to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

        // Handle movement and rotation
        HandleRotationAndMovement(angleToPlayer, directionToPlayer);
    }

    private void HandleRotationAndMovement(float angleToPlayer, Vector3 directionToPlayer)
    {
        // Check distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Stop moving if close enough
        if (distanceToPlayer <= stopDistance)
        {
            StopMovement();
            return;
        }

        // Handle rotation based on the angle to the player
        if (Mathf.Abs(angleToPlayer) > rotationThreshold) // Boss needs to turn
        {
            if (angleToPlayer > 0) // Turn Right
            {
                PlayTurnRightAnimation();
            }
            else // Turn Left
            {
                PlayTurnLeftAnimation();
            }
        }
        else // Boss is facing the player
        {
            MoveForward(directionToPlayer);
        }
    }

    private void MoveForward(Vector3 directionToPlayer)
    {
        // Play the forward movement animation
        animator.SetBool("IsMoving", true);
        animator.SetBool("IsTurningLeft", false);
        animator.SetBool("IsTurningRight", false);

        // Move toward the player using NavMeshAgent
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
    }

    private void PlayTurnLeftAnimation()
    {
        // Trigger the turn left animation
        animator.SetBool("IsTurningLeft", true);
        animator.SetBool("IsTurningRight", false);
        animator.SetBool("IsMoving", false);
    }

    private void PlayTurnRightAnimation()
    {
        // Trigger the turn right animation
        animator.SetBool("IsTurningRight", true);
        animator.SetBool("IsTurningLeft", false);
        animator.SetBool("IsMoving", false);
    }

    private void StopMovement()
    {
        // Stop all movement and reset animations
        agent.SetDestination(transform.position);
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsTurningLeft", false);
        animator.SetBool("IsTurningRight", false);
    }

    private void StartRotation()
    {
        agent.speed = 0;
    }

    /// Stops the NavMeshAgent and locks movement.
    public void LockMovement()
    {
        if (agent)
        {
            agent.speed = 0;
            agent.isStopped = true; // Stop the NavMeshAgent
            agent.velocity = Vector3.zero; // Ensure the agent stops moving immediately
            StopMovement();
        }
    }

    /// <summary>
    /// Resumes the NavMeshAgent and unlocks movement.
    /// </summary>
    public void UnlockMovement()
    {
        if (agent)
        {
            agent.isStopped = false; // Resume NavMeshAgent movement
        }
    }
    

}
