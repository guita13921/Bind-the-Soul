using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator animator;

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnim, 0.2f);
        }
    }
}
