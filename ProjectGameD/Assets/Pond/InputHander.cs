using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SG;
using Unity.VisualScripting;
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
    public bool critical_Attack_Input;
    public bool k_Up;
    public bool k_Down;
    public bool k_Left;
    public bool k_Right;
    public bool Q_Input;
    public bool Lt_Input;
    public bool lockOnInput;
    public bool right_Stick_Right_Input;
    public bool right_Stick_Left_Input;

    public bool rollFlag;
    public bool twohandflag;
    public bool sprintFlag;
    public bool comboflang;
    public bool lockOnFlag;
    public float rollInputTimer;

    public Transform CriticalAttackRayCastStartPoint;

    PlayerControls inputAction;
    PlayerAttack playerAttack;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    CameraHandler cameraHandler;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    BlockingColliderPlayer blockingColliderPlayer;

    [SerializeField] GameObject cameraObject;

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
        cameraHandler = FindObjectOfType<CameraHandler>();
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
            inputAction.PlayerAction.CriticalAttack.performed += i => critical_Attack_Input = true;
            inputAction.PlayerAction.LockOn.performed += i => lockOnFlag = true;
            inputAction.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
            inputAction.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Left_Input = true;

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
        HandleMoveInput(delta);
        HandleRollinput(delta);
        HandleSprintinput();
        HandleAttackInput(delta);
        HandleQuickSlotsInput();
        HandleInteractingButtonInput();
        HandleTwoHandInput();
        HandleCriticalAttackInput();
        HandleLockOnInput();
    }
    private void HandleMoveInput(float delta)
    {
        if (playerManager.isInteracting)
            return;

        // Step 1: Get raw input
        Vector3 input = new Vector3(movementInput.x, 0f, movementInput.z);

        // Step 2: Get camera-relative directions (flattened on Y axis)
        Vector3 cameraForward = cameraObject.transform.forward;
        Vector3 cameraRight = cameraObject.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Step 3: Create movement direction relative to camera
        Vector3 moveDirection = cameraForward * input.z + cameraRight * input.x;

        // Step 4: Set values
        horizontal = moveDirection.x;
        vertical = moveDirection.z;
        moveAmount = Mathf.Clamp01(moveDirection.magnitude);

        mouseX = cameraInput.x;
        mouseY = cameraInput.z;
    }

    private void HandleRollinput(float delta)
    {

        if (b_Input)
        {
            // ถ้าแตะปุ่มในระยะเวลา < 0.5 วิ → Roll
            //if (rollInputTimer > 0 && rollInputTimer <= 0.5f && playerStats.currentStamina > 0)
            if (rollInputTimer > 0 && playerStats.currentStamina > 0)
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

        if (playerManager.isDrawWeapon)
        {
            return;
        }

        if (Al_Input)
        {
            if (playerManager.CanDoCombo)
            {
                comboflang = true;
                Debug.Log("HandleWeaponCombo");
                playerAttack.HandleWeaponCombo(playerInventory.rightWeapon);
                comboflang = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.CanDoCombo)
                    return;

                Debug.Log("HandleLightAttack");
                playerAttack.HandleLightAttack(playerInventory.rightWeapon);
            }

        }

        if (Ah_Input)
        {
            Debug.Log("HandleHeavyAttack");
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
        /*
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
        */

    }

    private void HandleCombatInput(float delta)
    {

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

        if (playerManager.isInteracting)
        {
            return;
        }

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
    private void HandleCriticalAttackInput()
    {
        critical_Attack_Input = false;
        // playerAttack.AttemptBackStabOrRiposte();
    }
    private void HandleLockOnInput()
    {
        if (lockOnFlag && lockOnFlag == false)
        {

            lockOnInput = false;
            lockOnFlag = true;
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.ClearLockOnTargets();
        }

        if (lockOnFlag && right_Stick_Left_Input)
        {
            right_Stick_Left_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
            }
        }
        if (lockOnFlag && right_Stick_Right_Input)
        {
            right_Stick_Right_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
            }
        }
        cameraHandler.SetCameraHeight();
    }
}
