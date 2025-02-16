using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BossDemon_Animation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private BossRotationWithAnimation movementController;
    [SerializeField] private DemonKnightBoss dragonBoss;
    public float rotationSpeed = 0.5f; // Adjust rotation speed as needed

    [Header("Malee")]
    [SerializeField] private SphereCollider AttackHitBox01; // Reference to the flamethr
    [SerializeField] private Weapon_Enemy AttackHitBox01_Script;
    private bool isClawSlength = false;
    private bool isRushForward = false;
    
    [Header("Bullet")]
    [SerializeField] public GameObject enemyBullet;
    [SerializeField] public Transform SpawnPoint; //Headposition
    [SerializeField] private int numberOfBullets = 5; // Number of bullets to fire
    [SerializeField] private float bulletDelay = 0f; // Time between bullets
    public Vector2 uiOffset;

    private void Update()
    {
        /*
        if ((isFiringLaser || isClawSlength || isRushForward) && player.transform.position != null)
        {
            if(isFiringLaser && meleeSensor.IsPlayerInRange()){
                return;
            }else{
                LookAtPlayer();
            }
        }else{
            return;
        }
        */
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
        GameObject projectile = Instantiate(enemyBullet, SpawnPoint.position, SpawnPoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player.transform, (Vector3)uiOffset);
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep rotation level (prevent looking up/down)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void PerformClawSwipe()
    {
        Debug.Log("Dragon performs Claw Swipe!");
        if(Random.value > 0.5f){
            animator.SetTrigger("ClawSwipe1");
            isClawSlength = true;
        }else{
            animator.SetTrigger("ClawSwipe2");
            isClawSlength = true;
        }
    }

    public void PerformSummonMinions()
    {
        Debug.Log("Dragon summons minions!");
        animator.SetTrigger("SummonMinions");
    }

    public void PerformAttack()
    {
        Debug.Log("Dragon summons minions!");
        animator.SetTrigger("Attack");

    }

    public void PerformSpecialAttack()
    {
        Debug.Log("Dragon summons minions!");
        animator.SetTrigger("SpecialAttack");

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
