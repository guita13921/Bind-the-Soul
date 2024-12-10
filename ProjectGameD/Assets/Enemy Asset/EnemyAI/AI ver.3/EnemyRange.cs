using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : EnemyAI3{
    public float BulletSpeed;
    public GameObject enemyBullet;
    private float bulletTime;
    [SerializeField]private float bulletTimeEr;
    [SerializeField]public Transform SpawnPoint;



    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        CheckHealth();
        if(state != State.Dead){
            AnimationCheckState();
            CooldownKnockBackTime();
            CoolDownAttaickTime();
            playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Patrol();
            if (playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Chase();
            if (playerInsight && PlayerInAttackrange && state == State.Ready)Shoot();
            if (state == State.Cooldown)Hide();
        }
    }

    void Shoot(){
        animator.SetBool("Attack", true);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.transform.LookAt(player.transform);
        //state = State.Cooldown;

        /*
        int numberOfBullets = 3; // Number of bullets fired in one attack
        float spreadAngle = 15f; // Angle in degrees for the scatter
        float bulletDelay = 0.2f; // Delay between bullets
        StartCoroutine(ShootWithDelay(numberOfBullets, spreadAngle, bulletDelay));
        */
    }

    private IEnumerator ShootWithDelay(int numberOfBullets, float spreadAngle, float bulletDelay)
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            // Instantiate the bullet
            GameObject bullet = Instantiate(enemyBullet, SpawnPoint.transform.position, SpawnPoint.transform.rotation);

            // Calculate scatter direction
            Vector3 scatterDirection = SpawnPoint.forward;
            scatterDirection = Quaternion.Euler(
                UnityEngine.Random.Range(-spreadAngle, spreadAngle), // Yaw (left-right)
                UnityEngine.Random.Range(-spreadAngle, spreadAngle), // Pitch (up-down)
                0 // No roll
            ) * scatterDirection;

            // Add force to the bullet in the scatter direction
            Rigidbody bulletRig = bullet.GetComponent<Rigidbody>();
            bulletRig.AddForce(scatterDirection.normalized * BulletSpeed, ForceMode.Impulse);

            // Destroy bullet after 5 seconds
            Destroy(bullet, 5f);

            // Wait for the specified delay before firing the next bullet
            yield return new WaitForSeconds(bulletDelay);
        }

        // After shooting is done, re-enable movement
        agent.isStopped = false; // Allow the agent to move again
    }

    public void Hide() {
        animator.SetBool("Chase", true);
        agent.transform.LookAt(player.transform);
        agent.SetDestination(-player.transform.position);
    }


    void StartShoot(){
        int numberOfBullets = 3; // Number of bullets fired in one attack
        float spreadAngle = 15f; // Angle in degrees for the scatter
        float bulletDelay = 0.2f; // Delay between bullets

        StartCoroutine(ShootWithDelay(numberOfBullets, spreadAngle, bulletDelay));
    }

    void EndShoot(){
        animator.SetBool("Attack", false);
        state = State.Cooldown;
        agent.speed = speed;
    }

}
