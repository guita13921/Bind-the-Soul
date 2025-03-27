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
    public bool sprintFlag;
    public float rollInputTimer;


    PlayerControls inputAction;


    Vector3 movementInput;
    Vector3 cameraInput;



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
        //Debug.Log("Sprint Input: " + b_Input);

        if (b_Input)
        {
            sprintFlag = true;
        }
        else
        {
            sprintFlag = false; // Reset flag when input is released
        }
    }


}
