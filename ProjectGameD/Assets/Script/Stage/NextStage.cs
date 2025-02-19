using System;
using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : MonoBehaviour
{
    GameObject upgradeCanvas;
    public CharacterData characterData;
    public Health MChealth;

    void Update()
    {
        // Find the GameObject named "upgradecanvas"
        upgradeCanvas = GameObject.Find("UpgradeCanvas(Clone)");
    }

    public TransitionSettings transition;
    public float loaddelay;

    [SerializeField]
    String currentScene;

    [SerializeField]
    String NextSceneName;

    public float healthRatio = 1f;

    // Start is called before the first frame update
    public void loadscene(string NextSceneName)
    {
        characterData.rerollpoint += 2;

        characterData.healthRatio = MChealth.currentHealth / MChealth.maxHealth;

        characterData.Health = MChealth.currentHealth;
        TransitionManager.Instance().Transition(NextSceneName, transition, loaddelay);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && upgradeCanvas == null)
            loadscene(NextSceneName);
    }
}
