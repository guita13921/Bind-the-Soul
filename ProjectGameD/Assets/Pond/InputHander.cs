using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SG
{

    public class InputHander : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input;
        public bool Al_Input;
        public bool Ah_Input;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboflang;
        public float rollInputTimer;


        PlayerControls inputAction;
        PlayerAttack playerAttack;
        PlayerInventory playerInventory;
        PlayerManager playerManager;


        Vector3 movementInput;
        Vector3 cameraInput;

        private void Awake()
        {
            playerAttack = GetComponent<PlayerAttack>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
        }

        public void OnEnable()
        {
            if (inputAction == null)
            {
                inputAction = new PlayerControls();
                inputAction.PlayerMovement.Movement.performed += inputAction => movementInput = inputAction.ReadValue<Vector3>();
                inputAction.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector3>();
            }
            inputAction.Enable();
        }
        private void OnDisable()
        {
            inputAction.Disable();
        }
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollinput(delta);
            HandleSprintinput();
            HandleAttackInput(delta);
        }
        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.z;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.z;
        }
        /*
        private void HandleRollinput(float delta)
        {
            b_Input = inputAction.PlayerAction.Roll.phase == InputActionPhase.Started;
            if (b_Input)
            {
                rollInputTimer += delta;
                sprintFlag = true;
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }
        */

        private void HandleRollinput(float delta)
        {
            b_Input = inputAction.PlayerAction.Roll.phase == InputActionPhase.Started;
            if (b_Input)
            {
                rollInputTimer += delta;
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleSprintinput()
        {
            b_Input = inputAction.PlayerAction.Sprint.phase == InputActionPhase.Performed; // Use Performed for better control
                                                                                           // Debug.Log("Sprint Input: " + b_Input);

            if (b_Input)
            {
                sprintFlag = true;
            }
            else
            {
                sprintFlag = false; // Reset flag when input is released
            }
        }
        private void HandleAttackInput(float delta)
        {
            inputAction.PlayerAction.AttackL.performed += i => Al_Input = true;
            inputAction.PlayerAction.AttackH.performed += i => Ah_Input = true;

            if (Al_Input)
            {
                if (playerManager.CanDoCombo)
                {
                    comboflang = true;
                    playerAttack.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboflang = false;
                }
                else
                {
                    if (playerManager.isInteracting)
                        return;
                    if (playerManager.CanDoCombo)
                        return;
                    playerAttack.HandleLightAttack(playerInventory.rightWeapon);
                }

            }
            if (Ah_Input)
            {
                playerAttack.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }


    }
}