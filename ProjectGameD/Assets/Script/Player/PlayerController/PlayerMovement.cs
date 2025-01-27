using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerControl : MonoBehaviour
{
    [SerializeField]
    public float _speed = 3.5f;

    [SerializeField]
    private float _turnSpeed = 500;

    [SerializeField]
    private float movewhenATKduration_0to1 = 0.2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Move(float speed)
    {
        Vector3 movement = transform.forward * _input.normalized.magnitude * speed * Time.deltaTime;
        _rb.MovePosition(_rb.position + movement);
    }

    private void MoveWhenATK()
    {
        if (
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < movewhenATKduration_0to1
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
        )
        {
            Vector3 attackMovement = transform.forward * _speed * Time.deltaTime; // Use _speed or another attack-specific speed
            _rb.MovePosition(_rb.position + attackMovement / 3);
        }
    }

    private void Look()
    {
        if (_input.magnitude > 0.01f)
        {
            var dot = Vector3.Dot(transform.forward, _input.normalized);
            var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);

            if (dot < -0.2f)
            {
                transform.rotation = rot;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rot,
                    _turnSpeed * Time.deltaTime
                );
            }
        }
    }

    private void WalkingSFX()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Pause();
        }
    }
}
