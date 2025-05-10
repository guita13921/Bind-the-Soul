using System.Collections;
using UnityEngine;

public partial class PlayerControl
{
    [SerializeField] private bool isAttack = false;
    [SerializeField] private bool isDashing = false;

    private float movewhenATK = 0.185f;

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isAttack = true;
        }

        bool animtorAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        float currentAnimtortime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (
            animtorAttack && currentAnimtortime >= 0.7
            || animator.GetCurrentAnimatorStateInfo(0).IsName("SPAttack")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit")
        )
        {
            isAttack = false;
        }
        else if (animtorAttack && currentAnimtortime < 0.7)
        {
            isAttack = true;
        }
        MoveWhenATK();

        if (animtorAttack && currentAnimtortime > 0.7)
        {
            isAttack = false;
        }
    }

    [SerializeField] private float dashDistance = 2f; // Total distance to dash
    [SerializeField] private float dashTime = 0.5f;

    public GameObject prefabToInstantiate; // Assign your prefab in the inspector
    private Vector3 spawnPosition; // Set this to the desired spawn position

    public DashCheck dashCheck; // Assign in inspector
    public ControlPower controlPower;
    public float dashWaitTime = 0.75f;

    IEnumerator Dash()
    {
        if (!canDash)
        {
            yield break; // Exit if the cooldown is active
        }

        isAttack = false;
        controlPower.DashVFX();

        canDash = false; // Prevent dashing again immediately
        isDashing = true;
        Vector3 dashDirection = transform.forward.normalized;

        float distanceTraveled = 0f;

        animator.Play("Dash");
        spawnPosition = transform.position + dashDirection * dashDistance;

        bool CheckForCollision = false;
        dashCheck.SetCollisionState(false);

        yield return null;

        if (!dashCheck.willCollide)
        {

            Physics.IgnoreLayerCollision(7, 9, true);
        }

        while (distanceTraveled < dashDistance)
        {
            CheckForCollision = CheckForCollisions();
            float dashStep = (dashDistance / dashTime) * Time.deltaTime;
            Vector3 dashMovement = dashDirection * dashStep;

            if (CheckForCollisions())
            {
                break;
            }
            if (!CheckForCollision || !dashCheck.willCollide)
            {
                transform.position += dashMovement;
                distanceTraveled += dashStep;
            }
            yield return null;
        }
        Physics.IgnoreLayerCollision(7, 9, false);

        animator.Play("Idle");
        isDashing = false;

        yield return new WaitForSeconds(dashWaitTime);
        canDash = true; // Reset the cooldown
    }

    private bool CheckForCollisions()
    {
        float checkRadius = 1.5f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToCollider = (hitCollider.transform.position - transform.position).normalized;

            if (Vector3.Dot(transform.forward, directionToCollider) > 0)
            {
                if (hitCollider.CompareTag("CantDash") || hitCollider.CompareTag("Void"))
                {
                    return true; // Collision detected in the front area
                }
            }
        }

        return false;
    }

    public bool GetisDash()
    {
        return isDashing;
    }

}
