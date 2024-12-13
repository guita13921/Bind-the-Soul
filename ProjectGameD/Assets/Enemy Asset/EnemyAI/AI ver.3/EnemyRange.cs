using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRange : EnemyAI3{
    public GameObject enemyBullet;
    [SerializeField]public Transform SpawnPoint;
    public Vector2 uiOffset;
    [SerializeField] private int numberOfBullets = 3; // Number of bullets to fire
    [SerializeField] private float bulletDelay = 0.5f; // Time between bullets



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
    }

    IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
    {
        // Prevent starting multiple shooting coroutines
        state = State.Cooldown;
        for (int i = 0; i < numberOfBullets; i++)
        {
            ShootBullet(); // Fire a bullet
            yield return new WaitForSeconds(bulletDelay); // Wait before firing the next
        }
        EndShoot();
    }

    private void ShootBullet()
    {
        animator.SetBool("Attack", true);

        GameObject projectile = Instantiate(enemyBullet, SpawnPoint.position, SpawnPoint.rotation);
        projectile.GetComponent<BulletScript>().UpdateTarget(player.transform, (Vector3)uiOffset);
    }

    public void Hide() {
        animator.SetBool("Chase", true);
        agent.transform.LookAt(player.transform);
        agent.SetDestination(-player.transform.position);
    }

    void StartShoot(){
        StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
    }

    void EndShoot(){
        animator.SetBool("Attack", false);
        state = State.Cooldown;
        agent.speed = speed;
    }

}
