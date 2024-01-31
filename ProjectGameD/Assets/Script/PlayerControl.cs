using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;

    [SerializeField]
    private float _speed = 6.5f;

    [SerializeField]
    private float _turnSpeed = 500;
    private Vector3 _input;

    bool isSprint = false;
    bool isAttack = false;
    bool isDashing = false;

       [SerializeField]
 float dashTime = 0.5f;
       [SerializeField] float  dashDistance =2f;
 
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GatherInput();
        if(!isAttack && !isDashing){
        Look();
       }
        Reload();
        Attack();

        
    }

    private void FixedUpdate()
    {

        if(!isAttack && !isDashing){
        Move();}
        if(Input.GetKey(KeyCode.L)){
            StartCoroutine(Dash());
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
    


    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.M))
        {
            isSprint = true;
        }
        else
        {
            isSprint = false;
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

    void Attack(){
        if(Input.GetKey(KeyCode.J)){
            isAttack = true;
        }      
     if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
        isAttack=false;
     }
    }

    private void Move()
    {
        float currentSpeed = _speed;

        if (isSprint)
        {
            currentSpeed = 10f;
        }
   
        _rb.MovePosition(
            transform.position
                + transform.forward * _input.normalized.magnitude * currentSpeed * Time.deltaTime
        );
    }

    void Reload()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(currentSceneIndex);
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
