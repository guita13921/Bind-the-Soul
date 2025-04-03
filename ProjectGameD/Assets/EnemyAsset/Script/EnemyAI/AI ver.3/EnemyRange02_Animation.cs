using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyRange02_Animation : MonoBehaviour
{
    private Animator animator;
    private EnemyRange02 enemy;
    private NavMeshAgent agent;
    private bool isAnimationLocked = false; // Prevent overriding animations
    private float defaultSpeed; // To store the agent's normal speed
    

    void Start()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyRange02>();
        agent = GetComponent<NavMeshAgent>();

        defaultSpeed = agent.speed;
    }

    void Update()
    {
        if(enemy.GetIsSpawning()) return;

        if (isAnimationLocked)
        {
            agent.speed = 0; // Stop movement while the animation is locked
            return;
        }
        else
        {
            agent.speed = defaultSpeed; // Restore normal speed when unlocked
        }

        // Update animations based on the enemy's state
        if (enemy.GetIsStunned())
        {
            PlayStunAnimation();
        }
        else if (enemy.GetIsOnAttackCooldown())
        {
            PlayHideAnimation();
        }
        else if (enemy.GetIsShooting()) // Shooting during attack
        {
            PlayAttackAnimation();
        }
        else
        {
            PlayPatrolAnimation();
        }
    }

    public void PlayPatrolAnimation()
    {
        if (isAnimationLocked) return;
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHiding", false);
        animator.SetBool("isStunned", false);
    }

    public void PlayAttackAnimation()
    {
        LockAnimation(); // Lock animation until attack finishes
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isHiding", false);
        animator.SetBool("isStunned", false);

        // Automatically unlock after attack animation ends
        StartCoroutine(UnlockAfterAnimation("Attack"));
    }

    public void PlayHideAnimation()
    {
        if (isAnimationLocked) return;
        //animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHiding", true);
        animator.SetBool("isStunned", false);
    }

    public void PlayStunAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHiding", false);
        animator.SetTrigger("isStunned");
    }

    public void PlayIdleAnimation()
    {
        animator.SetBool("isIdle", true);
    }

    public void PlayEndIdleAnimation()
    {
        animator.SetBool("isIdle", false);
    }

    public void PlayDeadAniamtion()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHiding", false);
        animator.SetBool("isStunned", false);
        animator.SetBool("isDeath", true);
        LockAnimation();
    }

    private void LockAnimation()
    {
        isAnimationLocked = true;
        agent.speed = 0; // Stop movement when the animation is locked

    }

    private void UnlockAnimation()
    {
        isAnimationLocked = false;
        agent.speed = defaultSpeed; // Restore movement when the animation is unlocked
    }

    private IEnumerator UnlockAfterAnimation(string animationName)
    {
        // Get the duration of the current animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(animationName))
        {
            // Wait until the specified animation starts playing
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(stateInfo.length);

        // Unlock animations
        UnlockAnimation();
    }
}
