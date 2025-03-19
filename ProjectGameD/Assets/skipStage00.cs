using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skipStage00 : MonoBehaviour
{
        public CharacterData characterData;
    public GameObject Intro;
            public GameObject CamNormal;
            public GameObject CamIntro;

        public GameObject MCNormal;
    public GameObject BasicControl;

    public Intro intro;
        void Update()
    {
                if (Input.GetKeyDown(KeyCode.Escape) && characterData.deathCount==0)
        {
            intro.SetDeactiveDOF();
            intro.SetDeactiveEYE();
        
        CamIntro.SetActive(false);

            Intro.SetActive(false);
            BasicControl.SetActive(true);
            CamNormal.SetActive(true);

            MCNormal.SetActive(true);
        }
    }
}
