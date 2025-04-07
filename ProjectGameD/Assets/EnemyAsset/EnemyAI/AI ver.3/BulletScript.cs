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
    [SerializeField] private float elapsedTime;
    [SerializeField] private float runDuration;
    private bool isHomingActive = true;

    [Space]
    [Header("Self-Destruction Settings")]
    [SerializeField] private float maxLifetime;
    [SerializeField] private float maxRange;
    private Vector3 spawnPosition;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        FlashEffect();
        newRandom();
        spawnPosition = transform.position;
        Invoke("SetUp", maxLifetime); // Call SetUp instead of Destroy
    }

    void newRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
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
        if (Vector3.Distance(spawnPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
            return;
        }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            HitTarget();
        }
        else if (other.tag == "Obstacle")
        {
            Instantiate(hit, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            FlashEffect();
        }
    }

    void FlashEffect()
    {
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
