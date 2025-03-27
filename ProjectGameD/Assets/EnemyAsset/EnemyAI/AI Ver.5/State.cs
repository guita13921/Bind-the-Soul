using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimatorManager);
}
