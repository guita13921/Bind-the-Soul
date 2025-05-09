using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        InputHander inputHander;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;

        public Animator anim;
        public AnimatorHander animatorHander;
        public PlayerStats playerStats;
        public PlayerAttack playerAttack;
        public PlayerData playerData;

        public bool isInteracting;
        public bool isInvulerable;
        public bool isDrawWeapon;

        public static Room currentRoom;
        public static new Transform transform;
        public static List<Drop> availableDrops = new List<Drop>();
        public static List<ShopItem> availableShopItem = new List<ShopItem>();

        protected override void Awake()
        {
            base.Awake();
            animatorHander = GetComponentInChildren<AnimatorHander>();
            cameraHandler = CameraHandler.singleton;
            //backStabCollider = GetComponentInChildren<CriticalDamageCollider>();
        }

        void Start()
        {
            inputHander = GetComponent<InputHander>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerStats = GetComponent<PlayerStats>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            CanDoCombo = anim.GetBool("CanDoCombo");
            isInvulerable = anim.GetBool("IsInvulnerable");
            anim.SetBool("IsBlocking", isBlocking);
            isDrawWeapon = anim.GetBool("isDrawWeapon");
            anim.SetBool("isDead", playerStats.isDead);

            inputHander.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            isUsingLefthand = anim.GetBool("isUsingLefthand");
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            playerStats.RegenerateStamina();


            //CheckForInteractableObjiect();

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
            isSprinting = inputHander.SHFIT_Input;
            inputHander.Al_Input = false;
            inputHander.Ah_Input = false;
            inputHander.Lt_Input = false;
            inputHander.k_Up = false;
            inputHander.k_Down = false;
            inputHander.k_Right = false;
            inputHander.k_Left = false;
            inputHander.a_Input = false;
        }

        public void CheckForInteractableObjiect()
        {
            RaycastHit hit;
            if (cameraHandler != null && Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    interactable interactableObject = hit.collider.GetComponent<interactable>();
                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                    }
                    if (inputHander.a_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }

    }
}
