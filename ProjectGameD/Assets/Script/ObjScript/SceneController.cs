using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]Door door;
    float detonationTime;
    float bombTimer;
    [SerializeField] String currentScene;
    [SerializeField] String NextSceneName;
    void Start(){
        detonationTime = 5;
        currentScene = SceneManager.GetActiveScene().name;
    }
    public void loadscene(String NextSceneName){
        SceneManager.LoadScene(NextSceneName);
    }
    void Update(){
        if(door.IsOpen){
            bombTimer += Time.deltaTime;
            if(bombTimer > detonationTime) loadscene(NextSceneName);
        }
    }
}
