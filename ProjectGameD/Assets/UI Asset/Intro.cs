using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject DOF;
    public GameObject BlackCir;
    public AudioSource whisper;
    public Animator animator; // Use Animator instead of AnimatorController
    public GameObject EYE;
    public GameObject BasicControl;

        public GameObject SecondCAM;

                public GameObject RealMC;
        public GameObject IntroMC;
        public GameObject sword;


    public void SetActiveDOF()
    {
        DOF.SetActive(true);
    }

    public void SetDeactiveDOF()
    {
        DOF.SetActive(false);
    }

    public void SetActiveBlackCir()
    {
        BlackCir.SetActive(true);
        
      
    }

    public void SetDeactiveBlackCir()
    {
        BlackCir.SetActive(false);
    }

    public void StandingUp(){

            animator.Play("Stand") ;
    }
        public void SetActiveEYE()
    {
        EYE.SetActive(true);
    }   public void SetDeactiveEYE()
    {
        EYE.SetActive(false);
                IntroMC.SetActive(false);
                                sword.SetActive(false);

                RealMC.SetActive(true);
                SecondCAM.SetActive(true);
BasicControl.SetActive(true);
        gameObject.SetActive(false);

        
    }
}
