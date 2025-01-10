using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public TransitionSettings transition;
    public float loaddelay;

    public void NextScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, loaddelay);
    }
}
