using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        PlayerStats playerStats;
        Transform cameraObject;
        InputHander inputHander;
        Vector3 moveDirection;
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHander animatorHander;
        public new Rigidbody rigidbody;
        public GameObject normalCameral;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        private float sprintStaminaTimer = 0f;
        public float staminaDrainInterval = 0.25f; // Drain every half second

        [Header("Stamina Costa")]
        [SerializeField]
        public int rollStaminaCost = 5;
        public int backstepStaminaCost = 1;
        public float sprintStaminaCost = 1f;

        public CapsuleCollider CharacterCollider;
        public CapsuleCollider CharacterCollisiomBlockerCollider;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            rigidbody = GetComponent<Rigidbody>();
            inputHander = GetComponent<InputHander>();
            animatorHander = GetComponentInChildren<AnimatorHander>();
        }

        void Start()
        {

            cameraObject = normalCameral.transform;
            myTransform = transform;
            animatorHander.Initialize();

            Physics.IgnoreCollision(CharacterCollider, CharacterCollisiomBlockerCollider, true);
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
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;
            float rs = rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;

        }
        public void HandleMovement(float delta)
        {
            if (inputHander.rollFlag)
                return;

            moveDirection = cameraObject.forward * inputHander.vertical;
            moveDirection += cameraObject.right * inputHander.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (inputHander.sprintFlag && inputHander.moveAmount > 0.5f && playerStats.currentStamina > 0)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;

                sprintStaminaTimer += delta;

                if (sprintStaminaTimer >= staminaDrainInterval)
                {
                    sprintStaminaTimer = 0f;
                    playerStats.TakeStaminaDamage((int)sprintStaminaCost);
                    //                    Debug.Log("running");
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection * speed, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHander.UpdateAnimatorValues(inputHander.moveAmount, 0, playerManager.isSprinting);

            if (animatorHander.canRotate)
            {
                HandleRotation(delta);
            }
        }
        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHander.anim.GetBool("isInteracting"))
                return;
            if (playerStats.currentStamina <= 0)
                return;
            if (inputHander.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHander.vertical;
                moveDirection += cameraObject.right * inputHander.horizontal;
                if (inputHander.moveAmount > 0)
                {
                    animatorHander.PlayTargetAnimation("Roll", true);
                    moveDirection.y = 0;
                    Quaternion rollRotaion = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotaion;
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                }
                else
                {
                    animatorHander.PlayTargetAnimation("Back Step", true);
                    playerStats.TakeStaminaDamage(backstepStaminaCost);
                }
            }
        }
        #endregion
    }
}
