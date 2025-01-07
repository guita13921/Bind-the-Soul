using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class kamikazeEnemy : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] protected LayerMask groundLayer,playerLayer;

    [SerializeField] private Transform player;       // Reference to the player's transform
    [SerializeField] private bool hasCollided = false;  // To check if the enemy has already collided with the player
    [SerializeField] private NavMeshAgent agent;

    //Walk Var
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] private protected float range;
    [SerializeField] private protected float sightRange,attackRange;
    [SerializeField] private protected bool playerInsight;

    //Heath and Canvas
    [SerializeField] Canvas bar;
    [SerializeField] EnemyHealth health;
    [SerializeField] PlayerWeapon playerWeapon;

    //Effect
    [SerializeField] private GameObject explosionEffect; // Explosion VFX prefab

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player in the scene
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>(); 
    }

    public void SetStat(int IN_speed,int IN_range,int IN_sightRange){
        agent.speed = IN_speed;
        range = IN_range;
        sightRange = IN_sightRange;
    }

    protected void Update()
    {
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        if (!playerInsight)Patrol();
        if (playerInsight)Chase();
        if (health.GetCurrentHealth() <= 0f && !hasCollided)Die();
    }
    
    protected virtual void Patrol(){
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

    public void Chase() {
        agent.SetDestination(player.transform.position);
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Player") && health.GetCurrentHealth() > 0f){
            Explode();
        }

        if (other.gameObject.CompareTag("PlayerSword") && health.GetCurrentHealth() > 0f){
            Debug.Log("Dog got hit");
            health.CalculateDamage(10f);
            agent.transform.LookAt(player.transform);
        }
    }


    public void TakeDamage(float damage)
    {
        health.CalculateDamage(damage);
        Vector3 knockBackDirection = transform.position - player.transform.position;
        KnockBack(knockBackDirection, 10f);

        if (health.GetCurrentHealth() <= 0f){
            Die();
        }
    }

    public void KnockBack(Vector3 hitDirection, float knockBackForce){
        rb.AddForce(hitDirection.normalized * knockBackForce, ForceMode.Impulse);
    }

    void Explode()
    {
        if (hasCollided) return;
            hasCollided = true;
        Debug.Log("Boom! Enemy exploded!");
        Invoke(nameof(DestroySelf), 0.5f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f); 
    }


    void Die()
    {
        // If the enemy dies before reaching the player, no explosion occurs.
        Debug.Log("Enemy died without reaching player.");

        // You can add death animations or particle effects here
        Destroy(gameObject);  // Destroy the enemy object
    }

}
