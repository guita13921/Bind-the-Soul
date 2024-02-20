using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] GameObject Sword_SFX;
    [SerializeField] GameObject Player_Walking_SFX;

    // Start is called before the first frame update
   
    public void Slash(){
        GameObject sfx = Instantiate(Sword_SFX);
    }

    public void PlayerWalking(){
        GameObject sfx = Instantiate(Player_Walking_SFX);

    }
}
