using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;

    private Animator animator;
    private AudioSource audioSource;
    private CapsuleCollider capsuleCollider;
    private bool isDead;
    private Health health;
    public ControlPower controlPower;

    private void Start()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Vector3 dashDirection = transform.forward.normalized;

        Debug.DrawLine(
            transform.position,
            transform.position + dashDirection * dashDistance,
            Color.red
        );

        if (health.currentHealth > 0)
        {
            Imframe();
            GatherInput();
            WalkingSFX();

            if (!GotHit && !isDashing)
            {
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

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit"))
            {
                GotHit = true;
            }
            else
            {
                GotHit = false;
            }
        }
        //Reload();
    }

    [SerializeField]
    float speedwhengethit = 1.5f;

    private void FixedUpdate()
    {
        if (health.currentHealth > 0)
        {
            if (!isAttack && !isDashing)
            {
                Move(_speed);
            }

            if (GotHit)
            {
                Look();
                Move(speedwhengethit);
            }

            if (Input.GetKey(KeyCode.L) && !isDashing)
            {
                StartCoroutine(Dash());
                controlPower.DashVFX();
            }
        }
        else { }
    }
}
