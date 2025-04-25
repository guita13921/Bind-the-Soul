using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

public class ResetAnimatorBoolAI : ResetAnimatorBool
{
    public string isPhaseShifting = "isPhaseShifting";
    public bool isisPhaseShiftingStatus = false;

    public string isAttacking = "isAttacking";
    public bool isisAttackingStatus = false;

    public string isFiringSpell = "isFiringSpell";
    public bool isFiringSpellStatus = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(isPhaseShifting, isisPhaseShiftingStatus);
        animator.SetBool(isAttacking, isisAttackingStatus);
        animator.SetBool(isFiringSpell, isFiringSpellStatus);
    }
}
