using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GatherInput();


        Attack();

 if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
    (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >0.8)){
            Look();}




        Reload();

        
        
    }


    

    private void FixedUpdate()
    {

        if (!isAttack && !isDashing)
        {
            Move();
        }

        if (Input.GetKey(KeyCode.L))
        {
            StartCoroutine(Dash());
        }

    
    }

}
