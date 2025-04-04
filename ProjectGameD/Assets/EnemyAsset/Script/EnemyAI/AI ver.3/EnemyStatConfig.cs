using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStatConfig : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private EnemyAI3 scriptToAccessEnemyAI3; // Can assign Chaos_Warriro directly in the Inspector
    [SerializeField] private kamikazeEnemy scriptToAccesskamikazeEnemy;
    [SerializeField] private EnemyRange02 scriptToEnemyRange02;

    [Header("Agent")]
    [SerializeField] protected NavMeshAgent agent;

    [Header("Health")]
    [SerializeField] private EnemyHealth Health;
    [SerializeField] private Slider Health_Bar_Erase;
    [SerializeField] private Slider Health_Bar;

    [Header("EnemyAI3")]
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

    [Header("RangeOnly")]
    [SerializeField] private int IN_hideDistance;
    [SerializeField] private int IN_hideSearchRadius;

    private void Start()
    {
        scriptToAccessEnemyAI3 = GetComponent<EnemyAI3>();
        scriptToAccesskamikazeEnemy = GetComponent<kamikazeEnemy>();
        scriptToEnemyRange02 = GetComponent<EnemyRange02>();

        if (scriptToAccessEnemyAI3 != null)
        {
            scriptToAccessEnemyAI3.SetStat(
                IN_KnockBackTime,
                IN_CoolDownAttack,
                IN_numberOfRandomVariations,
                IN_Speed,
                IN_Damage,
                IN_range,
                IN_sightRange,
                IN_attackRange,
                IN_dashDistance,
                IN_dashSpeed,
                IN_stoprange,
                agent
            );

            Health.SetState(IN_Health);
            scriptToAccessEnemyAI3.setoriginalspeed();
        }
        else if (scriptToAccesskamikazeEnemy != null)
        {
            scriptToAccesskamikazeEnemy.SetStat(IN_Speed, IN_range, IN_sightRange);
            Health.SetState(IN_Health);
            scriptToAccessEnemyAI3.setoriginalspeed();

        }else if(scriptToEnemyRange02 != null){
            Health.SetState(IN_Health);

            scriptToEnemyRange02.SetStat(
                IN_KnockBackTime,
                IN_CoolDownAttack,
                IN_Speed,
                IN_Damage,
                IN_range,
                IN_sightRange,
                IN_attackRange,
                IN_dashDistance,
                IN_dashSpeed,
                IN_stoprange,
                agent,
                IN_hideDistance,
                IN_hideSearchRadius
            );
        }

    }
}
