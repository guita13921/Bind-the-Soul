using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStatConfig : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private EnemyAI3 scriptToAccess; // Can assign Chaos_Warriro directly in the Inspector
    [SerializeField] private Slider Health_Bar_Erase;
    [SerializeField] private Slider Health_Bar;
    [SerializeField] private float IN_KnockBackTime;
    [SerializeField] private float IN_CoolDownAttack;
    [SerializeField] private int IN_numberOfRandomVariations; 
    [SerializeField] private int IN_Health; 
    [SerializeField] private int IN_Speed; 
    [SerializeField] private int IN_Damage;
    [SerializeField] private int IN_range;
    [SerializeField] private int IN_sightRange;
    [SerializeField] private int IN_attackRange;
    [SerializeField] private float IN_dashDistance;
    [SerializeField] private float IN_dashSpeed;
    [SerializeField] private int IN_stoprange;

    private void Start()
    {
        if (scriptToAccess != null){
            scriptToAccess.SetStat(IN_KnockBackTime, IN_CoolDownAttack, IN_numberOfRandomVariations,
             IN_Speed, IN_Damage, IN_range, IN_sightRange, IN_attackRange, IN_dashDistance, IN_dashSpeed,
             IN_stoprange, agent);
        }
        Health_Bar_Erase.maxValue = IN_Health;
        Health_Bar.maxValue = IN_Health;
    }
}
