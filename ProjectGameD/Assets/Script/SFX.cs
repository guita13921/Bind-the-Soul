using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] GameObject Sword_SFX;
        [SerializeField] GameObject SSword_SFX;

    [SerializeField] GameObject Hit_SFX;


    // Start is called before the first frame update
   
    public void Slash(){
        GameObject sfx = Instantiate(Sword_SFX);

    }
    public void SkillSlash(){
        GameObject sfx = Instantiate(SSword_SFX);

    }
    public void Hit(){
        GameObject sfx = Instantiate(Hit_SFX);

    }


}
