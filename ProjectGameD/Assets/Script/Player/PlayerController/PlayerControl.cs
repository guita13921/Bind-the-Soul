using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;

    private Animator animator;
    private void Start()
    {
        
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GatherInput();
        
        if(!GotHit){
        Attack();
        if (
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            || (
                animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8
            )
        )
        {
            Look();
        }
        } 

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit")){
            GotHit = true;
        }else{
            GotHit = false;

        }
    





    
        Reload();
    }

    [SerializeField]float speedwhengethit = 1.5f;
    private void FixedUpdate()
    {   

        if (!isAttack && !isDashing )
        {
            Move(_speed);
        }

        if(GotHit){
            Look();
            Move(speedwhengethit);

        }

        if (Input.GetKey(KeyCode.L))
        {
            StartCoroutine(Dash());
        }
    }
}
