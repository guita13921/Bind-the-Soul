using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;
    public bool LocalRotation = false;
    [SerializeField] private Transform target;
    private Vector3 targetOffset;
    private float startDistanceToTarget;

    [Space]
    [Header("PROJECTILE PATH")]
    private float randomUpAngle;
    private float randomSideAngle;
    public float sideAngle = 25;
    public float upAngle = 20;

    // Homing Time
    [SerializeField] private float elapsedTime; // Timer to track elapsed time
    [SerializeField] private float runDuration; // Duration to run the Update logic
    private bool isHomingActive = true; // Flag to control homing behavior

    [Space]
    [Header("Self-Destruction Settings")]
    [SerializeField] private float maxLifetime = 5f; // Destroy bullet after this time
    [SerializeField] private float maxRange = 50f; // Maximum distance the bullet can travel before destruction
    private Vector3 spawnPosition; // To calculate traveled distance

    void Start()
    {
        FlashEffect();
        newRandom();
        spawnPosition = transform.position; // Record the initial spawn position
        Destroy(gameObject, maxLifetime); // Automatically destroy after maxLifetime
    }

    void newRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
    }

    // Link from another script
    // TARGET POSITION + TARGET OFFSET
    public void UpdateTarget(Transform targetPosition, Vector3 Offset)
    {
        target = targetPosition;
        targetOffset = Offset;
        startDistanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; // Increment the timer

        // Destroy the bullet if it exceeds the maximum range
        if (Vector3.Distance(spawnPosition, transform.position) > maxRange)
        {
            Destroy(gameObject); // Destroy bullet if it travels too far
            return;
        }

        // Disable homing logic if elapsed time exceeds the run duration
        if (elapsedTime > runDuration)
        {
            isHomingActive = false;
        }

        // Homing logic only runs when isHomingActive is true
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

            // Translate the bullet while locking the Y position
            Vector3 newPosition = transform.position + direction.normalized * distanceThisFrame;
            newPosition.y = transform.position.y; // Lock Y position
            transform.position = newPosition;

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            // Move forward in a straight line after homing ends
            Vector3 straightDirection = transform.forward * speed * Time.deltaTime;
            Vector3 newPosition = transform.position + straightDirection;

            // Freeze the y-axis by maintaining the current y position
            newPosition.y = transform.position.y;

            transform.position = newPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            HitTarget();
        }
        else
        {
            FlashEffect();
        }
    }

    void FlashEffect()
    {
        // Debug.Log("FLASH");
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
    }

    void HitTarget()
    {
        // Debug.Log("HIT");
        if (hit != null)
        {
            var hitRotation = transform.rotation;
            if (LocalRotation == true)
            {
                hitRotation = Quaternion.Euler(0, 0, 0);
            }
            var hitInstance = Instantiate(hit, target.transform.position + targetOffset, hitRotation);
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 1);
            }
        }
        Destroy(gameObject);
    }
}
