using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Skull : MonoBehaviour{
    [SerializeField]GameObject player;
    [SerializeField]public NavMeshAgent agent;
    [SerializeField]LayerMask groundLayer,playerLayer;
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float range;
    [SerializeField]float sightRange,attackRange;
    [SerializeField]public bool playerInsight,PlayerInAttackrange; //FIX to see 2 player

    [SerializeField] Animator animator;

    [SerializeField] BoxCollider boxCollider;
    public enum State{Ready,Cooldown,KnockBack};
    [SerializeField] public State state;
    Health hp;
    float damage;
    int speed = 2;
    [SerializeField] float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;
    [SerializeField] float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField]Canvas bar;


    void Start(){
        bar = GetComponentInChildren<Canvas>();
        //audioSource = GetComponent<AudioSource>();
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        hp = GetComponent<Health>();
    }

    void Update(){
        CheckState();
        KnockBackTime();
        CoolDownTime();
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        CheckPlayerInAttackrange();
        if (!playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Patrol();
        if (playerInsight && !PlayerInAttackrange && state != State.KnockBack && state != State.Cooldown)Chase();
        if (playerInsight && PlayerInAttackrange && state == State.Ready)Attack();
    }

    void CheckPlayerInAttackrange(){
        if(PlayerInAttackrange){
            animator.SetBool("InAttackRange", true);
        }else{
            animator.SetBool("InAttackRange",false);
            agent.speed = speed;
        }
    }

    void CheckState(){
        if(state == State.KnockBack){
            KnockBack();
            agent.speed = 0;
        }
        
        if(hp.currentHealth <= 0){
            Dead();
        }
    }


    void CoolDownTime(){
        if (!timerReachedCoolDownAttack && state == State.Cooldown) timerCoolDownAttack += Time.deltaTime;
        if (!timerReachedCoolDownAttack && timerCoolDownAttack > 5 && state == State.Cooldown){
            //Debug.Log("Done CoolDown");
            state = State.Ready;
            timerCoolDownAttack = 0;
            timerCoolKnockBack = 0;
        }  

    }

    void KnockBackTime(){
        if (!timerReachedCoolKnockBack && state == State.KnockBack)timerCoolKnockBack += Time.deltaTime;
        if (!timerReachedCoolKnockBack && timerCoolKnockBack > 5 && state == State.KnockBack){
            //Debug.Log("Done KnockBack");
            state = State.Ready;
            timerCoolDownAttack = 0;
            timerCoolKnockBack = 0;
        }     
    }

    void KnockBack(){
        agent.transform.LookAt(player.transform);
    }

    void Chase(){
        agent.speed = speed;
        agent.SetDestination(player.transform.position);
    }

    void Attack(){
        agent.SetDestination(transform.position);
        agent.transform.LookAt(player.transform);
        agent.speed = 0;
        animator.SetTrigger("Attack");
        state = State.Cooldown;
    }

    void Dead(){
        Destroy(gameObject);
        Destroy(bar.gameObject);
        animator.enabled = false;
        this.enabled = false;
        agent.enabled = false;
        hp.enabled = false;
        boxCollider.enabled = false;
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

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.isTrigger && other.gameObject.CompareTag("PlayerSword")){
            state = State.KnockBack;
            animator.SetTrigger("GotHit");
            hp.currentHealth -= damage;
        }
    }

}