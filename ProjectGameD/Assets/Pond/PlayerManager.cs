using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        InputHander inputHander;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        public bool isInteracting;

        [Header("Player Flges")]
        public bool isSprinting;
        public bool isUsingRightHand;
        public bool isUsingLefthand;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        void Start()
        {
            inputHander = GetComponent<InputHander>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");

            inputHander.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            isUsingLefthand = anim.GetBool("isUsingLefthand");
            isUsingRightHand = anim.GetBool("isUsingRightHand");

        }
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHander.mouseX, inputHander.mouseY);
            }
        }
        private void LateUpdate()
        {
            inputHander.rollFlag = false;
            inputHander.sprintFlag = false;
            isSprinting = inputHander.b_Input;
            inputHander.Al_Input = false;
            inputHander.Ah_Input = false;
            //Debug.Log(inputHander.b_Input);
        }

    }
}