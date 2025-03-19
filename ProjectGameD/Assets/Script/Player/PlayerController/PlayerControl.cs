using UnityEngine;

public partial class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;

    private Animator animator;
    private AudioSource audioSource;
    private CapsuleCollider capsuleCollider;
    private bool isDead;
    private Health health;
    private void Start()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (health.currentHealth > 0)
        {
            //Imframe();
            GatherInput();
            WalkingSFX();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("CAST"))
                isAttack = false;
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

    private bool canDash = true; // Tracks whether dashing is allowed.

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
            }
        }
        else { }
    }
}
