using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class CharacterCombatManager : MonoBehaviour
    {
        [Header("AttackType")]
        public AttackType currentAttackType;

        public virtual void DrainStaminaBasedAttack()
        {
            //IF Enemy have stamina place here
        }
    }
}