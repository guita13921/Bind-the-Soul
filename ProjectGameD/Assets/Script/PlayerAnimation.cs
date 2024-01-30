using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    float velocity;
    int VelocityHash;
    public float acceleration = 2f;
        public float deacceleration = 2f;

    private float keyPressStartTime;
    bool ConflictInputDetect;

    HashSet<KeyCode> keysPressed = new HashSet<KeyCode>();

    void Start()
    {
        animator = GetComponent<Animator>(); 
        VelocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        ConflictInputDetecter();
        if (!ConflictInputDetect )
        {
            WalkAndRun();
            animator.SetFloat(VelocityHash, velocity);
        }
    }

    void WalkAndRun()
    {
        bool Walking =
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);

        if (Walking && Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.M))
        {
            velocity += Time.deltaTime * acceleration;
            velocity = Mathf.Clamp(velocity, 0f, 1f);
        }
        else
        {
            velocity -= Time.deltaTime * deacceleration;
            velocity = Mathf.Max(velocity, 0.01f);
        }
        if (!Walking)
        {
            velocity = 0f;
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

        if (conflictInput)
        {
            keyPressStartTime += Time.deltaTime;
            //Debug.Log(keyPressStartTime);
            if (keyPressStartTime > 0.1f)
            {
                animator.SetBool("ConflictInput", true);
                ConflictInputDetect = true;
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
