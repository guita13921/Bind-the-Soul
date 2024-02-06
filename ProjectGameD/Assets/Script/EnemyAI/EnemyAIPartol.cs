using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPartol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer, playerLayer;
    Vector3 destPoint;
    [SerializeField]bool walkpointSet;
    [SerializeField]float range;
    [SerializeField]float sightRange , attackRange;
    bool playerInsight, PlayerInAttackrange;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update(){
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if(!playerInsight && !PlayerInAttackrange) Patrol();
        if(playerInsight && !PlayerInAttackrange) Chase();
        if(playerInsight && PlayerInAttackrange)Attack();
    }

    void Chase(){
        agent.SetDestination(player.transform.position);
    }

    void Attack(){

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

}
