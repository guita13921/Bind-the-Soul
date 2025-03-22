using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG 
{ 
public class PlayerManager : MonoBehaviour
{
    InputHander inputHander;
    Animator anim;
        public bool isInteracting;

    void Start()
    {
        inputHander = GetComponent<InputHander>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isInteracting = anim.GetBool("isInteracting");
        inputHander.rollFlag = false;
    }

    
       /*float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta,mouseX,mouseY);
        }*/
    
}
}