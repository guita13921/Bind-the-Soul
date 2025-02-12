using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeBomb02 : EnemyRange02
{
    [SerializeField] private float throwAngle = 45f; // Angle for the throw
    [SerializeField] private int numberOfBombs = 1; // Number of bombs to throw
    [SerializeField] private float bombDelay = 0.5f; // Time delay between bombs

    protected override void Start() {
        base.Start();
    }
    protected virtual void Update(){
        base.Update();
    }

    private void StartShoot()
    {
        if (!isShooting)
        {
            StartCoroutine(ThrowBombsWithDelay(numberOfBombs, bombDelay));
        }
    }

    private IEnumerator ThrowBombsWithDelay(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            ThrowBombAtPlayer();
            yield return new WaitForSeconds(delay);
        }
    }

    private void ThrowBombAtPlayer()
    {

        if (player == null) return;

        enemyAnimation.PlayAttackAnimation();
        Vector3 targetPosition = player.transform.position;
        GameObject bomb = Instantiate(BulletPrefab, firePoint.position, Quaternion.identity);

        // Calculate velocity
        Vector3 velocity = CalculateThrowVelocity(targetPosition, firePoint.position, throwAngle);
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

}
