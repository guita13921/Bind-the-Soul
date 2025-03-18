using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyBase : MonoBehaviour
{
    public float health;
    public float shieldHealth;
    public bool hasShield;
    public Transform target;
    public GameObject meleeWeapon;

    protected EnemyAnimation enemyAnimation;
    protected EnemyVFX enemyVFX;

    protected virtual void Start()
    {
        enemyAnimation = GetComponent<EnemyAnimation>();
        enemyVFX = GetComponent<EnemyVFX>();
    }

    public abstract void Attack();
    public abstract void Chase();
    public abstract void Defend();
    public abstract void TakeDamage(float amount);
}