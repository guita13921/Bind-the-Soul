using System.Collections;
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
        if (!isAttack && !isDashing)
        {
            Look();
        }
        Reload();
        Attack();
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
