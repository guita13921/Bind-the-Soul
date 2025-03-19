using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBlock : MonoBehaviour
{
    private BoxCollider col;
    private PlayerControl playerControl;
    [SerializeField] VoidChecker voidChecker;


    void Start()
    {
        voidChecker = FindObjectOfType<VoidChecker>();
        playerControl = FindObjectOfType<PlayerControl>();
        col = GetComponent<BoxCollider>();
    }

    void Update()
    {
        CheckWhenDash();
    }

    void CheckWhenDash()
    {
        if (playerControl.GetisDash())
        {
            if (playerControl.GetisDash() && voidChecker.IsInVoid() == true)
            {
                col.enabled = true; // Disable collision when dashing
            }
            else
            {
                col.enabled = false;  // Enable collision when not dashing
            }
        }
        else
        {
            col.enabled = true;
        }
    }
}
