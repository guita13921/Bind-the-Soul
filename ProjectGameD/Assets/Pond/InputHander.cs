using System.Diagnostics;
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
    [SerializeField] CameraHandler cameraHandler;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    BlockingColliderPlayer blockingColliderPlayer;

    [SerializeField] GameObject cameraObject;

    //Vector3 movementInput;
    //Vector3 cameraInput;

    Vector2 movementInput;
    Vector2 cameraInput;

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

    void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
        }
    }

    public void OnEnable()
    {
        if (inputAction == null)
        {
            inputAction = new PlayerControls();
            inputAction.PlayerMovement.Movement.performed += inputAction => movementInput = inputAction.ReadValue<Vector2>();
            inputAction.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputAction.PlayerAction.Roll.performed += i => b_Input = true;
            inputAction.PlayerAction.Roll.canceled += i => b_Input = false;
            inputAction.PlayerAction.Sprint.performed += i => SHFIT_Input = true;
            inputAction.PlayerAction.Sprint.canceled += i => SHFIT_Input = false;
            inputAction.PlayerAction.LT.performed += i => Lt_Input = true;
            inputAction.PlayerAction.Blocking.performed += i => Q_Input = true;
            inputAction.PlayerAction.Blocking.canceled += i => Q_Input = false;
            inputAction.PlayerAction.Y.performed += i => y_Input = true;
            inputAction.PlayerAction.CriticalAttack.performed += i => critical_Attack_Input = true;
            inputAction.PlayerAction.LockOn.performed += i => lockOnInput = true;
            inputAction.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
            inputAction.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;

        }
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    public void TickInput(float delta)
    {
        if (playerStats.isDead) return;
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
        {
            return;
        }

        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollinput(float delta)
    {
        if (b_Input)
        {
            if (rollInputTimer > 0 && playerStats.currentStamina > 0)
            {
                rollFlag = true;
            }
            else
            {
                rollFlag = false;
            }

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
        if (playerManager.isInteracting)
        {
            return;
        }

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
            if (playerManager.isBlocking == false || playerManager.playerAttack.currentKnifeCharges <= 0)
            {
                playerAttack.HandleArtAction();
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
        if (critical_Attack_Input)
        {
            critical_Attack_Input = false;
            playerAttack.AttemptBackStabOrRiposte();
        }

    }

    private void HandleLockOnInput()
    {
        if (lockOnInput && lockOnFlag == false)
        {
            lockOnInput = false;
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

        // Continuously update lock-on target while active
        if (lockOnFlag)
        {
            cameraHandler.HandleLockOn();

            if (right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                if (cameraHandler.leftLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                if (cameraHandler.rightLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }
            cameraHandler.SetCameraHeight();
        }

    }

}