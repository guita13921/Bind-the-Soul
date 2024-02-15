using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPartol : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    NavMeshAgent agent;

    [SerializeField]
    LayerMask groundLayer,
        playerLayer;
    Vector3 destPoint;
    bool walkpointSet;

    [SerializeField]
    float range;

    [SerializeField]
    float sightRange,
        attackRange;
    bool playerInsight,
        PlayerInAttackrange; //FIX to see 2 player
    public double current_delaytimeAttack;

    [SerializeField]
    public double delaytimeAttack;
    public double current_delaytimeGetHit;

    [SerializeField]
    public double delaytimeGetHit;
    Animator animator;
    BoxCollider boxCollider;
    public bool isDead;
    public bool isHit;
    public double delaytimeDead;
    float damage;
    Health hp;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        delaytimeAttack = 0.5;
        delaytimeDead = 0.5;
        delaytimeGetHit = 0.75;
        damage = 10; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        hp = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit == true && current_delaytimeGetHit > 0)
        {
            current_delaytimeGetHit -= Time.deltaTime;
        }
        else
        {
            current_delaytimeGetHit = delaytimeGetHit;
            isHit = false;
        }

        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (isDead == true)
            Dead();
        if (!playerInsight && !PlayerInAttackrange && !isDead && !isHit)
            Patrol();
        if (playerInsight && !PlayerInAttackrange && !isDead && !isHit)
            Chase();
        if (playerInsight && PlayerInAttackrange && !isDead && !isHit)
            Attack();
    }

    void Chase()
    {
        animator.SetTrigger("Chase");
        agent.speed = 4;
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if (current_delaytimeAttack > 0)
        {
            current_delaytimeAttack -= Time.deltaTime;
        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetInteger("AttackIndex", UnityEngine.Random.Range(0, 3));
            animator.SetTrigger("Attack");
            agent.transform.LookAt(player.transform);
            current_delaytimeAttack = delaytimeAttack;
        }
    }

    void Dead()
    {
        animator.SetTrigger("Dead");

        if (delaytimeDead > 0)
        {
            delaytimeDead -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Patrol()
    {
        if (!walkpointSet)
            SearchForDest();
        if (walkpointSet)
            agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10)
            walkpointSet = false;
    }

    void SearchForDest()
    {
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(
            transform.position.x + x,
            transform.position.y,
            transform.position.z + z
        );

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void EnableAttack()
    {
        boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        boxCollider.enabled = false;
    }

    void OnTriggerEnter()
    {
        hp.currentHealth -= damage;
        animator.SetTrigger("HIT!");
        isHit = true;
    }
}
