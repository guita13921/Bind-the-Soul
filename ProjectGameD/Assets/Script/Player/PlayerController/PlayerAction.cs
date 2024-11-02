using System.Collections;
using UnityEngine;

public partial class PlayerControl
{
    [SerializeField]
    private float dashTime = 0.5f;

    [SerializeField]
    private float dashDistance = 2f;
    private bool isAttack = false;
    private bool isDashing = false;

    [SerializeField]
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

    IEnumerator Dash()
    {
        isDashing = true;
        animator.Play("Dash", 0, 0);
        bool isCollide = false;
        // Calculate the direction to dash in once, so we avoid issues if the character rotates
        Vector3 dashDirection = transform.forward.normalized;

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            // Move the character along the dash direction with collision detection
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dashDirection, out hit, 1f))
            {
                // If hit an object with the "FF" tag, stop dashing immediately
                if (hit.collider.gameObject.tag == "DD")
                {
                    isCollide = true;
                    Debug.Log("gg");
                }
            }
            if (!isCollide)
            {
                // Otherwise, move by the remaining distance or time
                Vector3 dashMovement = dashDirection * (dashDistance / dashTime) * Time.deltaTime;

                transform.position += dashMovement;
            }

            yield return null;
        }

        isDashing = false; // Reset dashing state when done
    }
}
