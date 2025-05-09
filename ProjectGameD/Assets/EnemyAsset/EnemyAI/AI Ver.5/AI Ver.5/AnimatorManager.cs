using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator animator;

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false)
        {
            // Make sure to not re-trigger if already playing
            if (animator == null) return;

            animator.applyRootMotion = isInteracting;
            animator.SetBool("canRotate", canRotate);
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFadeInFixedTime(targetAnim, 0f);
        }

        public void PlayTargetAnimationWithRootRotation(string targetAnim, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isRotatingWithRootMotion", isInteracting);
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnim, 0f);
        }

        public void PlayRecoilAnimation(string name)
        {
            animator.applyRootMotion = true;
            animator.SetBool("isInteracting", true);
            animator.SetBool("isAttacking", false); // Stop attack state
            animator.SetBool("canDoCombo", false);  // Optional: if using combos
            animator.CrossFade(name, 0f);   // Faster blend for snappy reaction
        }


    }
}
