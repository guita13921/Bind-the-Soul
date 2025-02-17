using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSwordAnimation : MonoBehaviour
{
public Animator animator;
    public GameObject sword;
    public GameObject player;
    bool startmove = false;
    public float moveSpeed = 2;

    public Animator cam;

    void Start()
    {



}
    public  void Swrod()
    {
             //sword.transform.position = new Vector3( sword.transform.position.x, 1.926469f,  sword.transform.position.z);
               //sword.transform.rotation = Quaternion.Euler(98.22501f, 18.675f, 117.289f);
                               Rigidbody rb = sword.GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.None; // Unfreeze all constraints
        Collider col = sword.GetComponent<Collider>();
            col.enabled = true; // Activate collider
    }

    public void Run(){
                 //   animator.Play("FinalRun");

    }

        

        public void Run2(){
            animator.Play("FinalRun2");
startmove = true;
    }

   void Update()
    {
        if (startmove && player != null)
        {
            player.transform.position += player.transform.forward * moveSpeed * Time.deltaTime;
        }
    }


}
