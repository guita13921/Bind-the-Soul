using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour
{
    public string isInvulnerable = "IsInvulnerable";
    public bool isInvulnerableStatus = false;

    public string isInterActingBool = "isInteracting";
    public bool isInterActingStatue = false;

    public string isRotatingWithRootMotion = "isRotatingWithRootMotion";
    public bool isRotatingWithRootMotionStatus = false;

    public string canRotateBool = "canRotate";
    public bool canRotateStatue = true;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isInterActingBool, isInterActingStatue);
        animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
        animator.SetBool(canRotateBool, canRotateStatue);
    }
}
