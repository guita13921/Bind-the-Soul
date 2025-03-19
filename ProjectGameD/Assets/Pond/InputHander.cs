using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHander : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;
    
    PlayerControls inputAction;

    Vector3 movementInput;
    Vector3 cameraInput;
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
    }
    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.z;
        moveAmount= Mathf.Clamp01(Mathf.Abs(horizontal)+Mathf.Abs(vertical));
        mouseX =cameraInput.x;
        mouseY=cameraInput.z;
    }
}
