using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAnimations : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem fireBreathEffect;
    [SerializeField] private Transform player; // Reference to the player
    private BossRotationWithAnimation movementController;

    public void PerformFireBreath()
    {
        Debug.Log("Dragon uses Fire Breath!");
        animator.SetTrigger("FireBreath");
        if (fireBreathEffect != null) fireBreathEffect.Play();

    }

    public void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    public IEnumerator DashToPlayer()
    {
        float dashDuration = 1f; // Duration of the dash in seconds
        float dashSpeed = 15f;   // Speed of the dash

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.position;

        // Calculate the direction to dash
        Vector3 dashDirection = (targetPosition - startPosition).normalized;
        dashDirection.y = 0; // Ensure the boss stays on the ground

        float elapsedTime = 0f;

        // Perform the dash over the specified duration
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move the boss toward the player
            transform.position += dashDirection * dashSpeed * Time.deltaTime;

            yield return null;
        }
        //Debug.Log("Forward Rush completed!");
    }


    public void PerformForwardRush()
    {
        Debug.Log("Dragon uses ForwardRush!");
        LookAtPlayer();
        animator.SetTrigger("ForwardRush");
        //if (fireBreathEffect != null) fireBreathEffect.Play();
        StartCoroutine(DashToPlayer());
    }

    public void PerformLaser()
    {
        Debug.Log("Dragon uses Laser!");
        animator.SetTrigger("Laser");
        //if (fireBreathEffect != null) fireBreathEffect.Play();
    }

    public void PerformClawSwipe()
    {
        Debug.Log("Dragon performs Claw Swipe!");
        if(Random.value > 0.5f){
            animator.SetTrigger("ClawSwipe1");
        }else{
            animator.SetTrigger("ClawSwipe2");
        }
    }

    public void PerformTailSweep()
    {
        Debug.Log("Dragon performs Tail Sweep!");

        animator.SetTrigger("TailSweep");
    }

    public void PerformSummonMinions()
    {
        Debug.Log("Dragon summons minions!");

        animator.SetTrigger("SummonMinions");

    }

    public void PerformKnockBack()
    {
        Debug.Log("Dragon performs KnockBack!");

        animator.SetTrigger("KnockBackRoar");

    }

    public void LockMovement()
    {
        //Debug.Log("Movement locked.");
        if (movementController) movementController.LockMovement(); // Call LockMovement from BossRotationWithAnimation
        Debug.Log("LockMomoent");
    }

    public void UnlockMovement()
    {
        //Debug.Log("Movement unlocked.");
        if (movementController) movementController.UnlockMovement(); // Call UnlockMovement from BossRotationWithAnimation
    }

}
