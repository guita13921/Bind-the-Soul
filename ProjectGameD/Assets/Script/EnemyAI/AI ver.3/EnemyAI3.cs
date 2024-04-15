using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyAI3 : MonoBehaviour{
    [SerializeField]GameObject player;
    NavMeshAgent agent;
    [SerializeField] CapsuleCollider caps;
    [SerializeField]LayerMask groundLayer,playerLayer;
    //Walk Var
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;
    [SerializeField]float sightRange,attackRange;
    [SerializeField] bool playerInsight,PlayerInAttackrange;
    //Animatotion var
    Animator animator;
    [SerializeField] BoxCollider boxCollider;
    //State Var
    public enum State{Ready,Cooldown,KnockBack};
    [SerializeField] public State state;
    int speed = 4;
    //CoolDown var
    [SerializeField] float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;
    [SerializeField] float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;

    void Start(){
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
    }

    void Update(){
        AnimationCheckState();
        CooldownKnockBackTime();
        CoolDownAttaickTime();
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (!playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Patrol();
        if (playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Chase();
        if (playerInsight && PlayerInAttackrange && state == State.Ready)Attack();
    }

    void AnimationCheckState(){
        if(state == State.Cooldown){
            animator.SetBool("IsCooldown",true);
            DisableAttack();
        }else{
            animator.SetBool("IsCooldown",false);
        }

        if(state == State.KnockBack){
            DisableAttack();
            animator.SetBool("isKnockBack",true);
            animator.SetBool("Chase",false);
            animator.SetBool("Attack",false);
        }else{
            animator.SetBool("isKnockBack",false);
        }
    }


    void CoolDownAttaickTime(){
        if (!timerReachedCoolDownAttack && state == State.Cooldown) timerCoolDownAttack += Time.deltaTime;
        if (!timerReachedCoolDownAttack && timerCoolDownAttack > 2 && state == State.Cooldown){
            agent.speed = speed;
            state = State.Ready;
            timerCoolDownAttack = 0;
        }  

    }

    private void CooldownKnockBackTime(){
        if (!timerReachedCoolKnockBack && state == State.KnockBack)timerCoolKnockBack += Time.deltaTime;
        if (!timerReachedCoolKnockBack && timerCoolKnockBack > 2 && state == State.KnockBack){
            agent.speed = speed;
            state = State.Ready;
            timerCoolKnockBack = 0;
        }     
    }

    void KnockBack(){
        state = State.KnockBack;
        animator.SetTrigger("Knockback");
        agent.transform.LookAt(player.transform);
    }

    void Chase(){
        DisableAttack();
        animator.SetBool("Chase",true);
        agent.SetDestination(player.transform.position);
    }

    void Attack(){
        animator.SetBool("Chase",false);
        animator.SetBool("Attack",true);
        agent.transform.LookAt(player.transform);
    }

    void Dead(){
        animator.enabled = false;
        this.enabled = false;
        agent.enabled = false;
        boxCollider.enabled = false;
        this.tag = "Untagged";
    }

    void Patrol(){
        if (!walkpointSet)
            SearchForDest();
        if (walkpointSet)
            agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10)
            walkpointSet = false;
    }

    void SearchForDest(){
        float z = UnityEngine.Random.Range(-range, range);
        float x = UnityEngine.Random.Range(-range, range);

        destPoint = new Vector3(
            transform.position.x + x,
            transform.position.y,
            transform.position.z + z
        );

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer)){
            walkpointSet = true;
        }
    }

    void EnableAttack(){
        boxCollider.enabled = true;
    }

    void DisableAttack(){
        boxCollider.enabled = false;
    }

    void StartAttack(){
        agent.speed = 0;
    }

    void StopAttack(){
        animator.SetBool("Attack",false);
        state = State.Cooldown;
    }

    void StartKnockBack(){
        agent.speed = 0;
    }

    void OnTriggerEnter(Collider other){
        if(other.isTrigger && other.gameObject.CompareTag("PlayerSword")){
            KnockBack();
        }
    }
}