using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour{

    [SerializeField] String currentScene;
    [SerializeField] String NextSceneName;
    // Start is called before the first frame update
    public void loadscene(String NextSceneName){
        SceneManager.LoadScene(NextSceneName);
    }

    void OnTriggerEnter(){
        loadscene(NextSceneName);
    }
}
