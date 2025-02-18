using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class BossDemon_Animation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private BossDemon_Rotation movementController;
    private float rotationSpeed = 0.5f; // Adjust rotation speed as needed

    [Header("Malee")]
    [SerializeField] private bool isAttacking; // Reference to the flamethr
    [SerializeField] private List<BoxCollider> attackHitboxes = new List<BoxCollider>(); 
    
    [Header("Dashing")]
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;

    [Header("Bullet")]
    [SerializeField] private int numberOfBullets;
    [SerializeField] private float bulletDelay = 0.5f;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private Transform firePoint;
    private Vector2 uiOffset;

    [Header("Indicator")]
    [SerializeField] AttackIndicatorController attackIndicatorController;


    private void Update()
    {
        
    }


    IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
    {
        // Prevent starting multiple shooting coroutines
        for (int i = 0; i < numberOfBullets; i++)
        {
                ShootBullet(); // Fire a bullet
                yield return new WaitForSeconds(bulletDelay); // Wait before firing the next
        }
    }

    private void ShootBullet()
    {
        GameObject projectile = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player.transform, (Vector3)uiOffset);
    }

    public void PerformSummonMinions()
    {
        Debug.Log("Dragon summons minions!");
        animator.SetTrigger("SummonMinions");
    }

    public void PerformAttack01()
    {
        Debug.Log("PerformAttack01");
        animator.SetTrigger("Attack01");

    }

    public void PerformAttack02()
    {
        Debug.Log("PerformAttack02");
        animator.SetTrigger("Attack02");
    }

    public void PerformAttack03()
    {
        Debug.Log("PerformAttack03");
        animator.SetTrigger("Attack03");
    }

    public void PerformAttack04()
    {
        Debug.Log("PerformAttack04");
        animator.SetTrigger("Attack04");
    }

    public void PerformAttack05()
    {
        Debug.Log("PerformAttack05");
        animator.SetTrigger("Attack05");
    }

    public void PerformCast05()
    {
        Debug.Log("PerformCast05");
        animator.SetTrigger("Cast05");
    }

    public void LockMovement()
    {
        //Debug.Log("Movement locked.");
        if (movementController) movementController.LockMovement(); // Call LockMovement from BossRotationWithAnimation
    }

    public void UnlockMovement()
    {
        //Debug.Log("Movement unlocked.");
        if (movementController) movementController.UnlockMovement(); // Call UnlockMovement from BossRotationWithAnimation
    }

    private IEnumerator DashForward()
    {
        isDashing = true;

        Vector3 dashDirection = transform.forward;
        float reducedDashDistance = dashDistance;
        Vector3 potentialTargetPosition = transform.position + dashDirection * reducedDashDistance;

        // Project the target position onto the NavMesh
        if (
            NavMesh.SamplePosition(
                potentialTargetPosition,
                out NavMeshHit hit,
                reducedDashDistance,
                NavMesh.AllAreas
            )
        )
        {
            Vector3 targetPosition = hit.position; // Use the position on the NavMesh
            float dashTime = Vector3.Distance(transform.position, targetPosition) / dashSpeed; // Adjust dash time
            float startTime = Time.time;

            // While the enemy has not reached the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                float journeyProgress = (Time.time - startTime) / dashTime;
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    journeyProgress
                );
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Dash target position is not on the NavMesh. Cancelling dash.");
        }
        isDashing = false;
    }

    void EnableAttack(String Number)
    {
        if (!isDashing)
        {
            StartCoroutine(DashForward());
        }
        HideIndicator();
        int x = 0;
        Int32.TryParse(Number, out x);
        attackHitboxes[x].enabled = true;
    }

    void DisableAttack(String Number)
    {
        HideIndicator();

        int x = 0;
        Int32.TryParse(Number, out x);
            
        //Debug.Log("DisableAttack");
        attackHitboxes[x].enabled = false;
        //if (AttackTimeFrame != 0)
        //{
        //    ShowIndicator(AttackTimeFrame);
        //}
        
    }

    void ShowIndicator(int AttackTimeFrame)
    {
        if (attackIndicatorController != null)
        {
            //attackIndicatorCanvas.enabled = true;
            attackIndicatorController.ShowIndicator(AttackTimeFrame);
        }
    }

    void HideIndicator()
    {
        if (attackIndicatorController != null)
        {
            attackIndicatorController.HideIndicator();
        }
    }


    public void StartKnockBack(){
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 forceDirection = (player.position - transform.position).normalized; // Direction away from dragon
                float knockbackForce = 10f; // Adjust force value as needed
                forceDirection.y = 1f; // Add slight upward force for impact
                playerRb.AddForce(forceDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }

}
