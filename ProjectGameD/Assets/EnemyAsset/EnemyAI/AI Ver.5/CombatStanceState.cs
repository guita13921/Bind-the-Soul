using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class CombatStanceState : State
    {
        public override State Tick(EnemyManager enemyManager, EnemyStat enemyStat, EnemyAnimatorManager enemyAnimator)
        {
            //Check for attack range
            //potentially circle player or walk around them
            //if in attack range return Attack State
            //if we are in a cool down after attack, return this state and continue circling player
            //if the player runs out of range return the pursuetarget state
            return this;
        }
    }

}