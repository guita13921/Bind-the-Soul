using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeBomb : EnemyAI3
{
    public GameObject bombPrefab; // Prefab for the bomb
    [SerializeField] private Transform throwPoint; // Point from which bombs are thrown
    [SerializeField] private float throwAngle = 45f; // Angle for the throw
    [SerializeField] private int numberOfBombs = 1; // Number of bombs to throw
    [SerializeField] private float bombDelay = 0.5f; // Time delay between bombs

    protected override void Update()
    {

        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        if (isSpawning || state == State.Dead) return;

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

    private void StartBombThrowing()
    {
        StartCoroutine(ThrowBombsWithDelay(numberOfBombs, bombDelay));
    }

    private IEnumerator ThrowBombsWithDelay(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            ThrowBombAtPlayer();
            yield return new WaitForSeconds(delay);
        }
        EndBombThrowing();
    }

    private void ThrowBombAtPlayer()
    {

        if (player == null) return;

        animator.SetBool("Attack", true);
        Vector3 targetPosition = player.transform.position;
        GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);

        // Calculate velocity
        Vector3 velocity = CalculateThrowVelocity(targetPosition, throwPoint.position, throwAngle);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.velocity = velocity;
    }

    private Vector3 CalculateThrowVelocity(Vector3 target, Vector3 origin, float angle)
    {
        float gravity = Physics.gravity.y;
        float radianAngle = angle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarOrigin = new Vector3(origin.x, 0, origin.z);

        float distance = Vector3.Distance(planarTarget, planarOrigin);
        float yOffset = origin.y - target.y;

        float initialVelocity = Mathf.Sqrt(distance * -gravity / (Mathf.Sin(2 * radianAngle)));
        float verticalVelocity = initialVelocity * Mathf.Sin(radianAngle);
        float horizontalVelocity = initialVelocity * Mathf.Cos(radianAngle);

        Vector3 velocity = new Vector3(
            horizontalVelocity * (target.x - origin.x) / distance,
            verticalVelocity,
            horizontalVelocity * (target.z - origin.z) / distance
        );

        return velocity;
    }

    private void EndBombThrowing()
    {
        animator.SetBool("Attack", false);
        state = State.Cooldown;
        agent.speed = speed;
    }

    public void Hide() {
        animator.SetBool("Chase", true);
        agent.transform.LookAt(player.transform);
        agent.SetDestination(-player.transform.position);
    }
}
