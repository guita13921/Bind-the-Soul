using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SG
{
public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraObject;
    InputHander inputHander;
    Vector3 moveDirection;
    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimatorHander animatorHander;
    public new Rigidbody rigidbody;
    public GameObject normalCameral;

    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float rotationSpeed = 10;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        inputHander=GetComponent<InputHander>();
        animatorHander =GetComponentInChildren<AnimatorHander>();
        cameraObject = normalCameral.transform;
        myTransform = transform;
        animatorHander.Initialize();
    }

        public void Update()
        {
            float delta = Time.deltaTime;

            
            inputHander.TickInput(delta);
            HandleMovement(delta);
           HandleRollingAndSprinting(delta);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;
private void HandleRotation(float delta)
{
    Vector3 targetDir = Vector3.zero;
    float moveOverride = inputHander.moveAmount;

    targetDir = cameraObject.forward * inputHander.vertical;
    targetDir += cameraObject.right * inputHander.horizontal;
    targetDir.Normalize();
    targetDir.y=0;
    if (targetDir == Vector3.zero)
    targetDir = myTransform.forward;
    float rs =rotationSpeed;
    Quaternion tr =Quaternion.LookRotation(targetDir);
    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation,tr,rs * delta);

    myTransform.rotation =targetRotation;

}
public void HandleMovement(float delta)
{
            if(inputHander.rollFlag)
            return;
            moveDirection = cameraObject.forward * inputHander.vertical;
            moveDirection += cameraObject.right * inputHander.horizontal;
            moveDirection.Normalize();
            moveDirection.y=0;

            float speed = movementSpeed;
            moveDirection *= speed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection,normalVector);
            rigidbody.velocity=projectedVelocity;

            animatorHander.UpdateAnimatorValues(inputHander.moveAmount,0);

            if (animatorHander.canRotate)
            {
               HandleRotation(delta);
            }
}
public void HandleRollingAndSprinting(float delta)
{
    if(animatorHander.anim.GetBool("isInteracting"))
    return;
    if(inputHander.rollFlag)
    {
        moveDirection= cameraObject.forward*inputHander.vertical;
        moveDirection += cameraObject.right*inputHander.horizontal;
        if (inputHander.moveAmount>0)
        {
            animatorHander.PlayTargetAnimation("Roll",true);
            moveDirection.y=0;
            Quaternion rollRotaion = Quaternion.LookRotation(moveDirection);
            myTransform.rotation=rollRotaion;
        }
        else
        {
        animatorHander.PlayTargetAnimation("Back Step",true);
        }
    }
}
    #endregion
}
}
