using UnityEngine;


public class Chaos_warriors : EnemyAI3
{
    public bool isBerserk = false; // Whether the enemy is in berserk mode
    public bool hasDiedOnce = false; // Whether the enemy has already died once

    // Stat Config
    [SerializeField] private float berserkSpeedMultiplier;
    [SerializeField] private int berserkDamageBoost;
    [SerializeField] private float berserkAttackCooldownMultiplier;

    // Animation Config
    [SerializeField] private float animationSpeedMultiplier;

    protected override void Start()
    {
        base.Start(); // Call the base Start() method
        SetAnimationSpeed(1f); // Initialize the animation speed
    }

    protected override void Update()
    {
        if (isSpawning || state == State.Dead) return;
        base.Update(); // Retain the base functionality
    }


    protected override void CheckHealth()
    {
        if (health.GetCurrentHealth() <= 0 && !hasDiedOnce)
        {
            EnterBerserkMode(); // Trigger berserk mode instead of dying the first time
        }
        else if (health.GetCurrentHealth() <= 0 && hasDiedOnce)
        {
            base.CheckHealth(); // Proceed with normal death logic on the second death
        }
    }


    private void SetAnimationSpeed(float speedMultiplier)
    {
        if (animator != null)
        {
            animator.speed = speedMultiplier;
        }
    }

    private void EnterBerserkMode()
    {
        state = State.Cooldown;
        if (isBerserk) return; // If already in berserk mode, do nothing

        // Set berserk state
        isBerserk = true;
        hasDiedOnce = true;

        // Fully restore health
        health.SetState(health.GetMaxHealth());

        // Boost stats in berserk mode
        speed *= berserkSpeedMultiplier;
        originalSpeed *= berserkSpeedMultiplier;
        //agent.speed = speed;
        weapon.damage += berserkDamageBoost;
        CoolDownAttack *= berserkAttackCooldownMultiplier; // Reduce attack cooldown for faster attacks

        // Play berserk animation
        animator.SetTrigger("Berserk");
        animator.SetBool("IsBerserk", true);

        // Speed up animations
        SetAnimationSpeed(animationSpeedMultiplier);

        // Optional: Add visual or audio effects for entering berserk mode
        Debug.Log($"{gameObject.name} has entered Berserk Mode!");
    }

    /// <summary>
    /// Play the death sequence after the second death.
    /// </summary>
    public override void Dead()
    {
        if (!hasDiedOnce)
        {
            EnterBerserkMode();
            return;
        }

        base.Dead();
    }


}
