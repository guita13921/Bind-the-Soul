using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPartol : MonoBehaviour
{
    [SerializeField] GameObject player;
    NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer, playerLayer;
    Vector3 destPoint;
    [SerializeField]bool walkpointSet;
    [SerializeField]float range;
    [SerializeField]float sightRange , attackRange;
    bool playerInsight, PlayerInAttackrange;
    Animator animator;
    BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update(){
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(!playerInsight && !PlayerInAttackrange) Patrol();
        if(playerInsight && !PlayerInAttackrange) Chase();
        if(playerInsight && PlayerInAttackrange)Attack();
    }

    void Chase(){
        agent.SetDestination(player.transform.position);
    }

    void Attack(){
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1h1")){
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }
    }

    void Patrol(){
        if(!walkpointSet) SearchForDest();
        if(walkpointSet) agent.SetDestination(destPoint);
        if(Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }

    void SearchForDest(){
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if(Physics.Raycast(destPoint,Vector3.down, groundLayer)){
            walkpointSet = true;
        }

    }

    void EnableAttack(){
        boxCollider.enabled = true;
    }

    void DisableAttack(){
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other){
        var player = other.GetComponent<CapsuleCollider>(); //WTF
        if(player != null){
            Debug.Log("HIT");
        }
    }

}
