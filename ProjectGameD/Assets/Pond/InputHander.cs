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

    [SerializeField] private InputAction mouseDeltaAction;
    private float mouseFlickThreshold = 30f; // Adjust this for sensitivity
    private float flickCooldown = 0.3f;
    private float lastFlickTime = -1f;

    public bool rollFlag;
    public bool twohandflag;
    public bool sprintFlag;
    public bool comboflang;
    public bool lockOnFlag;
    public float rollInputTimer;

    public Transform CriticalAttackRayCastStartPoint;
    public float rayLength = 1f;

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

    private float attackHoldTimer = 0f;
    private bool isAttackHeld = false;
    private bool hasHeavyAttacked = false;
    private float repeatAttackCooldown = 0f; // Time between swings
    private float repeatAttackTimer = 0f;


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
        HandleMouseAim();
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
        bool isLeftPressed = Mouse.current.leftButton.wasPressedThisFrame;
        bool isLeftReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        // Handle swing start
        if (isLeftPressed)
        {
            if (!playerManager.isInteracting && playerManager.isBlocking)
            {
                isAttackHeld = true;
                playerAttack.HandleStartSwing(playerInventory.rightWeapon);
            }
            else
            {
                playerAttack.HandleALAction();
            }
        }

        // Handle swing stop
        if (isLeftReleased)
        {
            isAttackHeld = false;
            playerAttack.HandleStopSwing();
        }

        // Handle Q Input (Block)
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

        // Handle LT input (Special)
        if (Lt_Input)
        {
            if (!playerManager.isBlocking || playerManager.playerAttack.currentKnifeCharges <= 0)
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

    public void HandleMouseAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (playerManager.isInteracting)
        {
            if (playerManager.canRotate)
            {
                lockOnFlag = true;

                if (groundPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    Vector3 direction = hitPoint - transform.position;
                    direction.y = 0f;

                    if (direction.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * cameraHandler.rotationSpeed);
                    }
                }
            }
        }
        else
        {
            lockOnFlag = true;

            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = hitPoint - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * cameraHandler.rotationSpeed);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (CriticalAttackRayCastStartPoint == null)
            return;

        // Set gizmo color
        Gizmos.color = Color.red;

        // Draw ray direction from the raycast start point forward
        Vector3 rayDirection = transform.TransformDirection(Vector3.forward);

        // Draw the ray in scene view
        Gizmos.DrawRay(CriticalAttackRayCastStartPoint.position, rayDirection * rayLength);

        // Optionally, draw a small sphere at the origin for visibility
        Gizmos.DrawWireSphere(CriticalAttackRayCastStartPoint.position, 0.05f);
    }
}