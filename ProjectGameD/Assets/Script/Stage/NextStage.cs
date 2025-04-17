using System;
using EasyTransition;
using UnityEngine;

public class NextStage : MonoBehaviour
{
    GameObject upgradeCanvas;
    public CharacterData characterData;
    public Health MChealth;
    public TransitionSettings transition;
    public float loaddelay;

    [SerializeField] public String currentScene;
    [SerializeField] public String NextSceneName;

    private float healthRatio = 1f;

    void Update()
    {

        upgradeCanvas = GameObject.Find("UpgradeCanvas(Clone)");
    }


    // Start is called before the first frame update
    public void loadscene(string NextSceneName)
    {
        characterData.rerollpoint += 2;
        CheckHPRestoreTHreshold();
        characterData.healthRatio = MChealth.currentHealth / MChealth.maxHealth;

        characterData.Health = MChealth.currentHealth;
        TransitionManager.Instance().Transition(NextSceneName, transition, loaddelay);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && upgradeCanvas == null)
            loadscene(NextSceneName);
    }

    void CheckHPRestoreTHreshold()
    {
        float threshold = 0.20f * characterData.healToThreshold;
        if (characterData.Health < (characterData.maxHealth * threshold))
        {
            characterData.Health = characterData.maxHealth * threshold;
            MChealth.currentHealth = characterData.Health;
        }
    }
}
