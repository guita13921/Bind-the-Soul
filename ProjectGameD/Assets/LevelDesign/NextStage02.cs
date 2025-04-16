using System;
using EasyTransition;
using UnityEngine;

namespace SG
{

    public class NextStage02 : MonoBehaviour
    {
        public TransitionSettings transition;
        public float loaddelay;

        [SerializeField] public String currentScene;
        [SerializeField] public String NextSceneName;

        public void loadscene(string NextSceneName)
        {
            TransitionManager.Instance().Transition(NextSceneName, transition, loaddelay);
        }

        public void loadRoom()
        {
            TransitionManager.Instance().Transition(transition, loaddelay);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") loadscene(NextSceneName);
        }

    }
}