using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour
{
    public float speed;
    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;
    public bool LocalRotation = false;
    public int health = 3; // Health of the bullet when grounded

    [SerializeField] private Transform target;
    private Vector3 targetOffset;
    private float startDistanceToTarget;
    private bool hasBounced = false;
    private bool isExploding = false;

    [Header("PROJECTILE PATH")]
    private float randomUpAngle;
    private float randomSideAngle;
    public float sideAngle = 25;
    public float upAngle = 20;

    [SerializeField] private float elapsedTime;
    [SerializeField] private float runDuration;
    private bool isHomingActive = true;

    [Header("PROJECTILE PATH")]
    public GameObject BulletPrefab;
    private Vector3 spawnPosition;

    void Start()
    {
        FlashEffect();
        newRandom();
        spawnPosition = transform.position;
    }

    void newRandom()
    {
        randomUpAngle = Random.Range(0, upAngle);
        randomSideAngle = Random.Range(-sideAngle, sideAngle);
    }

    public void UpdateTarget(Transform targetPosition, Vector3 Offset)
    {
        target = targetPosition;
        targetOffset = Offset;
        startDistanceToTarget = Vector3.Distance((target.position + targetOffset), transform.position);
    }

    void Update()
    {
        if (hasBounced || isExploding)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime > runDuration)
            isHomingActive = false;

        if (isHomingActive && target != null)
        {
            Vector3 direction = ((target.position + targetOffset) - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isExploding) return;

        if (other.CompareTag("Player"))
        {
            Explode();
        }
        else if (!hasBounced)
        {
            BounceToGround();
        }
    }

    void BounceToGround()
    {
        hasBounced = true;
        speed = 0;
        GameObject bomb = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        bomb.transform.position =  this.transform.position;
        
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && hasBounced)
        {
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        isExploding = true;
        if (hit != null)
        {
            var hitInstance = Instantiate(hit, transform.position, Quaternion.identity);
            Destroy(hitInstance, 2f);
        }
        Destroy(gameObject);
    }

    void FlashEffect()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            Destroy(flashInstance, 1f);
        }
    }
}
