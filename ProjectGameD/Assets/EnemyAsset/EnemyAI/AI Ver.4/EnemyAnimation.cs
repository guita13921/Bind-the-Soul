using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    [SerializeField] private float currentVelocity;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");

        // Set the weight of the "Upper Layer" to 1
        int upperLayerIndex = animator.GetLayerIndex("Upper Layer");
        if (upperLayerIndex != -1) // Ensure the layer exists
        {
            animator.SetLayerWeight(upperLayerIndex, 1f);
        }
    }

    public void PlayShieldAnimation()
    {
        animator.SetBool("Shield", true);
    }

    public void UpdateAnimator(float speed)
    {
        animator.SetFloat("Speed", speed); // Adjust animation based on speed
    }

    public void CalculateVelocity()
    {
        // Calculate velocity based on position change over time
        currentVelocity = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
        lastPosition = transform.position; // Update last position
        UpdateAnimator(currentVelocity);
    }

}