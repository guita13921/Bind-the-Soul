using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private SteeringBasics steeringBasics;
    [SerializeField] private CollisionAvoidance collisionAvoidance;
    [SerializeField] private WallAvoidance wallAvoidance;
    [SerializeField] private List<MovementAIRigidbody> nearbyObstacles;
    [SerializeField] private EnemyAnimation enemyAnimation;

    private void Start()
    {
        enemyAnimation = GetComponent<EnemyAnimation>();
        steeringBasics = GetComponent<SteeringBasics>();
        collisionAvoidance = GetComponent<CollisionAvoidance>();
        wallAvoidance = GetComponent<WallAvoidance>();
        nearbyObstacles = new List<MovementAIRigidbody>();
    }

    public void CheckDistance(float stoppingDistance, float distanceToTarget)
    {
        if (stoppingDistance >= distanceToTarget)
        {
            lockEnemy();
        }
    }

    public void Chase()
    {
        if (target != null)
        {
            steeringBasics.maxVelocity = 2f;
            Vector3 accel = steeringBasics.Seek(target.position);
            accel += collisionAvoidance.GetSteering(nearbyObstacles);
            accel += wallAvoidance.GetSteering();
            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
            enemyAnimation.CalculateVelocity();
        }
    }

    public void GetLock()
    {
        lockEnemy();
    }

    private void OnTriggerEnter(Collider other)
    {
        MovementAIRigidbody obstacle = other.GetComponent<MovementAIRigidbody>();
        if (obstacle != null && !nearbyObstacles.Contains(obstacle))
        {
            nearbyObstacles.Add(obstacle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MovementAIRigidbody obstacle = other.GetComponent<MovementAIRigidbody>();
        if (obstacle != null && nearbyObstacles.Contains(obstacle))
        {
            nearbyObstacles.Remove(obstacle);
        }
    }


    private void lockEnemy()
    {
        steeringBasics.maxVelocity = 0f;
        steeringBasics.LookAtDirection(target.position);
        enemyAnimation.UpdateAnimator(0f);
        Vector3 accel = steeringBasics.Seek(target.position);
        accel += collisionAvoidance.GetSteering(nearbyObstacles);
        accel += wallAvoidance.GetSteering();
        steeringBasics.Steer(accel);
        steeringBasics.LookWhereYoureGoing();
        enemyAnimation.CalculateVelocity();
    }
}
