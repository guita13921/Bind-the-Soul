using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatConfig : MonoBehaviour
{
    [SerializeField] private EnemyAI3 scriptToAccess; // Reference to your original script
    [SerializeField] float IN_KnockBackTime;
    [SerializeField] float IN_CoolDownAttack;

    private void Start()
    {
        scriptToAccess.SetStat(IN_KnockBackTime, IN_CoolDownAttack);
    }
}