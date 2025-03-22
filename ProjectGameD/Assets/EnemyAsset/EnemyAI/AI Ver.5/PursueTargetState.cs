using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PursueTargetState : State
    {
        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimator)
        {
            //Chase the target
            //If within attack range, switch to combat stance stage
            //If target is out of range, return this state and continue to chase target
            return this;
        }
    }
}
