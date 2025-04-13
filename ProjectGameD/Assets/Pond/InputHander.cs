using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SG;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHander : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool a_Input;
    public bool Al_Input;
    public bool Ah_Input;
    public bool k_Up;
    public bool k_Down;
    public bool k_Left;
    public bool k_Right;
    public bool Lt_Input;

    public bool rollFlag;
    public bool sprintFlag;
    public bool comboflang;
    public float rollInputTimer;


    PlayerControls inputAction;
    PlayerAttack playerAttack;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    PlayerStats playerStats;


    Vector3 movementInput;
    Vector3 cameraInput;

    private void Awake()
    {
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
    }

    public void OnEnable()
    {
        if (inputAction == null)
        {
            inputAction = new PlayerControls();
            inputAction.PlayerMovement.Movement.performed += inputAction => movementInput = inputAction.ReadValue<Vector3>();
            inputAction.PlayerAction.Roll.performed += i => b_Input = true;
            inputAction.PlayerAction.Roll.canceled += i => b_Input = false;
            inputAction.PlayerAction.LT.performed += i => Lt_Input = true;
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
        HandleQuickSlotsInput();
        HandleInteractingButtonInput();
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


        if (b_Input)
        {
            rollInputTimer += delta;
            if (playerStats.currentStamina <= 0)
            {
                b_Input = false;
                sprintFlag = false;
            }
            if (moveAmount > 0.5f && playerStats.currentStamina > 0)
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {

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
        if (Lt_Input)
        {

        }

    }
    private void HandleQuickSlotsInput()
    {
        inputAction.PlayerQuickSlots.Right.performed += i => k_Right = true;
        inputAction.PlayerQuickSlots.Left.performed += i => k_Left = true;
        if (k_Right)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (k_Left)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }
    private void HandleInteractingButtonInput()
    {
        inputAction.PlayerAction.A.performed += i => a_Input = true;
    }

}
