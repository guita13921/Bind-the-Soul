using System.Collections;
using System.Collections.Generic;
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
    public bool rollFlag;
    public float rollInputTimer;
    public bool isInteracting;
    PlayerControls inputAction;
    CameraHandler cameraHandler;

    Vector3 movementInput;
    Vector3 cameraInput;

    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }

    private void FixedUpdate()
    {
         float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta,mouseX,mouseY);
        }
    }
    public void OnEnable()
    {
        if(inputAction == null)
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
    public void TickInput (float delta)
    {
     MoveInput(delta);
     HandleRollinput(delta);
    }
    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.z;
        moveAmount= Mathf.Clamp01(Mathf.Abs(horizontal)+Mathf.Abs(vertical));
        mouseX =cameraInput.x;
        mouseY=cameraInput.z;
    }

    private void HandleRollinput(float delta)
{
        b_Input = inputAction.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
    if(b_Input)
    {
        rollInputTimer += delta;
    }
    else
    {
        if(rollInputTimer > 0 && rollInputTimer < 0.5f)
        {
            rollFlag =true;
        }
        rollInputTimer = 0;
    }
}
}
