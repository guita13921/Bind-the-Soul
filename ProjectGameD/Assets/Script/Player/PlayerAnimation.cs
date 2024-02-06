using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;


    private float keyPressStartTime;
    bool ConflictInputDetect;

    HashSet<KeyCode> keysPressed = new HashSet<KeyCode>();

    void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        ConflictInputDetecter();
        if (!ConflictInputDetect )
        {
            
            WalkAndRun();
        }
        
    }

    void WalkAndRun()
    {
        bool Walking =
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);



       
        if (Walking)
        {
            animator.SetBool("isRunning", true);

        }else{
            animator.SetBool("isRunning", false);
         
        }

        

        


    }

    void ConflictInputDetecter()
    {
        UpdateKeysPressed();

        bool conflictInput =
            (
                keysPressed.Contains(KeyCode.W)
                && keysPressed.Contains(KeyCode.S)
            )
            || (
                keysPressed.Contains(KeyCode.A)
                && keysPressed.Contains(KeyCode.D)

            );

        if (conflictInput && keysPressed.Count ==2)
        {
            keyPressStartTime += Time.deltaTime;
            //Debug.Log(keyPressStartTime);
            if (keyPressStartTime > 0.1f)
            {
                ConflictInputDetect = true;
                animator.SetBool("ConflictInput", true);      
            }
        }
        else
        {
            animator.SetBool("ConflictInput", false);
            keyPressStartTime = 0f;
            ConflictInputDetect = false;
        }
    }

    void UpdateKeysPressed()
    {
        keysPressed.Clear();

        if (Input.GetKey(KeyCode.W))
            keysPressed.Add(KeyCode.W);
        if (Input.GetKey(KeyCode.A))
            keysPressed.Add(KeyCode.A);
        if (Input.GetKey(KeyCode.S))
            keysPressed.Add(KeyCode.S);
        if (Input.GetKey(KeyCode.D))
            keysPressed.Add(KeyCode.D);
    }

    
}
