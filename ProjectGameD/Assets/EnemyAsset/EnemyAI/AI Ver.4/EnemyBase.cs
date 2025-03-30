using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

// Base Enemy Class
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] public float shieldHealth;
    [SerializeField] public bool hasShield;
    [SerializeField] public Transform target;
    [SerializeField] public GameObject meleeWeapon;

    [SerializeField] protected EnemyAnimation enemyAnimation;
    [SerializeField] protected EnemyVFX enemyVFX;
    [SerializeField] protected SteeringBasics steeringBasics;  // Basic movement steering
    [SerializeField] protected CollisionAvoidance collisionAvoidance; // AI avoids obstacles
    [SerializeField] protected WallAvoidance wallAvoidance; // AI avoids walls
    [SerializeField] protected List<MovementAIRigidbody> nearbyObstacles; // List of detected obstacles
    [SerializeField] protected MeleeSensor meleeSensor;

    protected virtual void Start()
    {
        // Assigning necessary components
        enemyAnimation = GetComponent<EnemyAnimation>();
        enemyVFX = GetComponent<EnemyVFX>();
        steeringBasics = GetComponent<SteeringBasics>();
        collisionAvoidance = GetComponent<CollisionAvoidance>();
        wallAvoidance = GetComponent<WallAvoidance>();
        nearbyObstacles = new List<MovementAIRigidbody>();
        meleeSensor = GetComponent<MeleeSensor>();
    }

    // Abstract methods to be implemented by subclasses
    public abstract void Attack();
    public abstract void Chase();
    public abstract void Defend();
    public abstract void TakeDamage(float amount);
}