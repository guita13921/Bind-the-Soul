using System.Collections;
using UnityEngine;

public partial class PlayerControl
{
    private bool isAttack = false;
    private bool isDashing = false;

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

    [SerializeField]private float dashDistance = 2f; // Total distance to dash
    [SerializeField]private float dashTime = 0.5f;

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
        //Debug.Log(dashCheck.willCollide);

           // capsuleCollider.enabled = false;


        GameObject instantiatedObject = Instantiate(
            prefabToInstantiate,
            spawnPosition,
            Quaternion.identity
        );

        yield return null; // Allows one frame to pass
        //Debug.Log(dashCheck.willCollide);

        if (!dashCheck.willCollide)
        {
            //  Debug.Log("not hit");
            Physics.IgnoreLayerCollision(7, 9, true);
        }

        while (distanceTraveled < dashDistance)
        {
            CheckForCollision = CheckForCollisions();
            float dashStep = (dashDistance / dashTime) * Time.deltaTime;
            Vector3 dashMovement = dashDirection * dashStep;

            if (!CheckForCollision || !dashCheck.willCollide)
            {
                transform.position += dashMovement;
            }

            distanceTraveled += dashStep;

            yield return null;
        }
          //  capsuleCollider.enabled = true;

        Physics.IgnoreLayerCollision(7, 9, false);
        
        // Ensure the dash animation ends
        animator.Play("Idle");
        isDashing = false;

        // Wait for the cooldown to complete
        yield return new WaitForSeconds(dashWaitTime);
        canDash = true; // Reset the cooldown
    }

    private bool CheckForCollisions()
    {
        // Example: Use a sphere overlap check or a raycast to check for collisions
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("DD"))
            {
                Debug.Log("Collided during dash");
                return true; // Collision detected
            }
        }

        return false; // No collision detected
    }
}
