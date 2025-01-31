using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAnimations : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private BossRotationWithAnimation movementController;

    [Header("FireBreath")]
    [SerializeField] private FlamethrowerHitbox flamethrowerHitbox; // Reference to the flamethr
    [SerializeField] private ParticleSystem fireBreathEffect;
    
    [Header("Bullet")]
    [SerializeField] public GameObject enemyBullet;
    [SerializeField] public Transform SpawnPoint; //Headposition
    [SerializeField] private int numberOfBullets = 5; // Number of bullets to fire
    [SerializeField] private float bulletDelay = 0f; // Time between bullets
    public Vector2 uiOffset;

    public void PerformFireBreath()
    {
        Debug.Log("Dragon uses Fire Breath!");
        animator.SetTrigger("FireBreath");
        if (fireBreathEffect != null)
        {
            fireBreathEffect.Play();
            StartCoroutine(StopFireBreathEffect()); // Stop VFX after a delay
        }
    }

    private IEnumerator StopFireBreathEffect()
    {
        yield return new WaitForSeconds(6f); // Adjust based on the duration of the VFX
        if (fireBreathEffect != null)
        {
            fireBreathEffect.Stop();
        }

    }

    public void IncreaseFireBreath()
    {
        if (fireBreathEffect != null)
        {
            var main = fireBreathEffect.main;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constantMin * 4f, main.startSize.constantMax * 4f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constantMin * 4f, main.startSpeed.constantMax * 4f);
            flamethrowerHitbox.ActivateHitbox();
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    public void DecreaseFireBreath()
    {
        if (fireBreathEffect != null)
        {
            var main = fireBreathEffect.main;
            main.startSize = new ParticleSystem.MinMaxCurve(main.startSize.constantMin * 0.25f, main.startSize.constantMax * 0.25f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constantMin * 0.25f, main.startSpeed.constantMax * 0.25f);
            flamethrowerHitbox.DeactivateHitbox();
        }
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

    void StartShoot(){
        StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
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
