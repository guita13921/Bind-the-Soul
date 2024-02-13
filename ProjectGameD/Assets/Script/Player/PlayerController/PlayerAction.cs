using System.Collections;
using UnityEngine;

public partial class PlayerControl
{
    [SerializeField] private float dashTime = 0.5f;

    [SerializeField] private float dashDistance = 2f;
    private bool isAttack = false;
    private bool isDashing = false;
    [SerializeField] private float movewhenATK = 0.185f;

    private void Attack()
    {
        
        if (Input.GetKey(KeyCode.J)){
            isAttack = true;
        }

        MoveWhenATK();

      


        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            isAttack =false;
        }


    }

    IEnumerator Dash()
    {
        isDashing = true;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            Vector3 dashMovement = transform.forward * dashDistance * Time.deltaTime;

            transform.position += dashMovement;

            yield return null;

            isDashing = true;
        }

        isDashing = false;
    }
}
