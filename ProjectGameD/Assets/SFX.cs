using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] GameObject Sword_SFX;


    // Start is called before the first frame update
   
    public void Slash(){
        GameObject sfx = Instantiate(Sword_SFX);

    }


}
