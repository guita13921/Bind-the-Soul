using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntroManager : MonoBehaviour
{

    public CharacterData characterData;
    public GameObject Intro;
            public GameObject CamNormal;
            public GameObject CamIntro;

        public GameObject MCNormal;
    public GameObject BasicControl;
    void Start()
    {
        if(characterData.deathCount ==0){
            CamNormal.SetActive(false);
            MCNormal.SetActive(false);
            BasicControl.SetActive(false);
            Intro.SetActive(true);
            CamIntro.SetActive(true);


        }else{
            CamIntro.SetActive(false);

            Intro.SetActive(false);
            BasicControl.SetActive(true);
            CamNormal.SetActive(true);

            MCNormal.SetActive(true);
        }
        
    }


}
