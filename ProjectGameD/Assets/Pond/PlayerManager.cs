using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        InputHander inputHander;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;

        InteractbelUI interactbelUI;
        public GameObject interactbelUIGameObject;
        public GameObject itemInteractbleGameObject;
        public bool isInteracting;

        //[Header("Player Flges")]
        //public bool isSprinting;
        //public bool isUsingRightHand;
        //public bool isUsingLefthand;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        void Start()
        {
            inputHander = GetComponent<InputHander>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            interactbelUI = FindObjectOfType<InteractbelUI>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            CanDoCombo = anim.GetBool("CanDoCombo");

            inputHander.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            isUsingLefthand = anim.GetBool("isUsingLefthand");
            isUsingRightHand = anim.GetBool("isUsingRightHand");

            CheckForInteractableObjiect();

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
            inputHander.k_Up = false;
            inputHander.k_Down = false;
            inputHander.k_Right = false;
            inputHander.k_Left = false;
            inputHander.a_Input = false;
            //Debug.Log(inputHander.b_Input);
        }

        public void CheckForInteractableObjiect()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    interactable interactableObject = hit.collider.GetComponent<interactable>();
                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        interactbelUI.interactableText.text = interactableText;
                        interactbelUIGameObject.SetActive(true);
                    }
                    if (inputHander.a_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
            else
            {
                if (interactbelUIGameObject != null)
                {
                    interactbelUIGameObject.SetActive(false);
                }
                if (itemInteractbleGameObject != null && inputHander.a_Input)
                {
                    itemInteractbleGameObject.SetActive(false);
                }
            }
        }

    }
}