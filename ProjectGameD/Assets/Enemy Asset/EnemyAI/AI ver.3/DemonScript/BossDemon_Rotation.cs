using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossDemon_Rotation : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed; // Movement speed
    [SerializeField] private float stopDistance; // Stop moving when close to the player
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSpeed_laser;
    [SerializeField] private bool isLock = false;

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

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
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
        
        MoveForward(directionToPlayer);
    }

    private void MoveForward(Vector3 directionToPlayer)
    {
        LookAtPlayer();
        animator.SetBool("IsMoving", true);
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
    }

    private void StopMovement()
    {
        // Stop all movement and reset animations
        agent.SetDestination(transform.position);
        animator.SetBool("IsMoving", false);
    }

    private void StartRotation()
    {
        agent.speed = 0;
    }

    private void LookAtPlayer()
    {
        if(isLock == true) return;
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep rotation level (prevent looking up/down)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void LookAtPlayer_w0Lock()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep rotation level (prevent looking up/down)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void LookAtPlayer_WhenLaser()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep rotation level (prevent looking up/down)

        // Rotate the direction 45 degrees to the left
        direction = Quaternion.Euler(0, -45, 0) * direction;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed_laser);
    }

    /// Stops the NavMeshAgent and locks movement.
    public void LockMovement()
    {
        if (agent)
        {
            isLock = true;
            agent.speed = 0;
            agent.isStopped = true; // Stop the NavMeshAgent
            agent.velocity = Vector3.zero; // Ensure the agent stops moving immediately
            StopMovement();
        }
    }
    

    public void UnlockMovement()
    {
        if (agent)
        {   
            isLock = false;
            agent.isStopped = false; // Resume NavMeshAgent movement
        }
    }

    public void RequestLookAtplayer(){
        LookAtPlayer();
    }
    
    public void RequestLookAtplayer_laser(){
        LookAtPlayer_WhenLaser();
    }

    /*
    public void RequestInsideLookAtPlayer()
    {
        if (player == null) return; // Ensure the player reference exists

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation only on the Y-axis

        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer); // Instantly face the player
        }
    }
    */

    public void RequestLookAtPlayer_w0Lock()
    {
        LookAtPlayer_w0Lock();
    }
}
