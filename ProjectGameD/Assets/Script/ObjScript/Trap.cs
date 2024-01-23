using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public enum State {Trigger,Idle,Cooldown};
    [SerializeField] public State state;
    [SerializeField]public float timeRemaining = 1;
    [SerializeField]public double delaytimeRemaining = 0.2;

    private void Start(){
        this.state = State.Idle;
    }

    private void Reset(){
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider collider){
        if(collider.tag == "Player" && state == State.Idle){
            this.state = State.Trigger;
            //transform.position = transform.position + new Vector3 (0f,0.4f,0f);
            Debug.Log("Trap trigger");
        }
    }


    void Update(){

        if(state == State.Trigger && delaytimeRemaining > 0){
            delaytimeRemaining -= Time.deltaTime;
        }else if(state == State.Trigger && delaytimeRemaining <= 0){
            transform.position = transform.position + new Vector3 (0f,0.4f,0f);
            state = State.Cooldown;
        }

        if(state == State.Cooldown && timeRemaining > 0){
            timeRemaining -= Time.deltaTime;
        }else if(state == State.Cooldown && timeRemaining <= 0){
            transform.position = transform.position + new Vector3 (0f,-0.4f,0f);
            state = State.Idle;
            timeRemaining = 1;
            delaytimeRemaining = 0.2;
        }
    }
}
