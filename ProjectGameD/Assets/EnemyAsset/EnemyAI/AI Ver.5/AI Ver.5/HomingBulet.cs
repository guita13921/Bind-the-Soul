using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;


public class HomingBulet : MonoBehaviour
{

    [Header("PROJECTILE Homing")]
    public float speed;
    [SerializeField] private float elapsedTime;
    [SerializeField] private float runDuration;
    [SerializeField] private PlayerStats playerStats;
    private Transform target;
    private float randomUpAngle;
    private float randomSideAngle;
    public float sideAngle = 25;
    public float upAngle = 20;
    private bool isHomingActive = true;
    private Vector3 targetOffset;
    private float startDistanceToTarget;

    void Awake()
    {
        target = playerStats.transform;
    }

    public void UpdateTarget(GameObject player, Vector3 Offset)
    {
        target = player.transform;
        targetOffset = Offset;
        startDistanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > runDuration)
        {
            isHomingActive = false;
        }

        if (isHomingActive && target != null)
        {
            float distanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
            float angleRange = (distanceToTarget - 10) / 60;
            if (angleRange < 0) angleRange = 0;

            float saturatedDistanceToTarget = (distanceToTarget / startDistanceToTarget);
            if (saturatedDistanceToTarget < 0.5f)
                saturatedDistanceToTarget -= (0.5f - saturatedDistanceToTarget);
            saturatedDistanceToTarget -= angleRange;
            if (saturatedDistanceToTarget <= 0)
                saturatedDistanceToTarget = 0;

            Vector3 forward = ((target.position + targetOffset) - transform.position);
            Vector3 crossDirection = Vector3.Cross(forward, Vector3.up);
            Quaternion randomDeltaRotation = Quaternion.Euler(0, randomSideAngle * saturatedDistanceToTarget, 0) * Quaternion.AngleAxis(randomUpAngle * saturatedDistanceToTarget, crossDirection);
            Vector3 direction = randomDeltaRotation * forward;

            float distanceThisFrame = Time.deltaTime * speed;

            Vector3 newPosition = transform.position + direction.normalized * distanceThisFrame;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            Vector3 straightDirection = transform.forward * speed * Time.deltaTime;
            Vector3 newPosition = transform.position + straightDirection;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}
