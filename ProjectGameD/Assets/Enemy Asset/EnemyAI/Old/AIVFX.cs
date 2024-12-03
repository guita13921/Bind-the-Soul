using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AIVFX : MonoBehaviour
{
    [SerializeField] VisualEffect gameobject;

    void Start(){
        gameobject = GetComponent<VisualEffect>(); 
    }

    void OnTriggerEnter(Collider other){
        gameobject.enabled = true;
    }
}
