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
    [SerializeField] public MovementAIRigidbody target;
    [SerializeField] public GameObject meleeWeapon;

    [SerializeField] protected EnemyAnimation enemyAnimation;
    [SerializeField] protected EnemyVFX enemyVFX;
    [SerializeField] protected SteeringBasics steeringBasics;
    [SerializeField] protected CollisionAvoidance collisionAvoidance;
    [SerializeField] protected WallAvoidance wallAvoidance;
    [SerializeField] protected List<MovementAIRigidbody> nearbyObstacles;
    [SerializeField] protected NearSensor colAvoidSensor;

    protected virtual void Start()
    {
        enemyAnimation = GetComponent<EnemyAnimation>();
        enemyVFX = GetComponent<EnemyVFX>();
        steeringBasics = GetComponent<SteeringBasics>();
        //pursue = GetComponent<Pursue>();
        collisionAvoidance = GetComponent<CollisionAvoidance>();
        wallAvoidance = GetComponent<WallAvoidance>();
        nearbyObstacles = new List<MovementAIRigidbody>();
        colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
    }

    public abstract void Attack();
    public abstract void Chase();
    public abstract void Defend();
    public abstract void TakeDamage(float amount);
}