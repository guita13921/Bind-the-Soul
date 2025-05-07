using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {
        CameraHandler cameraHandler;
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
            cameraHandler = FindObjectOfType<CameraHandler>();
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
            if (inputHander.lockOnFlag)
            {
                if (inputHander.sprintFlag || inputHander.rollFlag)
                {
                    Vector3 targetDirection = cameraHandler.cameraTransform.forward * inputHander.vertical;
                    targetDirection += cameraHandler.cameraTransform.right * inputHander.horizontal;
                    targetDirection.y = 0;

                    if (targetDirection.sqrMagnitude == 0)
                    {
                        targetDirection = transform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targetDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
                else
                {
                    if (cameraHandler.currentLockOnTarget == null || !cameraHandler.currentLockOnTarget.gameObject.activeInHierarchy)
                        return;

                    Vector3 rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            else
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
        }

        public void HandleMovement(float delta)
        {
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
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection * speed, normalVector);
            rigidbody.velocity = projectedVelocity;

            if (inputHander.lockOnFlag && inputHander.sprintFlag == false)
            {
                animatorHander.UpdateAnimatorValues(inputHander.vertical, inputHander.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHander.UpdateAnimatorValues(inputHander.moveAmount, 0, playerManager.isSprinting);
            }

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

                if (playerManager.weaponSlotManager.rightHandSlot.currentWeaponItem.stantType == StantType.Medium)
                {
                    Roll(0.50f);
                }
                else if (playerManager.weaponSlotManager.rightHandSlot.currentWeaponItem.stantType == StantType.Heavy)
                {
                    Roll(0.80f);
                }
                else if (playerManager.weaponSlotManager.rightHandSlot.currentWeaponItem.stantType == StantType.Light)
                {
                    Roll(1.00f);
                }
                else
                {
                    Roll(1.00f);
                }
            }

        }


        public void Roll(float speed)
        {
            if (inputHander.moveAmount > 0)
            {
                animatorHander.PlayTargetAnimation("Roll", true, false, speed);
                moveDirection.y = 0;
                Quaternion rollRotaion = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotaion;
                playerStats.TakeStaminaDamage(rollStaminaCost);
                animatorHander.anim.SetBool("IsInvulnerable", true);
            }
            else
            {
                animatorHander.PlayTargetAnimation("Back Step", true, false, speed);
                playerStats.TakeStaminaDamage(backstepStaminaCost);
                animatorHander.anim.SetBool("IsInvulnerable", true);
            }
        }

        #endregion
    }
}
