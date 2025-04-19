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
    public bool y_Input;
    public bool SHFIT_Input;
    public bool a_Input;
    public bool Al_Input;
    public bool Ah_Input;
    public bool k_Up;
    public bool k_Down;
    public bool k_Left;
    public bool k_Right;
    public bool Q_Input;
    public bool Lt_Input;

    public bool rollFlag;
    public bool twohandflag;
    public bool sprintFlag;
    public bool comboflang;
    public float rollInputTimer;


    PlayerControls inputAction;
    PlayerAttack playerAttack;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    BlockingColliderPlayer blockingColliderPlayer;


    Vector3 movementInput;
    Vector3 cameraInput;

    private void Awake()
    {
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        blockingColliderPlayer = GetComponentInChildren<BlockingColliderPlayer>();
    }

    public void OnEnable()
    {
        if (inputAction == null)
        {
            inputAction = new PlayerControls();
            inputAction.PlayerMovement.Movement.performed += inputAction => movementInput = inputAction.ReadValue<Vector3>();
            inputAction.PlayerAction.Roll.performed += i => b_Input = true;
            inputAction.PlayerAction.Roll.canceled += i => b_Input = false;
            inputAction.PlayerAction.Sprint.performed += i => SHFIT_Input = true;
            inputAction.PlayerAction.Sprint.canceled += i => SHFIT_Input = false;
            inputAction.PlayerAction.LT.performed += i => Lt_Input = true;
            inputAction.PlayerAction.Blocking.performed += i => Q_Input = true;
            inputAction.PlayerAction.Blocking.canceled += i => Q_Input = false;
            inputAction.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector3>();
            inputAction.PlayerAction.Y.performed += i => y_Input = true;
            inputAction.PlayerAction.Y.canceled += i => y_Input = false;

        }
        inputAction.Enable();
    }
    private void OnDisable()
    {
        inputAction.Disable();
    }
    public void TickInput(float delta)
    {
        if (playerStats.isDead)
            return;
        MoveInput(delta);
        HandleRollinput(delta);
        HandleSprintinput();
        HandleAttackInput(delta);
        HandleQuickSlotsInput();
        HandleInteractingButtonInput();
        HandleTwoHandInput();
    }
    private void MoveInput(float delta)
    {
        if (playerManager.isInteracting)
            return;
        horizontal = movementInput.x;
        vertical = movementInput.z;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.z;
    }

    private void HandleRollinput(float delta)
    {

        if (b_Input)
        {
            // ถ้าแตะปุ่มในระยะเวลา < 0.5 วิ → Roll
            if (rollInputTimer > 0 && rollInputTimer <= 0.5f && playerStats.currentStamina > 0)
            {
                rollFlag = true;

            }
            else
            {
                rollFlag = false;
            }

            // Reset
            rollInputTimer = 0;
            b_Input = false;

            return;
        }
        rollInputTimer += delta;
        if (playerStats.currentStamina <= 0)
        {
            rollFlag = false;
            b_Input = false;
            return;
        }
    }



    private void HandleSprintinput()
    {
        SHFIT_Input = inputAction.PlayerAction.Sprint.phase == InputActionPhase.Performed;

        if (SHFIT_Input)
        {
            if (playerStats.currentStamina <= 0)
            {
                SHFIT_Input = false;
                sprintFlag = false;
            }
            else
            {
                sprintFlag = true;
            }

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

        if (Q_Input)
        {
            playerAttack.HandleQAction();
        }
        else
        {
            playerManager.isBlocking = false;
            if (blockingColliderPlayer.blockingCollider.enabled)
            {
                blockingColliderPlayer.DisableBlockingCollider();
            }
        }

        if (Lt_Input)
        {
            if (twohandflag)
            {

            }
            else
            {
                playerAttack.HandleLTAction();
            }
        }

    }
    private void HandleTwoHandInput()
    {
        if (y_Input)
        {
            y_Input = false;
            twohandflag = !twohandflag;
            if (twohandflag)
            {
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            }
            else
            {
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
            }
        }
    }
    private void HandleQuickSlotsInput()
    {
        inputAction.PlayerQuickSlots.Right.performed += i => k_Right = true;
        inputAction.PlayerQuickSlots.Left.performed += i => k_Left = true;
        if (k_Right)
        {
            playerInventory.ChangeRightWeapon();
            playerInventory.ChangeLeftWeapon();
        }
        if (k_Left)
        {
            // playerInventory.ChangeLeftWeapon();
        }
    }
    private void HandleInteractingButtonInput()
    {
        inputAction.PlayerAction.A.performed += i => a_Input = true;
    }

}
