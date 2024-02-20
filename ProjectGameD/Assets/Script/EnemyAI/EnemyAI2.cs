using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour{
    [SerializeField]GameObject player;
    NavMeshAgent agent;
    [SerializeField]LayerMask groundLayer,playerLayer;
    Vector3 destPoint;
    bool walkpointSet;
    bool isDead;
    [SerializeField] float range;
    [SerializeField]float sightRange,attackRange;
    [SerializeField] bool playerInsight,PlayerInAttackrange; //FIX to see 2 player
    Animator animator;
    [SerializeField] BoxCollider boxCollider;
    public enum State{Ready,Cooldown,KnockBack,Dead};
    [SerializeField] public State state;
    Health hp;
    float damage;
    [SerializeField] float timerCoolDownAttack = 0;
    bool timerReachedCoolDownAttack = false;
    [SerializeField] float timerCoolKnockBack = 0;
    bool timerReachedCoolKnockBack = false;


    void Start(){
        state = State.Ready;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        hp = GetComponent<Health>();
    }

    IEnumerator waiter(float x){
        yield return new WaitForSecondsRealtime(x);
    }

    void Update(){
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        PlayerInAttackrange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (!playerInsight && !PlayerInAttackrange && (state != State.KnockBack || state != State.Cooldown))Patrol();
        if (playerInsight && !PlayerInAttackrange && (state != State.KnockBack || state != State.Cooldown))Chase();
        if (playerInsight && PlayerInAttackrange && state == State.Ready)Attack();
        
        //Stop when player in attack range 
        if(PlayerInAttackrange || state == State.Cooldown){
            animator.SetBool("InRange", true);
            agent.speed = 0f;
        }else{
            animator.SetBool("InRange", false);
        }

        //KnockBack
        if(state == State.KnockBack){
            KnockBack();
        }

        //KnockBack Time
        if (!timerReachedCoolKnockBack && state == State.KnockBack)timerCoolKnockBack += Time.deltaTime;
        if (!timerReachedCoolKnockBack && timerCoolKnockBack > 3 && state == State.KnockBack){
            Debug.Log("Done KnockBack");
            state = State.Ready;
            animator.SetTrigger("Ready");
            timerCoolKnockBack = 0;
        }

        //Attack CoolDown
        if (!timerReachedCoolDownAttack && state == State.Cooldown)timerCoolDownAttack += Time.deltaTime;
        if (!timerReachedCoolDownAttack && timerCoolDownAttack > 2 && state == State.Cooldown){
            Debug.Log("Done AttackCoolDown");
            state = State.Ready;
            animator.SetTrigger("Ready");
            timerCoolDownAttack = 0;
        }
    }

    void Chase(){
        animator.SetTrigger("Chase");
        agent.speed = 4;
        agent.SetDestination(player.transform.position);
    }

    
    void Attack(){
        agent.SetDestination(transform.position);
        animator.SetInteger("AttackIndex", UnityEngine.Random.Range(0, 3));
        animator.SetTrigger("Attack");
        agent.transform.LookAt(player.transform);
        state = State.Cooldown;
    }

    void Dead(){
        state = State.Dead;
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

    void EnableAttack(){
        boxCollider.enabled = true;
    }

    void DisableAttack(){
        boxCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other){
        if(other.isTrigger && other.gameObject.CompareTag("PlayerSword")){
            state = State.KnockBack;
            print(other);
            hp.currentHealth -= damage;
        }
    }

    void KnockBack(){
        agent.isStopped = true;
        agent.transform.LookAt(player.transform);
        animator.SetTrigger("HIT!");
    }
    
}
